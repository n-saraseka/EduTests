using EduTests.Database.Entities;
using EduTests.Database.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EduTests.Services.Questions.AnswerVerifiers;

public class SequenceVerifier : IVerifier
{
    /// <summary>
    /// Verify a <see cref="QuestionType.Sequence"/> type of answer
    /// </summary>
    /// <param name="answerData">The <see cref="QuestionData"/> object containing answer data</param>
    /// <param name="correctData">The <see cref="QuestionData"/> object containing correct data</param>
    /// <returns>True if the answer is correct, false otherwise</returns>
    public bool Verify(QuestionData answerData, QuestionData correctData)
    {
        return answerData.Sequence.SequenceEqual(correctData.Sequence);
    }
}