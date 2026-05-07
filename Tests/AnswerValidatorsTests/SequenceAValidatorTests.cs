using EduTests.Database.Entities;
using EduTests.Services.Questions.AnswerValidators;
using NUnit.Framework;

namespace EduTests.Tests.AnswerValidatorsTests;

[TestFixture]
public class SequenceAValidatorTests
{
    private SequenceAValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new SequenceAValidator();
    }

    [Test]
    public void CheckFields_ValidDataProvided_DoesntThrow()
    {
        // Arrange
        var data = new QuestionData
        {
            Sequence = { "first", "second", "third" }
        };

        // Assert
        Assert.DoesNotThrow(() => _validator.CheckFields(data));
    }

    [Test]
    public void CheckFields_DataHasOptions_ThrowsArgumentException()
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
            Contains.Substring("should only have the sequence filled out"));
    }

    [Test]
    public void CheckFields_DataHasPairs_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Pairs = { new MatchingPair { Left = "first", Right = "second" } },
            Sequence = { "first", "second" }
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
            LeftColumn = { "first", "second" },
            Sequence = { "first", "second" }
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
            RightColumn = { "first", "second" },
            Sequence = { "first", "second" }
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
            ValidAnswers = { "answer" },
            Sequence = { "first", "second" }
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
            ValidIndices = { 0, 1 },
            Sequence = { "first", "second" }
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
            ChosenIndices = { 0, 1 },
            Sequence = { "first", "second" }
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
            TextAnswer = "some answer",
            Sequence = { "first", "second" }
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
            NumberAnswer = 42,
            Sequence = { "first", "second" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have the sequence filled out"));
    }

    [Test]
    public void CheckFields_DataHasTolerance_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Tolerance = 0.1,
            Sequence = { "first", "second" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have the sequence filled out"));
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
        var answerData = new QuestionData
        {
            Sequence = { "first", "second", "third" }
        };

        var questionData = new QuestionData
        {
            Options = { "first", "second", "third" }
        };

        // Assert
        Assert.DoesNotThrow(() => _validator.Validate(answerData, questionData));
    }

    [Test]
    public void Validate_ValidDataWithDifferentOrder_DoesntThrow()
    {
        // Arrange
        var answerData = new QuestionData
        {
            Sequence = { "third", "first", "second" }
        };

        var questionData = new QuestionData
        {
            Options = { "first", "second", "third" }
        };

        // Assert
        Assert.DoesNotThrow(() => _validator.Validate(answerData, questionData));
    }

    [Test]
    public void Validate_ValidDataWithReversedOrder_DoesntThrow()
    {
        // Arrange
        var answerData = new QuestionData
        {
            Sequence = { "third", "second", "first" }
        };

        var questionData = new QuestionData
        {
            Options = { "first", "second", "third" }
        };

        // Assert
        Assert.DoesNotThrow(() => _validator.Validate(answerData, questionData));
    }

    [Test]
    public void Validate_SequenceCountDoesNotMatchOptionsCount_ThrowsArgumentException()
    {
        // Arrange
        var answerData = new QuestionData
        {
            Sequence = { "first", "second" }
        };

        var questionData = new QuestionData
        {
            Options = { "first", "second", "third" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(answerData, questionData));

        Assert.That(exception.Message,
            Contains.Substring($"{nameof(answerData.Sequence)} items must match {nameof(questionData.Options)}"));
    }

    [Test]
    public void Validate_SequenceHasMoreItemsThanOptions_ThrowsArgumentException()
    {
        // Arrange
        var answerData = new QuestionData
        {
            Sequence = { "first", "second", "third", "fourth" }
        };

        var questionData = new QuestionData
        {
            Options = { "first", "second", "third" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(answerData, questionData));

        Assert.That(exception.Message,
            Contains.Substring($"{nameof(answerData.Sequence)} items must match {nameof(questionData.Options)}"));
    }

    [Test]
    public void Validate_SequenceHasItemsNotInOptions_ThrowsArgumentException()
    {
        // Arrange
        var answerData = new QuestionData
        {
            Sequence = { "first", "second", "wrong" }
        };

        var questionData = new QuestionData
        {
            Options = { "first", "second", "third" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(answerData, questionData));

        Assert.That(exception.Message,
            Contains.Substring($"{nameof(answerData.Sequence)} items must match {nameof(questionData.Options)}"));
    }

    [Test]
    public void Validate_SequenceHasDuplicateItems_ThrowsArgumentException()
    {
        // Arrange
        var answerData = new QuestionData
        {
            Sequence = { "first", "first", "second" }
        };

        var questionData = new QuestionData
        {
            Options = { "first", "second", "third" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(answerData, questionData));

        Assert.That(exception.Message,
            Contains.Substring($"{nameof(answerData.Sequence)} items must match {nameof(questionData.Options)}"));
    }

    [Test]
    public void Validate_SequenceHasAllWrongItems_ThrowsArgumentException()
    {
        // Arrange
        var answerData = new QuestionData
        {
            Sequence = { "wrong1", "wrong2", "wrong3" }
        };

        var questionData = new QuestionData
        {
            Options = { "first", "second", "third" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(answerData, questionData));

        Assert.That(exception.Message,
            Contains.Substring($"{nameof(answerData.Sequence)} items must match {nameof(questionData.Options)}"));
    }

    [Test]
    public void Validate_EmptySequence_ThrowsArgumentException()
    {
        // Arrange
        var answerData = new QuestionData
        {
            Sequence = { }
        };

        var questionData = new QuestionData
        {
            Options = { "first", "second", "third" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(answerData, questionData));

        Assert.That(exception.Message,
            Contains.Substring($"{nameof(answerData.Sequence)} items must match {nameof(questionData.Options)}"));
    }

    [Test]
    public void Validate_AnswerDataHasExtraFields_ThrowsArgumentException()
    {
        // Arrange
        var answerData = new QuestionData
        {
            Sequence = { "first", "second", "third" },
            Options = { "Option 1", "Option 2" }
        };

        var questionData = new QuestionData
        {
            Options = { "first", "second", "third" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(answerData, questionData));

        Assert.That(exception.Message,
            Contains.Substring("should only have the sequence filled out"));
    }
}