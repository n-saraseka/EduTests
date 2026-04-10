using EduTests.Database.Entities;
using EduTests.Database.Enums;

namespace EduTests.Services.Questions.AnswerValidators;

public class MatchPairsAValidator : IAnswerValidator
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
            || data.ValidIndices.Count != 0
            || data.ChosenIndices.Count != 0
            || data.TextAnswer != null
            || data.NumberAnswer != null)
            throw new ArgumentException(
                $"{nameof(data)} should only have pairs filled out");
    }

    /// <summary>
    /// Validate a <see cref="QuestionType.MatchPairs"/> type of answer
    /// </summary>
    /// <param name="answerData">The answer <see cref="QuestionData"/></param>
    /// <param name="questionData">The question <see cref="QuestionData"/></param>
    /// <exception cref="ArgumentException">If <paramref name="answerData"/>.Pairs haven't passed the validation</exception>
    public void Validate(QuestionData answerData, QuestionData questionData)
    {
        CheckFields(answerData);
        CheckFields(questionData);
        if (answerData.Pairs.Count != questionData.LeftColumn.Count)
            throw new ArgumentException($"{nameof(answerData.Pairs.Count)} should match the question column count");

        var left = questionData.LeftColumn;
        var right = questionData.RightColumn;
        var answerLeft = answerData.Pairs.Select(p => p.Left);
        var answerRight = answerData.Pairs.Select(p => p.Right);

        if (answerLeft.Any(s => !left.Contains(s)) || answerRight.Any(s => !right.Contains(s)))
            throw new ArgumentException($"Each column in {nameof(answerData.Pairs)} should have the same items as" +
                                        $"{nameof(questionData.LeftColumn)} and {nameof(questionData.RightColumn)}");
    }
}