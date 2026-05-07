using EduTests.Database.Entities;
using EduTests.Services.Questions.QuestionValidators;
using NUnit.Framework;

namespace EduTests.Tests.QuestionValidatorsTests;

[TestFixture]
public class SequenceQValidatorTests
{
    private SequenceQValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new SequenceQValidator();
    }

    [Test]
    public void CheckFields_ValidDataProvided_DoesntThrow()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "first", "second", "third" },
            Sequence = { "first", "second", "third" }
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
            Options = { "first", "second" },
            Sequence = { "first", "second" },
            Pairs = { new MatchingPair { Left = "first", Right = "second" } }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have the sequence filled out"));
    }

    [Test]
    public void CheckFields_DataHasLeftColumn_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "first", "second" },
            Sequence = { "first", "second" },
            LeftColumn = { "first", "second" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have the sequence filled out"));
    }

    [Test]
    public void CheckFields_DataHasRightColumn_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "first", "second" },
            Sequence = { "first", "second" },
            RightColumn = { "first", "second" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have the sequence filled out"));
    }

    [Test]
    public void CheckFields_DataHasValidAnswers_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "first", "second" },
            Sequence = { "first", "second" },
            ValidAnswers = { "answer" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have the sequence filled out"));
    }

    [Test]
    public void CheckFields_DataHasValidIndices_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "first", "second" },
            Sequence = { "first", "second" },
            ValidIndices = { 0, 1 }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have the sequence filled out"));
    }

    [Test]
    public void CheckFields_DataHasChosenIndices_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "first", "second" },
            Sequence = { "first", "second" },
            ChosenIndices = { 0, 1 }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have the sequence filled out"));
    }

    [Test]
    public void CheckFields_DataHasTextAnswer_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "first", "second" },
            Sequence = { "first", "second" },
            TextAnswer = "some answer"
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have the sequence filled out"));
    }

    [Test]
    public void CheckFields_DataHasNumberAnswer_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "first", "second" },
            Sequence = { "first", "second" },
            NumberAnswer = 42
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have the sequence filled out"));
    }

    [Test]
    public void CheckFields_DataWithoutSequence_DoesntThrow()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "first", "second", "third" }
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
            Options = { "first", "second", "third" }
        };

        var correctData = new QuestionData
        {
            Sequence = { "first", "second", "third" }
        };

        // Assert
        Assert.DoesNotThrow(() => _validator.Validate(data, correctData));
    }

    [Test]
    public void Validate_ValidDataWithDifferentOrder_DoesntThrow()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "first", "second", "third" }
        };

        var correctData = new QuestionData
        {
            Sequence = { "third", "first", "second" }
        };

        // Assert
        Assert.DoesNotThrow(() => _validator.Validate(data, correctData));
    }

    [Test]
    public void Validate_DataHasTooFewOptions_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "first" }
        };

        var correctData = new QuestionData
        {
            Sequence = { "first" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(data, correctData));

        Assert.That(exception.Message,
            Contains.Substring($"{nameof(data.Options)} must have at least two items"));
    }

    [Test]
    public void Validate_DataHasEmptyOptions_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { }
        };

        var correctData = new QuestionData
        {
            Sequence = { }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(data, correctData));

        Assert.That(exception.Message,
            Contains.Substring($"{nameof(data.Options)} must have at least two items"));
    }

    [Test]
    public void Validate_DataHasOneOption_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "only" }
        };

        var correctData = new QuestionData
        {
            Sequence = { "only" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(data, correctData));

        Assert.That(exception.Message,
            Contains.Substring($"{nameof(data.Options)} must have at least two items"));
    }

    [Test]
    public void Validate_SequenceCountDoesNotMatchOptionsCount_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "first", "second", "third" }
        };

        var correctData = new QuestionData
        {
            Sequence = { "first", "second" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(data, correctData));

        Assert.That(exception.Message,
            Contains.Substring($"{nameof(correctData.Sequence)} items must match {nameof(data.Options)}"));
    }

    [Test]
    public void Validate_SequenceHasItemsNotInOptions_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "first", "second", "third" }
        };

        var correctData = new QuestionData
        {
            Sequence = { "first", "second", "fourth" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(data, correctData));

        Assert.That(exception.Message,
            Contains.Substring($"{nameof(correctData.Sequence)} items must match {nameof(data.Options)}"));
    }

    [Test]
    public void Validate_SequenceHasDuplicateItems_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "first", "second", "third" }
        };

        var correctData = new QuestionData
        {
            Sequence = { "first", "first", "second" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(data, correctData));

        Assert.That(exception.Message,
            Contains.Substring($"{nameof(correctData.Sequence)} items must match {nameof(data.Options)}"));
    }

    [Test]
    public void Validate_DataHasExtraFields_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "first", "second", "third" },
            ValidIndices = { 0, 1 }
        };

        var correctData = new QuestionData
        {
            Sequence = { "first", "second", "third" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(data, correctData));

        Assert.That(exception.Message,
            Contains.Substring("should only have the sequence filled out"));
    }

    [Test]
    public void Validate_CorrectDataHasExtraFields_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Options = { "first", "second", "third" }
        };

        var correctData = new QuestionData
        {
            Sequence = { "first", "second", "third" },
            NumberAnswer = 42
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(data, correctData));

        Assert.That(exception.Message,
            Contains.Substring("should only have the sequence filled out"));
    }
}