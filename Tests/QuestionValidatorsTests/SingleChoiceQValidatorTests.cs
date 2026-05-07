using EduTests.Database.Entities;
using EduTests.Services.Questions.QuestionValidators;
using NUnit.Framework;

namespace EduTests.Tests.QuestionValidatorsTests;

[TestFixture]
public class SingleChoiceQValidatorTests
{
    private SingleChoiceQValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new SingleChoiceQValidator();
    }

    [Test]
    public void CheckFields_ValidDataProvided_DoesntThrow()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "Option 1", "Option 2", "Option 3" },
            ValidIndices = { 0 }
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
            Options = { "Option 1", "Option 2" },
            ValidIndices = { 0 },
            Pairs = { new MatchingPair { Left = "first", Right = "second" } }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have single or multiple choice related fields filled out"));
    }

    [Test]
    public void CheckFields_DataHasLeftColumn_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "Option 1", "Option 2" },
            ValidIndices = { 0 },
            LeftColumn = { "first", "second" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have single or multiple choice related fields filled out"));
    }

    [Test]
    public void CheckFields_DataHasRightColumn_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "Option 1", "Option 2" },
            ValidIndices = { 0 },
            RightColumn = { "first", "second" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have single or multiple choice related fields filled out"));
    }

    [Test]
    public void CheckFields_DataHasValidAnswers_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "Option 1", "Option 2" },
            ValidIndices = { 0 },
            ValidAnswers = { "answer" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have single or multiple choice related fields filled out"));
    }

    [Test]
    public void CheckFields_DataHasSequence_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "Option 1", "Option 2" },
            ValidIndices = { 0 },
            Sequence = { "first", "second" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have single or multiple choice related fields filled out"));
    }

    [Test]
    public void CheckFields_DataHasTextAnswer_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "Option 1", "Option 2" },
            ValidIndices = { 0 },
            TextAnswer = "some answer"
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have single or multiple choice related fields filled out"));
    }

    [Test]
    public void CheckFields_DataHasNumberAnswer_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "Option 1", "Option 2" },
            ValidIndices = { 0 },
            NumberAnswer = 42
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have single or multiple choice related fields filled out"));
    }

    [Test]
    public void CheckFields_DataWithoutValidIndices_DoesntThrow()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "Option 1", "Option 2", "Option 3" }
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
        var data = new QuestionData
        {
            Options = { "Option 1", "Option 2", "Option 3", "Option 4" }
        };

        var correctData = new QuestionData
        {
            ValidIndices = { 2 }
        };

        // Assert
        Assert.DoesNotThrow(() => _validator.Validate(data, correctData));
    }

    [Test]
    public void Validate_DataHasTooFewOptions_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "Option 1" }
        };

        var correctData = new QuestionData
        {
            ValidIndices = { 0 }
        };

        // Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _validator.Validate(data, correctData));

        Assert.That(exception.Message, Contains.Substring("must have between 2 and 10 items"));
    }

    [Test]
    public void Validate_DataHasTooManyOptions_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var data = new QuestionData();
        for (int i = 0; i < 11; i++)
        {
            data.Options.Add($"Option {i}");
        }

        var correctData = new QuestionData
        {
            ValidIndices = { 0 }
        };

        // Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _validator.Validate(data, correctData));

        Assert.That(exception.Message, Contains.Substring("must have between 2 and 10 items"));
    }

    [Test]
    public void Validate_ValidIndicesOutOfRangeBelowZero_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "Option 1", "Option 2", "Option 3" }
        };

        var correctData = new QuestionData
        {
            ValidIndices = { -1 }
        };

        // Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _validator.Validate(data, correctData));

        Assert.That(exception.Message, Contains.Substring($"{nameof(correctData.ValidIndices)}"));
    }

    [Test]
    public void Validate_ValidIndicesOutOfRangeAboveMax_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "Option 1", "Option 2", "Option 3" }
        };

        var correctData = new QuestionData
        {
            ValidIndices = { 3 }
        };

        // Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _validator.Validate(data, correctData));

        Assert.That(exception.Message, Contains.Substring($"{nameof(correctData.ValidIndices)}"));
    }

    [Test]
    public void Validate_ValidIndicesHasMoreThanOneItem_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "Option 1", "Option 2", "Option 3", "Option 4" }
        };

        var correctData = new QuestionData
        {
            ValidIndices = { 0, 2 }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(data, correctData));

        Assert.That(exception.Message,
            Contains.Substring($"{nameof(correctData.ValidIndices)} should have exactly one item"));
    }

    [Test]
    public void Validate_ValidIndicesHasZeroItems_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "Option 1", "Option 2", "Option 3", "Option 4" }
        };

        var correctData = new QuestionData
        {
            ValidIndices = { }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(data, correctData));

        Assert.That(exception.Message,
            Contains.Substring($"{nameof(correctData.ValidIndices)} should have exactly one item"));
    }

    [Test]
    public void Validate_DataHasExtraFields_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "Option 1", "Option 2", "Option 3" },
            Sequence = { "first", "second", "third" }
        };

        var correctData = new QuestionData
        {
            ValidIndices = { 0 }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(data, correctData));

        Assert.That(exception.Message,
            Contains.Substring("should only have single or multiple choice related fields filled out"));
    }

    [Test]
    public void Validate_CorrectDataHasExtraFields_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "Option 1", "Option 2", "Option 3" }
        };

        var correctData = new QuestionData
        {
            ValidIndices = { 0 },
            Pairs = { new MatchingPair { Left = "first", Right = "second" } }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(data, correctData));

        Assert.That(exception.Message,
            Contains.Substring("should only have single or multiple choice related fields filled out"));
    }
}