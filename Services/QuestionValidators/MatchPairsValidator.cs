using EduTests.Database.Entities;
using EduTests.Database.Enums;

namespace EduTests.Services.QuestionValidators;

public class MatchPairsValidator : IValidator
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
            || data.NumberAnswer != null)
            throw new ArgumentException(
                $"{nameof(data)} should only have pairs filled out");
    }

    /// <summary>
    /// Validate a <see cref="QuestionType.MatchPairs"/> type of question / answer
    /// </summary>
    /// <param name="data">The question / answer data</param>
    /// <param name="correctData">The valid answers data</param>
    /// <param name="isQuestion">Whether the <see cref="QuestionData"/> to validate comes from a <see cref="Question"/> or <see cref="UserAnswer"/></param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="data"/>.Pairs have less than two pairs</exception>
    /// <exception cref="ArgumentException">If <paramref name="correctData"/>.Pairs haven't passed the validation</exception>
    public void Validate(QuestionData data, QuestionData? correctData, bool isQuestion)
    {
        CheckFields(data);
        if (isQuestion)
        {
            CheckFields(correctData);
            var left = data.LeftColumn;
            var right = data.RightColumn;
            if (data.Pairs.Count > 0)
                throw new ArgumentException($"{nameof(data.Pairs)} should not have any items");
            var correctLeft = correctData.Pairs.Select(p => p.Left);
            var correctRight = correctData.Pairs.Select(p => p.Right);
            if (left.Count != right.Count)
                throw new ArgumentException($"{nameof(left.Count)} and {nameof(right.Count)} should have the same amount of items");
            if (correctLeft.Any(s => !left.Contains(s)) || correctRight.Any(s => !right.Contains(s)))
                throw new ArgumentException($"Each column in {nameof(correctData.Pairs)} should have the same items as" +
                                            $"{nameof(data.LeftColumn)} and {nameof(data.RightColumn)}");
        }
        else
        {
            if (data.Pairs.Count <= 1)
                throw new ArgumentOutOfRangeException($"{nameof(data.Pairs)} must have at least two pairs");
        }
    }
}