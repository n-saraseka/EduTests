using EduTests.Database.Entities;
using EduTests.Database.Enums;
using EduTests.Services.Questions.AnswerVerifiers;

namespace EduTests.Services.Questions;

public class AnswerVerifierService : IAnswerVerifierService
{
    private static readonly Dictionary<QuestionType, IVerifier> Validators = new()
    {
        [QuestionType.SingleChoice] = new SingleChoiceVerifier(),
        [QuestionType.MultipleChoice] = new MultipleChoiceVerifier(),
        [QuestionType.NumberInput] = new NumberInputVerifier(),
        [QuestionType.TextInput] = new TextInputVerifier(),
        [QuestionType.Sequence] = new SequenceVerifier(),
        [QuestionType.MatchPairs] = new MatchPairsVerifier()
    };
    
    /// <summary>
    /// Validate question data (and optionally answer data)
    /// </summary>
    /// <param name="answer">The <see cref="UserAnswer"/></param>
    /// <param name="question">The corresponding <see cref="Question"/></param>
    /// <param name="type">The <see cref="QuestionType"/></param>
    /// <exception cref="ArgumentException">If the <see cref="QuestionType"/> is not valid</exception>
    public bool Verify(UserAnswer answer, Question question, QuestionType type)
    {
        if (Validators.TryGetValue(type, out var validator))
            return validator.Verify(answer.Answers, question.CorrectData);
        throw new ArgumentException($"The type {type} is not valid.");
    }
}