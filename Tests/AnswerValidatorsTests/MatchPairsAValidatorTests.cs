using EduTests.Database.Entities;
using EduTests.Services.Questions.AnswerValidators;
using NUnit.Framework;

namespace EduTests.Tests.AnswerValidatorsTests;

[TestFixture]
public class MatchPairsAValidatorTests
{
    private MatchPairsAValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new MatchPairsAValidator();
    }

    [Test]
    public void CheckFields_ValidDataProvided_DoesntThrow()
    {
        // Arrange
        var data = new QuestionData
        {
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
    public void CheckFields_DataHasLeftColumn_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            LeftColumn = { "first", "second" },
            Pairs =
            {
                new MatchingPair { Left = "first", Right = "third" }
            }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have pairs filled out"));
    }

    [Test]
    public void CheckFields_DataHasRightColumn_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            RightColumn = { "third", "fourth" },
            Pairs =
            {
                new MatchingPair { Left = "first", Right = "third" }
            }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have pairs filled out"));
    }

    [Test]
    public void CheckFields_DataHasOptions_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "Option 1", "Option 2" },
            Pairs =
            {
                new MatchingPair { Left = "first", Right = "third" }
            }
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
            ValidAnswers = { "answer" },
            Pairs =
            {
                new MatchingPair { Left = "first", Right = "third" }
            }
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
            Sequence = { "first", "second" },
            Pairs =
            {
                new MatchingPair { Left = "first", Right = "third" }
            }
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
            TextAnswer = "some answer",
            Pairs =
            {
                new MatchingPair { Left = "first", Right = "third" }
            }
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
            NumberAnswer = 42,
            Pairs =
            {
                new MatchingPair { Left = "first", Right = "third" }
            }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have pairs filled out"));
    }

    [Test]
    public void CheckFields_DataHasValidIndices_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            ValidIndices = { 0, 1 },
            Pairs =
            {
                new MatchingPair { Left = "first", Right = "third" }
            }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have pairs filled out"));
    }

    [Test]
    public void CheckFields_DataHasChosenIndices_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            ChosenIndices = { 0, 1 },
            Pairs =
            {
                new MatchingPair { Left = "first", Right = "third" }
            }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have pairs filled out"));
    }

    [Test]
    public void CheckFields_DataHasTolerance_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Tolerance = 0.1,
            Pairs =
            {
                new MatchingPair { Left = "first", Right = "third" }
            }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have pairs filled out"));
    }

    [Test]
    public void Validate_ValidDataProvided_DoesntThrow()
    {
        // Arrange
        var answerData = new QuestionData
        {
            Pairs =
            {
                new MatchingPair { Left = "first", Right = "third" },
                new MatchingPair { Left = "second", Right = "fourth" }
            }
        };

        var questionData = new QuestionData
        {
            LeftColumn = { "first", "second" },
            RightColumn = { "third", "fourth" }
        };

        // Assert
        Assert.DoesNotThrow(() => _validator.Validate(answerData, questionData));
    }

    [Test]
    public void Validate_PairsCountDoesNotMatchLeftColumnCount_ThrowsArgumentException()
    {
        // Arrange
        var answerData = new QuestionData
        {
            Pairs =
            {
                new MatchingPair { Left = "first", Right = "third" }
            }
        };

        var questionData = new QuestionData
        {
            LeftColumn = { "first", "second" },
            RightColumn = { "third", "fourth" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(answerData, questionData));

        Assert.That(exception.Message,
            Contains.Substring($"{nameof(answerData.Pairs.Count)} should match the question column count"));
    }

    [Test]
    public void Validate_PairsHasLeftItemNotInLeftColumn_ThrowsArgumentException()
    {
        // Arrange
        var answerData = new QuestionData
        {
            Pairs =
            {
                new MatchingPair { Left = "wrong", Right = "third" },
                new MatchingPair { Left = "second", Right = "fourth" }
            }
        };

        var questionData = new QuestionData
        {
            LeftColumn = { "first", "second" },
            RightColumn = { "third", "fourth" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(answerData, questionData));

        Assert.That(exception.Message,
            Contains.Substring($"Each column in {nameof(answerData.Pairs)} should have the same items as"));
    }

    [Test]
    public void Validate_PairsHasRightItemNotInRightColumn_ThrowsArgumentException()
    {
        // Arrange
        var answerData = new QuestionData
        {
            Pairs =
            {
                new MatchingPair { Left = "first", Right = "wrong" },
                new MatchingPair { Left = "second", Right = "fourth" }
            }
        };

        var questionData = new QuestionData
        {
            LeftColumn = { "first", "second" },
            RightColumn = { "third", "fourth" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(answerData, questionData));

        Assert.That(exception.Message,
            Contains.Substring($"Each column in {nameof(answerData.Pairs)} should have the same items as"));
    }

    [Test]
    public void Validate_PairsHasMultipleInvalidItems_ThrowsArgumentException()
    {
        // Arrange
        var answerData = new QuestionData
        {
            Pairs =
            {
                new MatchingPair { Left = "wrong1", Right = "wrong2" },
                new MatchingPair { Left = "second", Right = "fourth" }
            }
        };

        var questionData = new QuestionData
        {
            LeftColumn = { "first", "second" },
            RightColumn = { "third", "fourth" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(answerData, questionData));

        Assert.That(exception.Message,
            Contains.Substring($"Each column in {nameof(answerData.Pairs)} should have the same items as"));
    }

    [Test]
    public void Validate_AnswerDataHasExtraFields_ThrowsArgumentException()
    {
        // Arrange
        var answerData = new QuestionData
        {
            Options = { "Option 1" },
            Pairs =
            {
                new MatchingPair { Left = "first", Right = "third" },
                new MatchingPair { Left = "second", Right = "fourth" }
            }
        };

        var questionData = new QuestionData
        {
            LeftColumn = { "first", "second" },
            RightColumn = { "third", "fourth" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(answerData, questionData));

        Assert.That(exception.Message,
            Contains.Substring("should only have pairs filled out"));
    }

    [Test]
    public void Validate_PairsWithDifferentOrder_DoesntThrow()
    {
        // Arrange
        var answerData = new QuestionData
        {
            Pairs =
            {
                new MatchingPair { Left = "second", Right = "fourth" },
                new MatchingPair { Left = "first", Right = "third" }
            }
        };

        var questionData = new QuestionData
        {
            LeftColumn = { "first", "second" },
            RightColumn = { "third", "fourth" }
        };

        // Assert
        Assert.DoesNotThrow(() => _validator.Validate(answerData, questionData));
    }
}