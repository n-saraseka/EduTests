namespace EduTests.Database.Entities;

// JSON field for Question and UserAnswer entities
public class QuestionData
{
    // multiple or single choice question, also base for sequence
    public List<string> Options { get; set; } = new();
    // Correct data for multiple / single choice
    public List<int> ValidIndices { get; set; } = new();
    // ONLY for Answers, not Question.Data or Question.CorrectData
    public List<int> ChosenIndices { get; set; } = new();
    
    // match pairs
    public List<string> LeftColumn { get; set; } = new();
    public List<string> RightColumn { get; set; } = new();
    public List<MatchingPair> Pairs { get; set; } = new();
    
    // number input
    public double? Tolerance;
    public double? NumberAnswer;
    
    // sequence
    public List<string> Sequence { get; set; } = new();
    
    // text input
    public string? TextAnswer;
    public List<string> ValidAnswers = new();
}