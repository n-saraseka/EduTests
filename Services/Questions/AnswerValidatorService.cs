using EduTests.Database.Entities;
using EduTests.Database.Enums;
using EduTests.Services.Questions.AnswerValidators;
using EduTests.Services.Questions.QuestionValidators;

namespace EduTests.Services.Questions;

public class AnswerValidatorService : IAnswerValidatorService
{
    private static readonly Dictionary<QuestionType, IAnswerValidator> Validators = new()
    {
        [QuestionType.SingleChoice] = new SingleChoiceAValidator(),
        [QuestionType.MultipleChoice] = new MultipleChoiceAValidator(),
        [QuestionType.NumberInput] = new NumberInputAValidator(),
        [QuestionType.TextInput] = new TextInputAValidator(),
        [QuestionType.Sequence] = new SequenceAValidator(),
        [QuestionType.MatchPairs] = new MatchPairsAValidator()
    };
    
    /// <summary>
    /// Validate answer data
    /// </summary>
    /// <param name="answerData">The answer <see cref="QuestionData"/></param>
    /// <param name="questionData">The question <see cref="QuestionData"/></param>
    /// <param name="type">The <see cref="QuestionType"/></param>
    /// <exception cref="ArgumentException">If the <see cref="QuestionType"/> is not valid</exception>
    public void Validate(QuestionData answerData, QuestionData questionData, QuestionType type)
    {
        if (Validators.TryGetValue(type, out var validator))
            validator.Validate(answerData, questionData);
        else
            throw new ArgumentException($"The type {type} is not valid.");
    }
}