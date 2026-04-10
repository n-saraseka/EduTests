using EduTests.Database.Entities;

namespace EduTests.Services.Questions.AnswerValidators;

public interface IAnswerValidator
{
    void CheckFields(QuestionData data);
    void Validate(QuestionData answerData, QuestionData questionData);
}