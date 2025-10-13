namespace LogicQuiz.Api.DTOs;

public record GameStateDto(
    int SessionId,
    string PlayerName,
    int Difficulty,
    List<QuestionDto> Questions,
    List<FallacyTypeDto> AvailableFallacies,
    DateTime StartTime
);
