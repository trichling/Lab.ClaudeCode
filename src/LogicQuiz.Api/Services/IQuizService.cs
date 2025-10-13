using LogicQuiz.Api.DTOs;

namespace LogicQuiz.Api.Services;

public interface IQuizService
{
    Task<GameStateDto> StartGameAsync(string playerName, int difficulty);
    Task<SubmitAnswerResponse> SubmitAnswerAsync(int sessionId, int questionId, int selectedFallacyTypeId);
    Task<GameResultDto> GetGameResultAsync(int sessionId);
}
