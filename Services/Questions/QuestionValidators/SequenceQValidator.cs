using EduTests.Database.Entities;
using EduTests.Database.Enums;

namespace EduTests.Services.Questions.QuestionValidators;

public class SequenceQValidator : IQuestionValidator
{
    /// <summary>
    /// Check if a <see cref="QuestionType.Sequence"/> type of <see cref="QuestionData"/> had been filled out correctly
    /// </summary>
    /// <param name="data">The <see cref="QuestionData"/> to check</param>
    /// <exception cref="ArgumentException">If the data hadn't been filled out correctly</exception>
    public void CheckFields(QuestionData data)
    {
        if (data.Pairs.Count != 0
            || data.LeftColumn.Count != 0
            || data.RightColumn.Count != 0
            || data.ValidAnswers.Count != 0
            || data.TextAnswer != null
            || data.NumberAnswer != null
            || data.ValidIndices != null
            || data.ChosenIndices != null)
            throw new ArgumentException(
                $"{nameof(data)} should only have the sequence filled out");
    }

    /// <summary>
    /// Validate a <see cref="QuestionType.Sequence"/> type of question / answer
    /// </summary>
    /// <param name="data">The question <see cref="QuestionData"/></param>
    /// <param name="correctData">The correct <see cref="QuestionData"/></param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="data"/>.Options has less than two items</exception>
    /// <exception cref="ArgumentException">If <paramref name="correctData"/>.Sequence hasn't passed the validation</exception>
    public void Validate(QuestionData data, QuestionData correctData)
    {
        CheckFields(data);
        CheckFields(correctData);
        if (data.Options.Count <= 1)
            throw new ArgumentException($"{nameof(data.Options)} must have at least two items");
        if (correctData.Sequence.Count != data.Options.Count ||
            correctData.Sequence.Any(s => !data.Options.Contains(s)))
            throw new ArgumentException($"{nameof(correctData.Sequence)} items must match {nameof(data.Options)}");
    }
}