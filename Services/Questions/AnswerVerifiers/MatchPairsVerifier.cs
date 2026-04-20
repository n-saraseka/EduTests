using EduTests.Database.Entities;
using EduTests.Database.Enums;

namespace EduTests.Services.Questions.AnswerVerifiers;

public class MatchPairsVerifier : IVerifier
{
    /// <summary>
    /// Check if a <see cref="QuestionType.MatchPairs"/> type of <see cref="QuestionData"/> had been filled out correctly
    /// </summary>
    /// <param name="data">The <see cref="QuestionData"/> to check</param>
    /// <exception cref="ArgumentException">If the data hadn't been filled out correctly</exception>
    public void CheckFields(QuestionData data)
    {
        if (data.Options.Count != 0
            || data.ValidAnswers.Count != 0
            || data.Sequence.Count != 0
            || data.TextAnswer != null
            || data.NumberAnswer != null
            || data.ValidIndices != null
            || data.ChosenIndices != null)
            throw new ArgumentException(
                $"{nameof(data)} should only have pairs filled out");
    }

    /// <summary>
    /// Verify a <see cref="QuestionType.MatchPairs"/> type of answer
    /// </summary>
    /// <param name="answerData">The <see cref="QuestionData"/> object containing answer data</param>
    /// <param name="correctData">The <see cref="QuestionData"/> object containing correct data</param>
    /// <returns>True if the answer is correct, false otherwise</returns>
    public bool Verify(QuestionData answerData, QuestionData correctData)
    {
        return answerData.Pairs.All(pair => correctData.Pairs.Any(pair.Equals));
    }
}