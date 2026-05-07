using EduTests.Database.Entities;
using EduTests.Database.Enums;

namespace EduTests.Services.Questions.AnswerVerifiers;

public class SingleChoiceVerifier : IVerifier
{
    /// <summary>
    /// Verify a <see cref="QuestionType.SingleChoice"/> type of answer
    /// </summary>
    /// <param name="answerData">The <see cref="QuestionData"/> object containing answer data</param>
    /// <param name="correctData">The <see cref="QuestionData"/> object containing correct data</param>
    /// <returns>True if the answer is correct, false otherwise</returns>
    /// <exception cref="ArgumentException">If <paramref name="answerData"/> doesn't have an answer selected</exception>
    public bool Verify(QuestionData answerData, QuestionData correctData)
    {
        return answerData.ChosenIndices.All(o => correctData.ValidIndices.Contains(o));
    }
}