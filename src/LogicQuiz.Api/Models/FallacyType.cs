namespace LogicQuiz.Api.Models;

public class FallacyType
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public int Difficulty { get; set; } // 1=Easy, 2=Medium, 3=Hard

    public ICollection<Question> Questions { get; set; } = new List<Question>();
}
