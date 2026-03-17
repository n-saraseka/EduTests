using EduTests.Database.Entities;
using EduTests.Database.Enums;

namespace EduTests.Services.QuestionValidator.Validators;

public class NumberInputValidator : IValidator
{
    /// <summary>
    /// Check if a <see cref="QuestionType.NumberInput"/> type of <see cref="QuestionData"/> had been filled out correctly
    /// </summary>
    /// <param name="data">The <see cref="QuestionData"/> to check</param>
    /// <exception cref="ArgumentException">If the data hadn't been filled out correctly</exception>
    public void CheckFields(QuestionData data)
    {
        if (data.Pairs.Count != 0
            || data.LeftColumn.Count != 0
            || data.RightColumn.Count != 0
            || data.Options.Count != 0
            || data.ValidAnswers.Count != 0
            || data.Sequence.Count != 0
            || data.TextAnswer != null)
            throw new ArgumentException(
                $"{nameof(data)} should only have the number filled out");
    }

    /// <summary>
    /// Validate a <see cref="QuestionType.NumberInput"/> type of question / answer
    /// </summary>
    /// <param name="data">The question / answer data</param>
    /// <param name="correctData">The valid answers data</param>
    /// <param name="isQuestion">Whether the <see cref="QuestionData"/> to validate comes from a <see cref="Question"/> or <see cref="UserAnswer"/></param>
    /// <exception cref="ArgumentException">If <paramref name="data"/>.NumberAnswer is not null</exception>
    /// <exception cref="ArgumentNullException">If <paramref name="correctData"/>.NumberAnswer is null when validating a <see cref="Question"/>,
    /// or if <paramref name="data"/>.NumberAnswer is null when validating a <see cref="UserAnswer"/></exception>
    public void Validate(QuestionData data, QuestionData? correctData, bool isQuestion)
    {
        CheckFields(data);
        if (isQuestion)
        {
            CheckFields(correctData);
            if (data.NumberAnswer is not null)
                throw new ArgumentException($"{nameof(data.NumberAnswer)} should be null");
            if (correctData.NumberAnswer is null)
                throw new ArgumentNullException($"{nameof(correctData.NumberAnswer)} should not be null");
        }
        else
        {
            if (data.NumberAnswer is null)
                throw new ArgumentNullException($"{nameof(data.NumberAnswer)} should not be null");
        }
    }
}