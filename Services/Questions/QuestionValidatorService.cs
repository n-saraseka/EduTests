using EduTests.Database.Entities;
using EduTests.Database.Enums;
using EduTests.Services.Questions.QuestionValidators;

namespace EduTests.Services.Questions;

public class QuestionValidatorService : IQuestionValidatorService
{
    private static readonly Dictionary<QuestionType, IValidator> Validators = new()
    {
        [QuestionType.SingleChoice] = new SingleChoiceValidator(),
        [QuestionType.MultipleChoice] = new MultipleChoiceValidator(),
        [QuestionType.NumberInput] = new NumberInputValidator(),
        [QuestionType.TextInput] = new TextInputValidator(),
        [QuestionType.Sequence] = new SequenceValidator(),
        [QuestionType.MatchPairs] = new MatchPairsValidator()
    };
    
    /// <summary>
    /// Validate question data (and optionally answer data)
    /// </summary>
    /// <param name="questionData">The question / answer data</param>
    /// <param name="correctData">The valid answers data</param>
    /// <param name="type">The <see cref="QuestionType"/></param>
    /// <param name="isQuestion">Whether the <see cref="QuestionData"/> to validate comes from a <see cref="Question"/> or <see cref="UserAnswer"/></param>
    /// <exception cref="ArgumentException">If the <see cref="QuestionType"/> is not valid</exception>
    public void Validate(QuestionData questionData, QuestionData? correctData, QuestionType type, bool isQuestion)
    {
        if (Validators.TryGetValue(type, out var validator))
            validator.Validate(questionData, correctData, isQuestion);
        else
            throw new ArgumentException($"The type {type} is not valid.");
    }
}