using EduTests.Database.Entities;
using EduTests.Database.Enums;

namespace EduTests.Services.Questions.AnswerVerifiers;

public class NumberInputVerifier : IVerifier
{
    /// <summary>
    /// Verify a <see cref="QuestionType.NumberInput"/> type of answer
    /// </summary>
    /// <param name="answerData">The <see cref="QuestionData"/> object containing answer data</param>
    /// <param name="correctData">The <see cref="QuestionData"/> object containing correct data</param>
    /// <returns>True if the answer is correct, false otherwise</returns>
    /// <exception cref="ArgumentNullException">If either <paramref name="answerData"/> or <paramref name="correctData"/>
    /// don't have the <see cref="QuestionData.NumberAnswer"/> filled out</exception>
    public bool Verify(QuestionData answerData, QuestionData correctData)
    {
        return answerData.NumberAnswer.Equals(correctData.NumberAnswer);
    }
}