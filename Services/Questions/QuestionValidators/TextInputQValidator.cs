using EduTests.Database.Entities;
using EduTests.Database.Enums;

namespace EduTests.Services.Questions.QuestionValidators;

public class TextInputQValidator : IQuestionValidator
{
    /// <summary>
    /// Check if a <see cref="QuestionType.TextInput"/> type of <see cref="QuestionData"/> had been filled out correctly
    /// </summary>
    /// <param name="data">The <see cref="QuestionData"/> to check</param>
    /// <exception cref="ArgumentException">If the data hadn't been filled out correctly</exception>
    public void CheckFields(QuestionData data)
    {
        if (data.Pairs.Count != 0
            || data.LeftColumn.Count != 0
            || data.RightColumn.Count != 0
            || data.Options.Count != 0
            || data.Sequence.Count != 0
            || data.NumberAnswer != null)
            throw new ArgumentException($"{nameof(data)} should only have the text answer or valid answers filled out");
    }

    /// <summary>
    /// Validate a <see cref="QuestionType.TextInput"/> type of question / answer
    /// </summary>
    /// <param name="data">The question <see cref="QuestionData"/></param>
    /// <param name="correctData">The correct <see cref="QuestionData"/></param>
    /// <exception cref="ArgumentException">If the <paramref name="data"/> or <paramref name="correctData"/> haven't passed the validation</exception>
    public void Validate(QuestionData data, QuestionData correctData)
    {
        CheckFields(data);
        CheckFields(correctData);
        if (data.TextAnswer is not null)
            throw new ArgumentException($"{nameof(data.TextAnswer)} should be null");
        if (correctData.TextAnswer is not null)
            throw new ArgumentException($"{nameof(correctData.TextAnswer)} should be null");
        if (correctData.ValidAnswers.Count == 0)
            throw new ArgumentException($"{nameof(correctData.ValidAnswers)} should have at least one item");
    }
}