using EduTests.Database.Entities;
using EduTests.Database.Enums;

namespace EduTests.Services.Questions;

public interface IAnswerVerifierService
{
    bool Verify(UserAnswer answer, Question question, QuestionType type);
}