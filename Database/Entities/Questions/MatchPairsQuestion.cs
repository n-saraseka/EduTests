using EduTests.Database.Enums;

namespace EduTests.Database.Entities.Questions;

public class MatchPairsQuestion : QuestionData
{
    public List<MatchingPair> Pairs { get; set; } = new();
    
    public override void ValidateQuestion(QuestionData? correctData, QuestionType expectedType)
    {
        if (correctData is not MatchPairsQuestion correct)
            throw new ArgumentException($"{nameof(correctData)} must be of type {nameof(MatchPairsQuestion)}");
        if (correct.Pairs.Count == 0 || Pairs.Count == 0)
            throw new ArgumentException($"{nameof(Pairs)} must not be empty");
        var availableLeft = Pairs.Select(p => p.Left);
        var availableRight = Pairs.Select(p => p.Right);
        var correctLeft = correct.Pairs.Select(p => p.Left);
        var correctRight = correct.Pairs.Select(p => p.Right);
        if (availableRight.Count() != correctRight.Count())
            throw new ArgumentException($"{nameof(Pairs)} must have the same amount of options in the right column");
        if (availableLeft.Count() != correctLeft.Count())
            throw new ArgumentException($"{nameof(Pairs)} must have the same amount of options in the left column");
        if (correctLeft.Any(s => !availableLeft.Contains(s)) || correctRight.Any(s => !availableRight.Contains(s)))
            throw new ArgumentException($"Each column in {nameof(Pairs)} should have the same items");
    }

    public override void ValidateAnswer()
    {
        if (Pairs.Count == 0)
            throw new ArgumentException($"{nameof(Pairs)} must not be empty");
    }
}