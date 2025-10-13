namespace LogicQuiz.Api.DTOs;

public record GameResultDto(
    int SessionId,
    string PlayerName,
    int Difficulty,
    int CorrectAnswers,
    int TotalQuestions,
    int Score,
    int TimeInSeconds,
    DateTime StartTime,
    DateTime EndTime
);
