using EduTests.Database.Entities;

namespace EduTests.Services.Questions.AnswerVerifiers;

public interface IVerifier
{
    bool Verify(QuestionData answerData, QuestionData correctData);
}