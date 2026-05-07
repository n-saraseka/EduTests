using EduTests.Database.Entities;
using EduTests.Database.Enums;

namespace EduTests.Services.Questions.AnswerVerifiers;

public class MultipleChoiceVerifier : IVerifier
{
    /// <summary>
    /// Verify a <see cref="QuestionType.MultipleChoice"/> type of answer
    /// </summary>
    /// <param name="answerData">The <see cref="QuestionData"/> object containing answer data</param>
    /// <param name="correctData">The <see cref="QuestionData"/> object containing correct data</param>
    /// <returns>True if the answer is correct, false otherwise</returns>
    public bool Verify(QuestionData answerData, QuestionData correctData)
    {
        return answerData.ChosenIndices.Count == correctData.ValidIndices.Count && answerData.ChosenIndices.All(o => correctData.ValidIndices.Contains(o));
    }
}