namespace LogicQuiz.Api.Models;

public class GameSession
{
    public int Id { get; set; }
    public required string PlayerName { get; set; }
    public int Difficulty { get; set; } // 1=Easy (3 options), 2=Medium (5 options), 3=Hard (8 options)
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int CorrectAnswers { get; set; }
    public int TotalQuestions { get; set; }
    public int Score { get; set; }

    public ICollection<GameAnswer> Answers { get; set; } = new List<GameAnswer>();
}
