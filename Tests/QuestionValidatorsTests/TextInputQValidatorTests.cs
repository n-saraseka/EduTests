using EduTests.Database.Entities;
using EduTests.Services.Questions.QuestionValidators;
using NUnit.Framework;

namespace EduTests.Tests.QuestionValidatorsTests;

[TestFixture]
public class TextInputQValidatorTests
{
    private TextInputQValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new TextInputQValidator();
    }

    [Test]
    public void CheckFields_ValidDataProvided_DoesntThrow()
    {
        // Arrange
        var data = new QuestionData
        {
            ValidAnswers = { "answer1", "answer2", "answer3" }
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
            ValidAnswers = { "answer1" },
            Pairs = { new MatchingPair { Left = "first", Right = "second" } }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have the text answer or valid answers filled out"));
    }

    [Test]
    public void CheckFields_DataHasLeftColumn_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            ValidAnswers = { "answer1" },
            LeftColumn = { "first", "second" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have the text answer or valid answers filled out"));
    }

    [Test]
    public void CheckFields_DataHasRightColumn_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            ValidAnswers = { "answer1" },
            RightColumn = { "first", "second" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have the text answer or valid answers filled out"));
    }

    [Test]
    public void CheckFields_DataHasOptions_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            ValidAnswers = { "answer1" },
            Options = { "Option 1", "Option 2" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have the text answer or valid answers filled out"));
    }

    [Test]
    public void CheckFields_DataHasSequence_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            ValidAnswers = { "answer1" },
            Sequence = { "first", "second" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have the text answer or valid answers filled out"));
    }

    [Test]
    public void CheckFields_DataHasValidIndices_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            ValidAnswers = { "answer1" },
            ValidIndices = { 0, 1 }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have the text answer or valid answers filled out"));
    }

    [Test]
    public void CheckFields_DataHasChosenIndices_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            ValidAnswers = { "answer1" },
            ChosenIndices = { 0, 1 }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have the text answer or valid answers filled out"));
    }

    [Test]
    public void CheckFields_DataHasNumberAnswer_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            ValidAnswers = { "answer1" },
            NumberAnswer = 42
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have the text answer or valid answers filled out"));
    }

    [Test]
    public void CheckFields_DataHasTextAnswer_DoesntThrow()
    {
        // Arrange
        var data = new QuestionData
        {
            TextAnswer = "some answer"
        };

        // Assert
        Assert.DoesNotThrow(() => _validator.CheckFields(data));
    }

    [Test]
    public void CheckFields_DataHasBothTextAnswerAndValidAnswers_DoesntThrow()
    {
        // Arrange
        var data = new QuestionData
        {
            TextAnswer = "some answer",
            ValidAnswers = { "answer1", "answer2" }
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
            ValidAnswers = { "correct answer", "another correct answer" }
        };

        // Assert
        Assert.DoesNotThrow(() => _validator.Validate(data, correctData));
    }

    [Test]
    public void Validate_DataHasTextAnswer_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            TextAnswer = "user answer"
        };

        var correctData = new QuestionData
        {
            ValidAnswers = { "correct answer" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(data, correctData));

        Assert.That(exception.Message,
            Contains.Substring($"{nameof(data.TextAnswer)} should be null"));
    }

    [Test]
    public void Validate_CorrectDataHasTextAnswer_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData();

        var correctData = new QuestionData
        {
            TextAnswer = "some text",
            ValidAnswers = { "correct answer" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(data, correctData));

        Assert.That(exception.Message,
            Contains.Substring($"{nameof(correctData.TextAnswer)} should be null"));
    }

    [Test]
    public void Validate_ValidAnswersEmpty_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData();

        var correctData = new QuestionData
        {
            ValidAnswers = { }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(data, correctData));

        Assert.That(exception.Message,
            Contains.Substring($"{nameof(correctData.ValidAnswers)} should have at least one item"));
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
            ValidAnswers = { "correct answer" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(data, correctData));

        Assert.That(exception.Message,
            Contains.Substring("should only have the text answer or valid answers filled out"));
    }

    [Test]
    public void Validate_CorrectDataHasExtraFields_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData();

        var correctData = new QuestionData
        {
            ValidAnswers = { "correct answer" },
            NumberAnswer = 42
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(data, correctData));

        Assert.That(exception.Message,
            Contains.Substring("should only have the text answer or valid answers filled out"));
    }

    [Test]
    public void Validate_DataHasTextAnswerAndCorrectDataValid_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            TextAnswer = "user answer"
        };

        var correctData = new QuestionData
        {
            ValidAnswers = { "correct answer", "another correct answer" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(data, correctData));

        Assert.That(exception.Message,
            Contains.Substring($"{nameof(data.TextAnswer)} should be null"));
    }

    [Test]
    public void Validate_ValidAnswersWithMultipleItems_DoesntThrow()
    {
        // Arrange
        var data = new QuestionData();

        var correctData = new QuestionData
        {
            ValidAnswers = { "answer1", "answer2", "answer3", "answer4", "answer5" }
        };

        // Assert
        Assert.DoesNotThrow(() => _validator.Validate(data, correctData));
    }

    [Test]
    public void Validate_ValidAnswersWithSingleItem_DoesntThrow()
    {
        // Arrange
        var data = new QuestionData();

        var correctData = new QuestionData
        {
            ValidAnswers = { "only answer" }
        };

        // Assert
        Assert.DoesNotThrow(() => _validator.Validate(data, correctData));
    }
}