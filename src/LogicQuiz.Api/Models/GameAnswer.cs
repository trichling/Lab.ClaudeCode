namespace LogicQuiz.Api.Models;

public class GameAnswer
{
    public int Id { get; set; }
    public int GameSessionId { get; set; }
    public int QuestionId { get; set; }
    public int SelectedFallacyTypeId { get; set; }
    public bool IsCorrect { get; set; }
    public DateTime AnsweredAt { get; set; }

    public GameSession? GameSession { get; set; }
    public Question? Question { get; set; }
    public FallacyType? SelectedFallacyType { get; set; }
}
