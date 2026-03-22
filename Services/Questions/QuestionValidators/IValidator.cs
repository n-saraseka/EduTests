using EduTests.Database.Entities;

namespace EduTests.Services.Questions.QuestionValidators;

public interface IValidator
{
    void CheckFields(QuestionData data);
    void Validate(QuestionData questionData, QuestionData? correctData, bool isQuestion);
}