using EduTests.Database.Enums;

namespace EduTests.Database.Entities.Questions;

public class ChoiceQuestion : QuestionData
{
    public List<string> Options { get; set; } = new();

    public override void ValidateQuestion(QuestionData? correctData, QuestionType expectedType)
    {
        if (correctData is not ChoiceQuestion correct)
            throw new ArgumentException($"{nameof(correctData)} must be of type {nameof(ChoiceQuestion)}");
        if (Options.Count is 0 or > 10)
            throw new ArgumentOutOfRangeException($"{nameof(Options)} must have between 1 and 10 items");
        if (!correct.Options.All(o => Options.Contains(o)))
            throw new ArgumentException("All correct options must be present from the available options");
        if (expectedType == QuestionType.SingleChoice && correct.Options.Count != 1)
            throw new ArgumentOutOfRangeException($"{nameof(correct.Options)} must have exactly one option");
    }

    public override void ValidateAnswer()
    {
        if (Options.Count is 0 or > 10)
            throw new ArgumentOutOfRangeException($"{nameof(Options)} must have between 1 and 10 items");
    }
}