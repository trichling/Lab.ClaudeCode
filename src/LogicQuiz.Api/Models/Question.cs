namespace LogicQuiz.Api.Models;

public class Question
{
    public int Id { get; set; }
    public required string Statement { get; set; }
    public int CorrectFallacyTypeId { get; set; }

    public FallacyType? CorrectFallacyType { get; set; }
}
