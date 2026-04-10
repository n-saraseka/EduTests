using EduTests.Database.Entities;
using EduTests.Database.Enums;

namespace EduTests.Services.Questions.AnswerValidators;

public class SequenceAValidator: IAnswerValidator
{
    /// <summary>
    /// Check if a <see cref="QuestionType.Sequence"/> type of <see cref="QuestionData"/> had been filled out correctly
    /// </summary>
    /// <param name="data">The <see cref="QuestionData"/> to check</param>
    /// <exception cref="ArgumentException">If the data hadn't been filled out correctly</exception>
    public void CheckFields(QuestionData data)
    {
        if (data.ValidAnswers.Count != 0
            || data.Pairs.Count != 0
            || data.LeftColumn.Count != 0
            || data.RightColumn.Count != 0
            || data.ValidIndices.Count != 0
            || data.ChosenIndices.Count != 0
            || data.TextAnswer != null
            || data.NumberAnswer != null)
            throw new ArgumentException(
                $"{nameof(data)} should only have sequence filled out");
    }

    /// <summary>
    /// Validate a <see cref="QuestionType.Sequence"/> type of answer
    /// </summary>
    /// <param name="answerData">The answer <see cref="QuestionData"/></param>
    /// <param name="questionData">The question <see cref="QuestionData"/></param>
    /// <exception cref="ArgumentException">If <paramref name="answerData"/>.Pairs haven't passed the validation</exception>
    public void Validate(QuestionData answerData, QuestionData questionData)
    {
        CheckFields(answerData);
        CheckFields(questionData);
        if (answerData.Sequence.Count != questionData.Options.Count ||
            answerData.Sequence.Any(s => !questionData.Options.Contains(s)))
            throw new ArgumentException($"{nameof(answerData.Sequence)} items must match {nameof(questionData.Options)}");
    }
}