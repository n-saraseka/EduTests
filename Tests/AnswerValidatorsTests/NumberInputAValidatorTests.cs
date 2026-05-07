using EduTests.Database.Entities;
using EduTests.Services.Questions.AnswerValidators;
using NUnit.Framework;

namespace EduTests.Tests.AnswerValidatorsTests;

[TestFixture]
public class NumberInputAValidatorTests
{
    private NumberInputAValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new NumberInputAValidator();
    }

    [Test]
    public void CheckFields_ValidDataProvided_DoesntThrow()
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
    public void CheckFields_DataHasTolerance_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            NumberAnswer = 42,
            Tolerance = 0.1
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have the number filled out"));
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
    public void CheckFields_DataWithNegativeNumber_DoesntThrow()
    {
        // Arrange
        var data = new QuestionData
        {
            NumberAnswer = -42.5
        };

        // Assert
        Assert.DoesNotThrow(() => _validator.CheckFields(data));
    }

    [Test]
    public void CheckFields_DataWithZero_DoesntThrow()
    {
        // Arrange
        var data = new QuestionData
        {
            NumberAnswer = 0
        };

        // Assert
        Assert.DoesNotThrow(() => _validator.CheckFields(data));
    }

    [Test]
    public void Validate_ValidDataProvided_DoesntThrow()
    {
        // Arrange
        var answerData = new QuestionData
        {
            NumberAnswer = 42
        };

        var questionData = new QuestionData
        {
            NumberAnswer = 42,
            Tolerance = 0.1
        };

        // Assert
        Assert.DoesNotThrow(() => _validator.Validate(answerData, questionData));
    }

    [Test]
    public void Validate_ValidDataWithNegativeNumber_DoesntThrow()
    {
        // Arrange
        var answerData = new QuestionData
        {
            NumberAnswer = -10.5
        };

        var questionData = new QuestionData
        {
            NumberAnswer = -10.5,
            Tolerance = 0.1
        };

        // Assert
        Assert.DoesNotThrow(() => _validator.Validate(answerData, questionData));
    }

    [Test]
    public void Validate_ValidDataWithZero_DoesntThrow()
    {
        // Arrange
        var answerData = new QuestionData
        {
            NumberAnswer = 0
        };

        var questionData = new QuestionData
        {
            NumberAnswer = 0,
            Tolerance = 0.1
        };

        // Assert
        Assert.DoesNotThrow(() => _validator.Validate(answerData, questionData));
    }

    [Test]
    public void Validate_ValidDataWithDecimal_DoesntThrow()
    {
        // Arrange
        var answerData = new QuestionData
        {
            NumberAnswer = 3.14159
        };

        var questionData = new QuestionData
        {
            NumberAnswer = 3.14159,
            Tolerance = 0.001
        };

        // Assert
        Assert.DoesNotThrow(() => _validator.Validate(answerData, questionData));
    }

    [Test]
    public void Validate_NumberAnswerIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var answerData = new QuestionData
        {
            NumberAnswer = null
        };

        var questionData = new QuestionData
        {
            NumberAnswer = 42,
            Tolerance = 0.1
        };

        // Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _validator.Validate(answerData, questionData));

        Assert.That(exception.Message,
            Contains.Substring($"{nameof(answerData.NumberAnswer)} must not be null"));
    }

    [Test]
    public void Validate_AnswerDataHasExtraFields_ThrowsArgumentException()
    {
        // Arrange
        var answerData = new QuestionData
        {
            NumberAnswer = 42,
            Options = { "Option 1", "Option 2" }
        };

        var questionData = new QuestionData
        {
            NumberAnswer = 42,
            Tolerance = 0.1
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(answerData, questionData));

        Assert.That(exception.Message,
            Contains.Substring("should only have the number filled out"));
    }

    [Test]
    public void Validate_EmptyAnswerData_ThrowsArgumentNullException()
    {
        // Arrange
        var answerData = new QuestionData();

        var questionData = new QuestionData
        {
            NumberAnswer = 42,
            Tolerance = 0.1
        };

        // Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _validator.Validate(answerData, questionData));

        Assert.That(exception.Message,
            Contains.Substring($"{nameof(answerData.NumberAnswer)} must not be null"));
    }
}