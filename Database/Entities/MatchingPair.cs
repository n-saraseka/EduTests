namespace EduTests.Database.Entities;

public class MatchingPair : IEquatable<MatchingPair>
{
    public required string Left { get; set; }
    public required string Right { get; set; }

    public bool Equals(MatchingPair? other)
    {
        if (other is null)
            return false;
        return Left == other.Left && Right == other.Right;
    }
}