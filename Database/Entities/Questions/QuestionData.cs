using EduTests.Database.Enums;

namespace EduTests.Database.Entities.Questions;

public abstract class QuestionData
{
    public virtual void ValidateQuestion(QuestionData correctData, QuestionType expectedType)
    {
        if (correctData is null)
            throw new ArgumentNullException(nameof(correctData));
    }

    public virtual void ValidateAnswer(QuestionData correctData, QuestionType expectedType)
    {
        if (correctData is not null)
            throw new ArgumentNullException($"{nameof(correctData)} is not null");
    }
}