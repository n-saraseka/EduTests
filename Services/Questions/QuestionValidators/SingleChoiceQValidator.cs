using EduTests.Database.Entities;
using EduTests.Database.Enums;

namespace EduTests.Services.Questions.QuestionValidators;

public class SingleChoiceQValidator : IQuestionValidator
{
    /// <summary>
    /// Check if a <see cref="QuestionType.SingleChoice"/> type of <see cref="QuestionData"/> had been filled out correctly
    /// </summary>
    /// <param name="data">The <see cref="QuestionData"/> to check</param>
    /// <exception cref="ArgumentException">If the data hadn't been filled out correctly</exception>
    public void CheckFields(QuestionData data)
    {
        if (data.Pairs.Count != 0
            || data.LeftColumn.Count != 0
            || data.RightColumn.Count != 0
            || data.ValidAnswers.Count != 0
            || data.Sequence.Count != 0
            || data.TextAnswer != null
            || data.NumberAnswer != null)
            throw new ArgumentException(
                $"{nameof(data)} should only have single or multiple choice related fields filled out");
    }

    /// <summary>
    /// Validate a <see cref="QuestionType.SingleChoice"/> type of question / answer
    /// </summary>
    /// <param name="data">The question <see cref="QuestionData"/></param>
    /// <param name="correctData">The correct <see cref="QuestionData"/></param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="data"/>.Options or <paramref name="correctData"/>.ValidIndices are out of range</exception>
    /// <exception cref="ArgumentException">If <paramref name="correctData"/>.ValidIndices doesn't have exactly one item</exception>
    public void Validate(QuestionData data, QuestionData correctData)
    {
        CheckFields(data);
        CheckFields(correctData);
        if (data.Options.Count is < 2 or > 10)
            throw new ArgumentOutOfRangeException($"{nameof(data.Options)} must have between 2 and 10 items");
        if (correctData.ValidIndices.Min() < 0 || correctData.ValidIndices.Max() > data.Options.Count - 1)
            throw new ArgumentOutOfRangeException($"{nameof(correctData.ValidIndices)}");
        if (correctData.ValidIndices.Count != 1)
            throw new ArgumentException($"{nameof(correctData.ValidIndices)} should have exactly one item");
    }
}