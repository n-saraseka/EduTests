using EduTests.Database.Entities;
using EduTests.Database.Enums;

namespace EduTests.Services.QuestionValidator.Validators;

public class MultipleChoiceValidator : IValidator
{
    /// <summary>
    /// Check if a <see cref="QuestionType.MultipleChoice"/> type of <see cref="QuestionData"/> had been filled out correctly
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
    /// Validate a <see cref="QuestionType.MultipleChoice"/> type of question / answer
    /// </summary>
    /// <param name="data">The question / answer data</param>
    /// <param name="correctData">The valid answers data</param>
    /// <param name="isQuestion">Whether the <see cref="QuestionData"/> to validate comes from a <see cref="Question"/> or <see cref="UserAnswer"/></param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="data"/>.Options are out of range</exception>
    /// <exception cref="ArgumentException">If <paramref name="correctData"/>.Options don't match options from <paramref name="data"/>.Options</exception>
    public void Validate(QuestionData data, QuestionData? correctData, bool isQuestion)
    {
        CheckFields(data);
        if (data.Options.Count is 0 or > 10)
            throw new ArgumentOutOfRangeException($"{nameof(data.Options)} must have between 1 and 10 items");;
        if (isQuestion)
        {
            CheckFields(correctData);
            if (!correctData.Options.All(o => data.Options.Contains(o)))
                throw new ArgumentException("All correct options must be present from the available options");
        }
    }
}