using EduTests.Database.Entities;
using EduTests.Database.Enums;

namespace EduTests.Services.Questions.QuestionValidators;

public class MatchPairsQValidator : IQuestionValidator
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
    /// Validate a <see cref="QuestionType.MatchPairs"/> type of question / answer
    /// </summary>
    /// <param name="data">The question <see cref="QuestionData"/></param>
    /// <param name="correctData">The correct <see cref="QuestionData"/></param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="data"/>.Pairs have less than two pairs</exception>
    /// <exception cref="ArgumentException">If <paramref name="correctData"/>.Pairs or <see cref="data"/>.Pairs haven't passed the validation</exception>
    public void Validate(QuestionData data, QuestionData correctData)
    {
        CheckFields(data);
        CheckFields(correctData);
        var left = data.LeftColumn;
        var right = data.RightColumn;
        if (data.Pairs.Count > 0)
            throw new ArgumentException($"{nameof(data.Pairs)} should not have any items");
        var correctLeft = correctData.Pairs.Select(p => p.Left);
        var correctRight = correctData.Pairs.Select(p => p.Right);
        if (left.Count != right.Count)
            throw new ArgumentException($"{nameof(left.Count)} and {nameof(right.Count)} should have the same amount of items");
        if (correctData.Pairs.Count != left.Count)
            throw new ArgumentException($"{nameof(correctData.Pairs)} and column items count should match");
        if (correctLeft.Any(s => !left.Contains(s)) || correctRight.Any(s => !right.Contains(s)))
            throw new ArgumentException($"Each column in {nameof(correctData.Pairs)} should have the same items as" +
                                        $"{nameof(data.LeftColumn)} and {nameof(data.RightColumn)}");
    }
}