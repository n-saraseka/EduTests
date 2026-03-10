namespace EduTests.Database.Entities;

// JSONB field for Question and UserAnswer entities
public class QuestionData
{
    // multiple and single choice, options is also base for sequence
    public List<String> Options { get; set; } = new();
    
    // text input and number input
    public string? AnswerString { get; set; }
    public float? AnswerNumber { get; set; }

    // match pairs
    public List<MatchingPair> Pairs { get; set; } = new();
    
    // sequence
    public List<String> Sequence { get; set; } = new();
}