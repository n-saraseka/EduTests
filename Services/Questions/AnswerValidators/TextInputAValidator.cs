using EduTests.Database.Entities;
using EduTests.Database.Enums;

namespace EduTests.Services.Questions.AnswerValidators;

public class TextInputAValidator : IAnswerValidator
{
    /// <summary>
    /// Check if a <see cref="QuestionType.TextInput"/> type of <see cref="QuestionData"/> had been filled out correctly
    /// </summary>
    /// <param name="data">The <see cref="QuestionData"/> to check</param>
    /// <exception cref="ArgumentException">If the data hadn't been filled out correctly</exception>
    public void CheckFields(QuestionData data)
    {
        if (data.Options.Count != 0
            || data.ValidAnswers.Count != 0
            || data.Sequence.Count != 0
            || data.Pairs.Count != 0
            || data.ValidIndices.Count != 0
            || data.ChosenIndices.Count != 0
            || data.NumberAnswer != null)
            throw new ArgumentException(
                $"{nameof(data)} should only have the text filled out");
    }

    /// <summary>
    /// Validate a <see cref="QuestionType.TextInput"/> type of answer
    /// </summary>
    /// <param name="answerData">The answer <see cref="QuestionData"/></param>
    /// <param name="questionData">The question <see cref="QuestionData"/></param>
    /// <exception cref="ArgumentNullException">If <paramref name="answerData"/>.TextAnswer is null</exception>
    public void Validate(QuestionData answerData, QuestionData questionData)
    {
        CheckFields(answerData);
        CheckFields(questionData);
        if (answerData.TextAnswer is null)
            throw new ArgumentNullException($"{nameof(answerData.TextAnswer)} must not be null");
    }
}