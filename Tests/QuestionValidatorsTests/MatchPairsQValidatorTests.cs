using EduTests.Database.Entities;
using EduTests.Services.Questions.QuestionValidators;
using NUnit.Framework;

namespace EduTests.Tests.QuestionValidatorsTests;

[TestFixture]
public class MatchPairsQValidatorTests
{
    private MatchPairsQValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new MatchPairsQValidator();
    }

    [Test]
    public void CheckFields_ValidDataProvided_DoesntThrow()
    {
        // Arrange
        var data = new QuestionData
        {
            LeftColumn = { "first", "second" },
            RightColumn = { "third", "fourth" },
            Pairs =
            {
                new MatchingPair { Left = "first", Right = "third" },
                new MatchingPair { Left = "second", Right = "fourth" }
            }
        };
        
        // Assert
        Assert.DoesNotThrow(() => _validator.CheckFields(data));
    }
    
    [Test]
    public void CheckFields_HasOptions_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "Option 1", "Option 2" },
            Pairs = { new MatchingPair { Left = "first", Right = "second" } }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have pairs filled out"));
    }
    
    [Test]
    public void CheckFields_HasChosenIndices_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            ChosenIndices = {0, 1}
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have pairs filled out"));
    }
    
    [Test]
    public void CheckFields_HasValidIndices_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            ValidIndices = {0, 1}
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have pairs filled out"));
    }

    [Test]
    public void CheckFields_DataHasValidAnswers_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "Option 1", "Option 2" },
            ValidAnswers = { "answer" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have pairs filled out"));
    }

    [Test]
    public void CheckFields_DataHasSequence_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "Option 1", "Option 2" },
            Sequence = { "first", "second" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have pairs filled out"));
    }

    [Test]
    public void CheckFields_DataHasTextAnswer_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "Option 1", "Option 2" },
            TextAnswer = "some answer"
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have pairs filled out"));
    }

    [Test]
    public void CheckFields_DataHasNumberAnswer_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "Option 1", "Option 2" },
            NumberAnswer = 42
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have pairs filled out"));
    }

    [Test]
    public void Validate_DataHasPair_ThrowsArgumentException()
    {
        var data = new QuestionData
        {
            LeftColumn = { "first", "second" },
            RightColumn = { "third", "fourth" },
            Pairs =
            {
                new MatchingPair { Left = "first", Right = "third" },
                new MatchingPair { Left = "second", Right = "fourth" }
            }
        };
        
        var correctData = new QuestionData
        {
            LeftColumn = { "first", "second" },
            RightColumn = { "third", "fourth" },
            Pairs =
            {
                new MatchingPair { Left = "first", Right = "third" },
                new MatchingPair { Left = "second", Right = "fourth" }
            }
        };
        
        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(data, correctData));
        
        Assert.That(exception.Message, Contains.Substring(" should not have any items"));
    }
    
    [Test]
    public void Validate_LeftAndRightNotEqual_ThrowsArgumentException()
    {
        var data = new QuestionData
        {
            LeftColumn = { "first" },
            RightColumn = { "third", "fourth" },
        };
        
        var correctData = new QuestionData
        {
            Pairs =
            {
                new MatchingPair { Left = "first", Right = "third" },
                new MatchingPair { Left = "second", Right = "fourth" }
            }
        };
        
        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(data, correctData));
        
        Assert.That(exception.Message, Contains.Substring(" should have the same amount of items"));
    }
    
    [Test]
    public void Validate_PairsAndColumnsNotEqualCount_ThrowsArgumentException()
    {
        var data = new QuestionData
        {
            LeftColumn = { "first", "second" },
            RightColumn = { "third", "fourth" },
        };
        
        var correctData = new QuestionData
        {
            Pairs =
            {
                new MatchingPair { Left = "first", Right = "third" },
            }
        };
        
        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(data, correctData));
        
        Assert.That(exception.Message, Contains.Substring(" and column items count should match"));
    }
    
    [Test]
    public void Validate_PairsAndColumnsDontContainSameItems_ThrowsArgumentException()
    {
        var data = new QuestionData
        {
            LeftColumn = { "first", "second" },
            RightColumn = { "third", "fourth" },
        };
        
        var correctData = new QuestionData
        {
            Pairs =
            {
                new MatchingPair { Left = "second", Right = "first" },
                new MatchingPair { Left = "third", Right = "fourth" },
            }
        };
        
        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(data, correctData));
        
        Assert.That(exception.Message, Contains.Substring(" should have the same items as"));
    }
}