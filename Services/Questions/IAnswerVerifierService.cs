using EduTests.Database.Entities;
using EduTests.Database.Enums;

namespace EduTests.Services.Questions;

public interface IAnswerVerifierService
{
    void Verify(UserAnswer answer, Question question, QuestionType type);
}