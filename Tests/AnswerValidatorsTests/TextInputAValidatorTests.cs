using EduTests.Database.Entities;
using EduTests.Services.Questions.AnswerValidators;
using NUnit.Framework;

namespace EduTests.Tests.AnswerValidatorsTests;

[TestFixture]
public class TextInputAValidatorTests
{
    private TextInputAValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new TextInputAValidator();
    }

    [Test]
    public void CheckFields_ValidDataProvided_DoesntThrow()
    {
        // Arrange
        var data = new QuestionData
        {
            TextAnswer = "user answer"
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
            TextAnswer = "user answer",
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
            TextAnswer = "user answer",
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
            TextAnswer = "user answer",
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
            TextAnswer = "user answer",
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
            TextAnswer = "user answer",
            Sequence = { "first", "second" }
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
            TextAnswer = "user answer",
            NumberAnswer = 42
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
            TextAnswer = "user answer",
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
            TextAnswer = "user answer",
            ChosenIndices = { 0, 1 }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have the text answer or valid answers filled out"));
    }

    [Test]
    public void CheckFields_DataHasTolerance_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            TextAnswer = "user answer",
            Tolerance = 0.1
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have the text answer or valid answers filled out"));
    }

    [Test]
    public void CheckFields_DataHasValidAnswers_ThrowsArgumentException()
    {
        // Arrange
        var data = new QuestionData
        {
            TextAnswer = "user answer",
            ValidAnswers = { "answer1", "answer2" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.CheckFields(data));

        Assert.That(exception.Message,
            Contains.Substring("should only have the text answer or valid answers filled out"));
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
    public void CheckFields_DataWithEmptyStringAnswer_DoesntThrow()
    {
        // Arrange
        var data = new QuestionData
        {
            TextAnswer = ""
        };

        // Assert
        Assert.DoesNotThrow(() => _validator.CheckFields(data));
    }

    [Test]
    public void CheckFields_DataWithWhitespaceAnswer_DoesntThrow()
    {
        // Arrange
        var data = new QuestionData
        {
            TextAnswer = "   "
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
            TextAnswer = "user answer"
        };

        var questionData = new QuestionData
        {
            ValidAnswers = { "correct answer", "another correct answer" }
        };

        // Assert
        Assert.DoesNotThrow(() => _validator.Validate(answerData, questionData));
    }

    [Test]
    public void Validate_ValidDataWithEmptyString_DoesntThrow()
    {
        // Arrange
        var answerData = new QuestionData
        {
            TextAnswer = ""
        };

        var questionData = new QuestionData
        {
            ValidAnswers = { "", "correct answer" }
        };

        // Assert
        Assert.DoesNotThrow(() => _validator.Validate(answerData, questionData));
    }

    [Test]
    public void Validate_ValidDataWithLongText_DoesntThrow()
    {
        // Arrange
        var answerData = new QuestionData
        {
            TextAnswer = "This is a very long user answer with multiple words and punctuation!"
        };

        var questionData = new QuestionData
        {
            ValidAnswers = { "short", "another answer" }
        };

        // Assert
        Assert.DoesNotThrow(() => _validator.Validate(answerData, questionData));
    }

    [Test]
    public void Validate_TextAnswerIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var answerData = new QuestionData
        {
            TextAnswer = null
        };

        var questionData = new QuestionData
        {
            ValidAnswers = { "correct answer" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _validator.Validate(answerData, questionData));

        Assert.That(exception.Message,
            Contains.Substring($"{nameof(answerData.TextAnswer)} must not be null"));
    }

    [Test]
    public void Validate_AnswerDataHasExtraFields_ThrowsArgumentException()
    {
        // Arrange
        var answerData = new QuestionData
        {
            TextAnswer = "user answer",
            Options = { "Option 1", "Option 2" }
        };

        var questionData = new QuestionData
        {
            ValidAnswers = { "correct answer" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(answerData, questionData));

        Assert.That(exception.Message,
            Contains.Substring("should only have the text answer or valid answers filled out"));
    }

    [Test]
    public void Validate_EmptyAnswerData_ThrowsArgumentNullException()
    {
        // Arrange
        var answerData = new QuestionData();

        var questionData = new QuestionData
        {
            ValidAnswers = { "correct answer" }
        };

        // Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _validator.Validate(answerData, questionData));

        Assert.That(exception.Message,
            Contains.Substring($"{nameof(answerData.TextAnswer)} must not be null"));
    }
}