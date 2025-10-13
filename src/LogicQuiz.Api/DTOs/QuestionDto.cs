namespace LogicQuiz.Api.DTOs;

public record QuestionDto(int Id, string Statement, int CorrectFallacyTypeId);
