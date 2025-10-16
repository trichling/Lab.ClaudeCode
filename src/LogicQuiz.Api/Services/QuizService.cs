using Microsoft.EntityFrameworkCore;
using LogicQuiz.Api.Data;
using LogicQuiz.Api.DTOs;
using LogicQuiz.Api.Models;

namespace LogicQuiz.Api.Services;

public class QuizService : IQuizService
{
    private readonly QuizDbContext _context;

    public QuizService(QuizDbContext context)
    {
        _context = context;
    }

    private static int GetQuestionCount(int difficulty)
    {
        return difficulty switch
        {
            1 => 3,  // Easy: 3 questions
            2 => 5,  // Medium: 5 questions
            3 => 10, // Hard: 10 questions
            _ => 3
        };
    }

    public async Task<GameStateDto> StartGameAsync(string playerName, int difficulty)
    {
        // Validate difficulty
        if (difficulty < 1 || difficulty > 3)
        {
            throw new ArgumentException("Difficulty must be 1 (Easy), 2 (Medium), or 3 (Hard)");
        }

        // Get question count based on difficulty
        var questionCount = GetQuestionCount(difficulty);

        // Get all questions and select random ones
        var allQuestions = await _context.Questions
            .Include(q => q.CorrectFallacyType)
            .ToListAsync();

        if (allQuestions.Count < questionCount)
        {
            throw new InvalidOperationException($"Not enough questions in database. Need at least {questionCount}");
        }

        var random = new Random();
        var selectedQuestions = allQuestions
            .OrderBy(x => random.Next())
            .Take(questionCount)
            .ToList();

        // Get fallacy types based on difficulty
        var fallacyTypes = await GetFallacyTypesByDifficultyAsync(difficulty);

        // Create game session
        var session = new GameSession
        {
            PlayerName = playerName,
            Difficulty = difficulty,
            StartTime = DateTime.UtcNow,
            TotalQuestions = questionCount
        };

        _context.GameSessions.Add(session);
        await _context.SaveChangesAsync();

        var questionDtos = selectedQuestions.Select(q => new QuestionDto(
            q.Id,
            q.Statement,
            q.CorrectFallacyTypeId
        )).ToList();

        var fallacyDtos = fallacyTypes.Select(f => new FallacyTypeDto(
            f.Id,
            f.Name,
            f.Description,
            f.Difficulty
        )).ToList();

        return new GameStateDto(
            session.Id,
            session.PlayerName,
            session.Difficulty,
            questionDtos,
            fallacyDtos,
            session.StartTime
        );
    }

    public async Task<SubmitAnswerResponse> SubmitAnswerAsync(int sessionId, int questionId, int selectedFallacyTypeId)
    {
        var session = await _context.GameSessions
            .Include(s => s.Answers)
            .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (session == null)
        {
            throw new InvalidOperationException("Game session not found");
        }

        var question = await _context.Questions
            .Include(q => q.CorrectFallacyType)
            .FirstOrDefaultAsync(q => q.Id == questionId);

        if (question == null)
        {
            throw new InvalidOperationException("Question not found");
        }

        var isCorrect = question.CorrectFallacyTypeId == selectedFallacyTypeId;

        var answer = new GameAnswer
        {
            GameSessionId = sessionId,
            QuestionId = questionId,
            SelectedFallacyTypeId = selectedFallacyTypeId,
            IsCorrect = isCorrect,
            AnsweredAt = DateTime.UtcNow
        };

        _context.GameAnswers.Add(answer);

        if (isCorrect)
        {
            session.CorrectAnswers++;
        }

        await _context.SaveChangesAsync();

        return new SubmitAnswerResponse(isCorrect, question.CorrectFallacyType!.Name);
    }

    public async Task<GameResultDto> GetGameResultAsync(int sessionId)
    {
        var session = await _context.GameSessions
            .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (session == null)
        {
            throw new InvalidOperationException("Game session not found");
        }

        // If game is not finished yet, finish it now
        if (session.EndTime == null)
        {
            session.EndTime = DateTime.UtcNow;

            // Calculate score: base points + time bonus
            var timeInSeconds = (int)(session.EndTime.Value - session.StartTime).TotalSeconds;
            var basePoints = session.CorrectAnswers * 100;

            // Time bonus: faster completion = more points
            // Max time bonus: 500 points (if answered in under 30 seconds)
            var timeBonus = Math.Max(0, 500 - (timeInSeconds / 2));

            // Difficulty multiplier
            var difficultyMultiplier = session.Difficulty switch
            {
                1 => 1.0,
                2 => 1.5,
                3 => 2.0,
                _ => 1.0
            };

            session.Score = (int)((basePoints + timeBonus) * difficultyMultiplier);

            await _context.SaveChangesAsync();
        }

        var totalTimeInSeconds = (int)(session.EndTime!.Value - session.StartTime).TotalSeconds;

        return new GameResultDto(
            session.Id,
            session.PlayerName,
            session.Difficulty,
            session.CorrectAnswers,
            session.TotalQuestions,
            session.Score,
            totalTimeInSeconds,
            session.StartTime,
            session.EndTime.Value
        );
    }

    private async Task<List<FallacyType>> GetFallacyTypesByDifficultyAsync(int difficulty)
    {
        // Easy: 3 options (only difficulty 1)
        // Medium: 5 options (difficulty 1 and 2)
        // Hard: 8 options (all difficulties)
        var maxDifficulty = difficulty;
        var count = difficulty switch
        {
            1 => 3,
            2 => 5,
            3 => 8,
            _ => 3
        };

        var fallacyTypes = await _context.FallacyTypes
            .Where(f => f.Difficulty <= maxDifficulty)
            .OrderBy(f => f.Difficulty)
            .ToListAsync();

        // If we don't have enough fallacies at the requested difficulty level,
        // take what we have
        if (fallacyTypes.Count < count)
        {
            return fallacyTypes;
        }

        // Return the requested number of fallacy types
        return fallacyTypes.Take(count).ToList();
    }
}
