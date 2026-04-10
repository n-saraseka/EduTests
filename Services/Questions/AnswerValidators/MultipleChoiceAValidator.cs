using EduTests.Database.Entities;
using EduTests.Database.Enums;

namespace EduTests.Services.Questions.AnswerValidators;

public class MultipleChoiceAValidator : IAnswerValidator
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
    /// Validate a <see cref="QuestionType.MultipleChoice"/> type of answer
    /// </summary>
    /// <param name="answerData">The answer <see cref="QuestionData"/></param>
    /// <param name="questionData">The question <see cref="QuestionData"/></param>
    /// <exception cref="ArgumentException">If <paramref name="answerData"/>.ChosenIndices have more items than <paramref name="questionData"/>.Options</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="answerData"/>.ChosenIndices are out of range</exception>
    public void Validate(QuestionData answerData, QuestionData questionData)
    {
        CheckFields(answerData);
        CheckFields(questionData);
        if (answerData.ChosenIndices.Count > questionData.Options.Count)
            throw new ArgumentException($"{nameof(answerData.ChosenIndices.Count)} must be less or equal to {nameof(questionData.Options.Count)}");
        if (answerData.ChosenIndices.Min() < 0 || answerData.ChosenIndices.Max() > questionData.Options.Count)
            throw new ArgumentOutOfRangeException($"{nameof(answerData.ChosenIndices)}");
    }
}