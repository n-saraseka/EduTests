using EduTests.Database.Enums;

namespace EduTests.Database.Entities.Questions;

public class TextQuestion : QuestionData
{
    public string? Answer;
    public List<string> ValidAnswers = new();

    public override void ValidateQuestion(QuestionData correctData, QuestionType expectedType)
    {
        base.ValidateQuestion(correctData, expectedType);
        
        if (correctData is not TextQuestion correct)
            throw new ArgumentException($"{nameof(correctData)} must be of type {nameof(TextQuestion)}");
        if (Answer is not null )
            throw new ArgumentException($"{nameof(Answer)} must be null");
        if (ValidAnswers.Count != 0)
            throw new ArgumentException($"{nameof(ValidAnswers)} must contain no items");
        if (correct.ValidAnswers.Count == 0)
            throw new ArgumentException($"{nameof(correct.ValidAnswers)} must contain at least one item");
    }

    public override void ValidateAnswer(QuestionData correctData, QuestionType expectedType)
    {
        base.ValidateAnswer(correctData, expectedType);
        if (Answer is null)
            throw new ArgumentException($"{nameof(Answer)} must not be null");
    }
}