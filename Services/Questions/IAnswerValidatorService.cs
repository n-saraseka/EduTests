using EduTests.Database.Entities;
using EduTests.Database.Enums;

namespace EduTests.Services.Questions;

public interface IAnswerValidatorService
{
    void Validate(QuestionData answerData, QuestionData questionData, QuestionType type);
}