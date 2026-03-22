using EduTests.Database.Entities;
using EduTests.Database.Enums;

namespace EduTests.Services.Questions;

public interface IQuestionValidatorService
{
    void Validate(QuestionData questionData, QuestionData? correctData, QuestionType type, bool isQuestion);
}