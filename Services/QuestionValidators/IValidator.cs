using EduTests.Database.Entities;
using EduTests.Database.Enums;

namespace EduTests.Services.QuestionValidators;

public interface IValidator
{
    void CheckFields(QuestionData data);
    void Validate(QuestionData questionData, QuestionData? correctData, bool isQuestion);
}