using EduTests.Database.Entities;
using EduTests.Services.Questions.AnswerValidators;
using NUnit.Framework;

namespace EduTests.Tests.AnswerValidatorsTests;

[TestFixture]
public class MultipleChoiceAValidatorTests
{
    private MultipleChoiceAValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new MultipleChoiceAValidator();
    }

    [Test]
    public void CheckFields_ValidDataProvided_DoesntThrow()
    {
        // Arrange
        var data = new QuestionData
        {
            ChosenIndices = { 0, 2, 3 }
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
            ChosenIndices = { 0 }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have single or multiple choice related fields filled out"));
    }

    [Test]
    public void CheckFields_DataHasValidIndices_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            ValidIndices = { 0, 1 },
            ChosenIndices = { 0 }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have single or multiple choice related fields filled out"));
    }

    [Test]
    public void CheckFields_DataHasPairs_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Pairs = { new MatchingPair { Left = "first", Right = "second" } },
            ChosenIndices = { 0 }
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
            LeftColumn = { "first", "second" },
            ChosenIndices = { 0 }
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
            RightColumn = { "first", "second" },
            ChosenIndices = { 0 }
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
            ValidAnswers = { "answer" },
            ChosenIndices = { 0 }
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
            Sequence = { "first", "second" },
            ChosenIndices = { 0 }
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
            TextAnswer = "some answer",
            ChosenIndices = { 0 }
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
            NumberAnswer = 42,
            ChosenIndices = { 0 }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have single or multiple choice related fields filled out"));
    }

    [Test]
    public void CheckFields_DataHasTolerance_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            Tolerance = 0.1,
            ChosenIndices = { 0 }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have single or multiple choice related fields filled out"));
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
            ChosenIndices = { 0, 2 }
        };

        var questionData = new QuestionData
        {
            Options = { "Option 1", "Option 2", "Option 3", "Option 4" }
        };

        // Assert
        Assert.DoesNotThrow(() => _validator.Validate(answerData, questionData));
    }

    [Test]
    public void Validate_ValidDataWithSingleChoice_DoesntThrow()
    {
        // Arrange
        var answerData = new QuestionData
        {
            ChosenIndices = { 1 }
        };

        var questionData = new QuestionData
        {
            Options = { "Option 1", "Option 2", "Option 3" }
        };

        // Assert
        Assert.DoesNotThrow(() => _validator.Validate(answerData, questionData));
    }

    [Test]
    public void Validate_ValidDataWithAllOptionsSelected_DoesntThrow()
    {
        // Arrange
        var answerData = new QuestionData
        {
            ChosenIndices = { 0, 1, 2, 3 }
        };

        var questionData = new QuestionData
        {
            Options = { "Option 1", "Option 2", "Option 3", "Option 4" }
        };

        // Assert
        Assert.DoesNotThrow(() => _validator.Validate(answerData, questionData));
    }

    [Test]
    public void Validate_ChosenIndicesCountGreaterThanOptionsCount_ThrowsArgumentException()
    {
        // Arrange
        var answerData = new QuestionData
        {
            ChosenIndices = { 0, 1, 2, 3, 4 }
        };

        var questionData = new QuestionData
        {
            Options = { "Option 1", "Option 2", "Option 3", "Option 4" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(answerData, questionData));

        Assert.That(exception.Message,
            Contains.Substring($"{nameof(answerData.ChosenIndices.Count)} must be less or equal to {nameof(questionData.Options.Count)}"));
    }

    [Test]
    public void Validate_ChosenIndicesOutOfRangeBelowZero_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var answerData = new QuestionData
        {
            ChosenIndices = { -1, 1 }
        };

        var questionData = new QuestionData
        {
            Options = { "Option 1", "Option 2", "Option 3" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _validator.Validate(answerData, questionData));

        Assert.That(exception.Message, Contains.Substring($"{nameof(answerData.ChosenIndices)}"));
    }

    [Test]
    public void Validate_ChosenIndicesOutOfRangeAboveMax_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var answerData = new QuestionData
        {
            ChosenIndices = { 0, 3 }
        };

        var questionData = new QuestionData
        {
            Options = { "Option 1", "Option 2", "Option 3" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _validator.Validate(answerData, questionData));

        Assert.That(exception.Message, Contains.Substring($"{nameof(answerData.ChosenIndices)}"));
    }

    [Test]
    public void Validate_EmptyChosenIndices_DoesntThrow()
    {
        // Arrange
        var answerData = new QuestionData
        {
            ChosenIndices = { }
        };

        var questionData = new QuestionData
        {
            Options = { "Option 1", "Option 2", "Option 3" }
        };

        // Assert
        Assert.DoesNotThrow(() => _validator.Validate(answerData, questionData));
    }

    [Test]
    public void Validate_AnswerDataHasExtraFields_ThrowsArgumentException()
    {
        // Arrange
        var answerData = new QuestionData
        {
            ChosenIndices = { 0, 1 },
            Options = { "Option 1", "Option 2" }
        };

        var questionData = new QuestionData
        {
            Options = { "Option 1", "Option 2", "Option 3" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(answerData, questionData));

        Assert.That(exception.Message,
            Contains.Substring("should only have single or multiple choice related fields filled out"));
    }
}