using EduTests.Database.Entities;

namespace EduTests.Services.Questions.AnswerVerifiers;

public interface IVerifier
{
    void CheckFields(QuestionData data);
    bool Verify(QuestionData answerData, QuestionData correctData);
}