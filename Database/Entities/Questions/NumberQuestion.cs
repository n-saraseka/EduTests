using EduTests.Database.Enums;

namespace EduTests.Database.Entities.Questions;

public class NumberQuestion : QuestionData
{
    public double Tolerance = 0.1;
    public double? Answer;

    public override void ValidateQuestion(QuestionData correctData, QuestionType expectedType)
    {
        base.ValidateQuestion(correctData, expectedType);
        
        if (correctData is not NumberQuestion correct)
            throw new ArgumentException($"{nameof(correctData)} must be of type {nameof(NumberQuestion)}");
        if (Answer != null)
            throw new ArgumentException($"{nameof(Answer)} must be null");
        if (correct.Answer == null)
            throw new ArgumentException($"{nameof(correct.Answer)} must not be null");
    }

    public override void ValidateAnswer(QuestionData correctData, QuestionType expectedType)
    {
        base.ValidateAnswer(correctData, expectedType);
        if (Answer == null)
            throw new ArgumentException($"{nameof(Answer)} must not be null");
    }
}