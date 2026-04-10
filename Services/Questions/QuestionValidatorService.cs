using EduTests.Database.Entities;
using EduTests.Database.Enums;
using EduTests.Services.Questions.QuestionValidators;

namespace EduTests.Services.Questions;

public class QuestionValidatorService : IQuestionValidatorService
{
    private static readonly Dictionary<QuestionType, IQuestionValidator> Validators = new()
    {
        [QuestionType.SingleChoice] = new SingleChoiceQValidator(),
        [QuestionType.MultipleChoice] = new MultipleChoiceQValidator(),
        [QuestionType.NumberInput] = new NumberInputQValidator(),
        [QuestionType.TextInput] = new TextInputQValidator(),
        [QuestionType.Sequence] = new SequenceQValidator(),
        [QuestionType.MatchPairs] = new MatchPairsQValidator()
    };
    
    /// <summary>
    /// Validate question data
    /// </summary>
    /// <param name="questionData">The question data</param>
    /// <param name="correctData">The correct answers data</param>
    /// <param name="type">The <see cref="QuestionType"/></param>
    /// <exception cref="ArgumentException">If the <see cref="QuestionType"/> is not valid</exception>
    public void Validate(QuestionData questionData, QuestionData correctData, QuestionType type)
    {
        if (Validators.TryGetValue(type, out var validator))
            validator.Validate(questionData, correctData);
        else
            throw new ArgumentException($"The type {type} is not valid.");
    }
}