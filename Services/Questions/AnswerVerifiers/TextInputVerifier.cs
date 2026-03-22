using EduTests.Database.Entities;
using EduTests.Database.Enums;

namespace EduTests.Services.Questions.AnswerVerifiers;

public class TextInputVerifier : IVerifier
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
    /// Verify a <see cref="QuestionType.TextInput"/> type of answer
    /// </summary>
    /// <param name="answerData">The <see cref="QuestionData"/> object containing answer data</param>
    /// <param name="correctData">The <see cref="QuestionData"/> object containing correct data</param>
    /// <returns>True if the answer is correct, false otherwise</returns>
    /// <exception cref="ArgumentNullException">If either <paramref name="answerData"/> or <paramref name="correctData"/>
    /// don't have the <see cref="QuestionData.TextAnswer"/> filled out</exception>
    public bool Verify(QuestionData answerData, QuestionData correctData)
    {
        CheckFields(answerData);
        
        if (answerData.TextAnswer is null)
            throw new ArgumentNullException(nameof(answerData.TextAnswer));
        if (correctData.TextAnswer is null)
            throw new ArgumentNullException(nameof(correctData.TextAnswer));
        
        return answerData.TextAnswer.Equals(correctData.TextAnswer);
    }
}