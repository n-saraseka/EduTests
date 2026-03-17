using EduTests.Database.Enums;

namespace EduTests.Database.Entities.Questions;

public class SequenceQuestion : QuestionData
{
    public List<string> Sequence { get; set; } = new();

    public override void ValidateQuestion(QuestionData correctData, QuestionType expectedType)
    {
        base.ValidateQuestion(correctData, expectedType);
        
        if (correctData is not SequenceQuestion correct)
            throw new ArgumentException($"{nameof(correctData)} must be of type {nameof(SequenceQuestion)}");
        if (Sequence.Count == 0 || correct.Sequence.Count == 0)
            throw new ArgumentException($"{nameof(Sequence)} must contain at least one item");
        if (correct.Sequence.Count != Sequence.Count || correct.Sequence.Any(s => !Sequence.Contains(s)))
            throw new ArgumentException($"{nameof(correct.Sequence)} items must match {nameof(SequenceQuestion)}");
    }

    public override void ValidateAnswer(QuestionData correctData, QuestionType expectedType)
    {
        base.ValidateAnswer(correctData, expectedType);
        if (Sequence.Count == 0)
            throw new ArgumentException($"{nameof(Sequence)} must contain at least one item");
    }
}