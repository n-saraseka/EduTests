using EduTests.Database.Entities;
using EduTests.Services.Questions.QuestionValidators;
using NUnit.Framework;

namespace EduTests.Tests.QuestionValidatorsTests;

[TestFixture]
public class NumberInputQValidatorTests
{
    private NumberInputQValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new NumberInputQValidator();
    }

    [Test]
    public void CheckFields_ValidDataProvided_DoesntThrow()
    {
        // Arrange
        var data = new QuestionData
        {
            NumberAnswer = 42,
            Tolerance = 0.1
        };

        // Assert
        Assert.DoesNotThrow(() => _validator.CheckFields(data));
    }

    [Test]
    public void CheckFields_DataHasPairs_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            NumberAnswer = 42,
            Pairs = { new MatchingPair { Left = "first", Right = "second" } }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have the number filled out"));
    }

    [Test]
    public void CheckFields_DataHasLeftColumn_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            NumberAnswer = 42,
            LeftColumn = { "first", "second" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have the number filled out"));
    }

    [Test]
    public void CheckFields_DataHasRightColumn_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            NumberAnswer = 42,
            RightColumn = { "first", "second" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have the number filled out"));
    }

    [Test]
    public void CheckFields_DataHasOptions_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            NumberAnswer = 42,
            Options = { "Option 1", "Option 2" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have the number filled out"));
    }

    [Test]
    public void CheckFields_DataHasValidAnswers_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            NumberAnswer = 42,
            ValidAnswers = { "answer" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have the number filled out"));
    }

    [Test]
    public void CheckFields_DataHasValidIndices_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            NumberAnswer = 42,
            ValidIndices = { 0, 1 }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have the number filled out"));
    }

    [Test]
    public void CheckFields_DataHasChosenIndices_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            NumberAnswer = 42,
            ChosenIndices = { 0, 1 }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have the number filled out"));
    }

    [Test]
    public void CheckFields_DataHasSequence_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            NumberAnswer = 42,
            Sequence = { "first", "second" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have the number filled out"));
    }

    [Test]
    public void CheckFields_DataHasTextAnswer_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            NumberAnswer = 42,
            TextAnswer = "some answer"
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have the number filled out"));
    }

    [Test]
    public void CheckFields_DataWithoutNumberAnswer_DoesntThrow()
    {
        // Arrange
        var data = new QuestionData
        {
            Tolerance = 0.1
        };

        // Assert
        Assert.DoesNotThrow(() => _validator.CheckFields(data));
    }

    [Test]
    public void CheckFields_DataWithoutTolerance_DoesntThrow()
    {
        // Arrange
        var data = new QuestionData
        {
            NumberAnswer = 42
        };

        // Assert
        Assert.DoesNotThrow(() => _validator.CheckFields(data));
    }

    [Test]
    public void CheckFields_EmptyData_DoesntThrow()
    {
        // Arrange
        var data = new QuestionData();

        // Assert
        Assert.DoesNotThrow(() => _validator.CheckFields(data));
    }

    [Test]
    public void Validate_ValidDataProvided_DoesntThrow()
    {
        // Arrange
        var data = new QuestionData();

        var correctData = new QuestionData
        {
            NumberAnswer = 42,
            Tolerance = 0.1
        };

        // Assert
        Assert.DoesNotThrow(() => _validator.Validate(data, correctData));
    }

    [Test]
    public void Validate_DataHasNumberAnswer_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            NumberAnswer = 10
        };

        var correctData = new QuestionData
        {
            NumberAnswer = 42,
            Tolerance = 0.1
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(data, correctData));

        Assert.That(exception.Message,
            Contains.Substring($"{nameof(data.NumberAnswer)} should be null"));
    }

    [Test]
    public void Validate_CorrectDataHasNullNumberAnswer_ThrowsArgumentNullException()
    {
        // Arrange
        var data = new QuestionData();

        var correctData = new QuestionData
        {
            NumberAnswer = null
        };

        // Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _validator.Validate(data, correctData));

        Assert.That(exception.Message,
            Contains.Substring($"{nameof(correctData.NumberAnswer)} should not be null"));
    }

    [Test]
    public void Validate_DataHasExtraFields_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "Option 1", "Option 2" }
        };

        var correctData = new QuestionData
        {
            NumberAnswer = 42,
            Tolerance = 0.1
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(data, correctData));

        Assert.That(exception.Message,
            Contains.Substring("should only have the number filled out"));
    }

    [Test]
    public void Validate_CorrectDataHasExtraFields_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData();

        var correctData = new QuestionData
        {
            NumberAnswer = 42,
            Tolerance = 0.1,
            Options = { "Option 1" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(data, correctData));

        Assert.That(exception.Message,
            Contains.Substring("should only have the number filled out"));
    }
}