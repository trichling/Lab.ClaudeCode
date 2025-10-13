namespace LogicQuiz.Api.DTOs;

public record SubmitAnswerRequest(int SessionId, int QuestionId, int SelectedFallacyTypeId);
