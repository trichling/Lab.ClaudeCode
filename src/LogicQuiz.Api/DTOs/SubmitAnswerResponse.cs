namespace LogicQuiz.Api.DTOs;

public record SubmitAnswerResponse(bool IsCorrect, string CorrectFallacyName);
