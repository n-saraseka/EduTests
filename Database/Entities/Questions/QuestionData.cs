using EduTests.Database.Enums;

namespace EduTests.Database.Entities.Questions;

public abstract class QuestionData
{
    public abstract void ValidateQuestion(QuestionData? correctData, QuestionType expectedType);

    public abstract void ValidateAnswer();
}