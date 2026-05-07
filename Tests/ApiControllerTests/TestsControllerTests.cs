using System.Security.Claims;
using EduTests.ApiObjects;
using EduTests.Commands.TestCommands;
using EduTests.Controllers.Api;
using EduTests.Database.Entities;
using EduTests.Database.Enums;
using EduTests.Database.Repositories.Interfaces;
using EduTests.Services;
using EduTests.Services.Questions;
using Microsoft.AspNetCore.Mvc;
using MockQueryable;
using Moq;
using NUnit.Framework;

namespace EduTests.Tests.ApiControllerTests;

[TestFixture]
public class TestsControllerTests
{
    private Mock<ITestRepository> _testMock;
    private Mock<IUserRatingRepository> _ratingMock;
    private Mock<ITestCompletionRepository> _completionMock;
    private Mock<ITagRepository> _tagMock;
    private Mock<IQuestionRepository> _questionMock;
    private Mock<ITestResultRepository> _resultMock;
    private Mock<ICommentRepository> _commentMock;
    private Mock<IUserAnswerRepository> _answerMock;
    private Mock<IAnonymousUserRepository> _anonMock;
    private Mock<IQuestionValidatorService> _validatorMock;
    private Mock<IEntityToDtoService> _entityToDtoServiceMock;
    
    private TestsController _controller;
    private DefaultHttpContext _httpContext;

    [SetUp]
    public void Setup()
    {
        _testMock = new Mock<ITestRepository>();
        _ratingMock = new Mock<IUserRatingRepository>();
        _completionMock = new Mock<ITestCompletionRepository>();
        _tagMock = new Mock<ITagRepository>();
        _questionMock = new Mock<IQuestionRepository>();
        _resultMock = new Mock<ITestResultRepository>();
        _commentMock = new Mock<ICommentRepository>();
        _answerMock = new Mock<IUserAnswerRepository>();
        _anonMock = new Mock<IAnonymousUserRepository>();
        _validatorMock = new Mock<IQuestionValidatorService>();
        _entityToDtoServiceMock = new Mock<IEntityToDtoService>();

        _controller = new TestsController(
            _testMock.Object, _ratingMock.Object, _completionMock.Object,
            _tagMock.Object, _questionMock.Object, _resultMock.Object,
            _commentMock.Object, _answerMock.Object, _anonMock.Object,
            _validatorMock.Object, _entityToDtoServiceMock.Object);

        _httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext { HttpContext = _httpContext };
    }

    [Test]
    public async Task CreateTest_NoQuestions_ReturnsBadRequest()
    {
        // Arrange
        SetupUser("1", "User");
        var command = new CreateOrUpdateTestCommand { 
            Name = "test",
            Questions = new List<ApiQuestion>() };

        // Act
        var result = await _controller.CreateTest(command);

        // Assert
        Assert.IsInstanceOf<BadRequestObjectResult>(result);
        Assert.That(((BadRequestObjectResult)result).Value, Is.EqualTo("No questions were added"));
    }

    [Test]
    public async Task UpdateTest_NotOwnerAndNotAdmin_ReturnsForbid()
    {
        // Arrange
        SetupUser("2", "User");
        var test = new Test
        {
            Id = 1,
            UserId = 10,
            Name = "Test",
        };
        _testMock.Setup(x => x.GetByIdWithTagsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(test);

        // Act
        var result = await _controller.UpdateTestAsync(1, new CreateOrUpdateTestCommand());

        // Assert
        Assert.IsInstanceOf<ForbidResult>(result);
    }

    [Test]
    public async Task RateTest_PasswordRequiredButNotVerified_ReturnsForbid()
    {
        // Arrange
        SetupUser("1", "User");
        var test = new Test
        {
            Id = 1,
            UserId = 1,
            Name = "Test",
            Password = "hashed_password",
        };
        
        _testMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(test);
        
        // Empty session
        var sessionMock = new MockSession();
        _httpContext.Session = sessionMock;

        // Act
        var result = await _controller.RateTestAsync(1, new RateTestCommand { IsPositive = true });

        // Assert
        Assert.IsInstanceOf<ForbidResult>(result);
    }

    [Test]
    public async Task GetTestRating_PrivateTest_OtherUser_ReturnsForbid()
    {
        // Arrange
        SetupUser("2", "User");
        var test = new Test
        {
            Id = 1,
            UserId = 10,
            Name = "Test",
            AccessType = AccessType.Private
        };
        _testMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(test);

        // Act
        var result = await _controller.GetTestRatingAsync(1);

        // Assert
        Assert.IsInstanceOf<ForbidResult>(result);
    }

    [Test]
    public async Task CreateCompletion_AttemptLimitReached_ReturnsForbid()
    {
        // Arrange
        SetupUser("1", "User");
        var test = new Test
        {
            Id = 1,
            UserId = 10,
            Name = "Test",
            AttemptLimit = 2
        };
        _testMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(test);

        var testCompletion = new TestCompletion
        {
            Id = 1,
            StartedAt = DateTime.UtcNow,
            TestId = 1
        };
        
        // 2 finished attempts
        _completionMock.Setup(x => x.GetByTestIdAndUserIdAsync(1, 1, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TestCompletion> { testCompletion, testCompletion });

        // Act
        var result = await _controller.CreateTestCompletionAsync(1);

        // Assert
        Assert.IsInstanceOf<ForbidResult>(result);
    }

    [Test]
    public async Task CreateCompletion_AlreadyHasActive_ReturnsBadRequest()
    {
        // Arrange
        SetupUser("1", "User");
        var test = new Test
        {
            Id = 1,
            UserId = 10,
            Name = "Test",
            AttemptLimit = 5
        };
        _testMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(test);

        var unfinishedCompletion = new TestCompletion
        {
            Id = 1,
            TestId = 1,
            StartedAt = DateTime.UtcNow
        };
        _completionMock.Setup(x => x.GetByTestIdAndUserIdAsync(1, 1, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TestCompletion> { unfinishedCompletion });

        // Act
        var result = await _controller.CreateTestCompletionAsync(1);

        // Assert
        Assert.IsInstanceOf<BadRequestObjectResult>(result);
        Assert.That(((BadRequestObjectResult)result).Value, Is.EqualTo("There is already a uncompleted completion"));
    }
    
    [Test]
    public async Task GetTests_UserRequestsOthersTests_FiltersOutPrivate()
    {
        // Arrange
        SetupUser("1", "User");
        int targetUserId = 99;
        
        var tests = new List<Test>
        {
            new Test { Id = 1,
                UserId = 99,
                Name = "Test",
                AccessType = AccessType.Public
            },
            new Test { Id = 2,
                UserId = 99,
                Name = "Test",
                AccessType = AccessType.Private
            },
        };
        
        var mockQuery = tests.BuildMock();
        _testMock.Setup(x => x.GetByUserId(targetUserId)).Returns(mockQuery);
        
        _ratingMock.Setup(x => x.GetTestRatingsAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<int, int> { { 1, 0 } });
        _completionMock.Setup(x => x.GetTestCompletionCountsAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<int, int> { { 1, 0 } });
        _entityToDtoServiceMock.Setup(x => x.TestEntityToDto(It.IsAny<Test>())).Returns((Test t) => new ApiTest 
        { 
            Id = t.Id, 
            Name = t.Name,
            UpdatedAt = t.UpdatedAt,
            CreatedAt = t.CreatedAt,
            HasPassword = t.Password != null,
            AccessType = t.AccessType,
            ShowCorrectAnswers = t.ShowCorrectAnswers
        });

        // Act
        var result = await _controller.GetTestsAsync(page: 1, amountPerPage: 10, userId: targetUserId, 
            sort: null, isDescending: null, isProfile: false);

        // Assert
        var okResult = (OkObjectResult)result;
        dynamic response = okResult.Value;
        Assert.That(response.tests.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task CreateCompletion_AnonymousUser_CreatesAnonRecordIfMissing()
    {
        // Arrange
        var anonId = Guid.NewGuid();
        _httpContext.Items["AnonymousId"] = anonId.ToString();
        _httpContext.User = new ClaimsPrincipal(new ClaimsIdentity());
        
        var test = new Test
        {
            Id = 1,
            UserId = 10,
            Name = "Test",
            AttemptLimit = 10
        };
        _testMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(test);
        
        _anonMock.Setup(x => x.GetByIdAsync(anonId, It.IsAny<CancellationToken>())).ReturnsAsync((AnonymousUser?)null);
        _completionMock.Setup(x => x.GetByTestIdAndUserIdAsync(1, null, anonId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TestCompletion>());

        // Act
        await _controller.CreateTestCompletionAsync(1);

        // Assert
        _anonMock.Verify(x => x.Create(It.Is<AnonymousUser>(u => u.Id == anonId)), Times.Once);
        _anonMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task CreateTest_NormalizationOfTags_WorksCorrectly()
    {
        // Arrange
        SetupUser("1", "User");
        var data = new QuestionData();
        var question = new ApiQuestion
        {
            OrderIndex = 1,
            TestId = 1,
            Type = QuestionType.MatchPairs,
            Description = "test",
            Data = data,
            CorrectData = data
        };
        var command = new CreateOrUpdateTestCommand 
        { 
            Name = "Test",
            Questions = new List<ApiQuestion> { question },
            Tags = new List<string> { "test", "TEST", "tEsT" }
        };

        _tagMock.Setup(x => x.GetByNameBulkAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Tag>());
        _testMock.Setup(x => x.GetByIdWithTagsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken ct) => new Test 
            { 
                Id = id,
                UserId = 1,
                Name = "Test",
                Tags = new List<Tag> { new Tag { Name = "test" } }
            });
        _entityToDtoServiceMock.Setup(x => x.TestEntityToDto(It.IsAny<Test>())).Returns((Test t) => new ApiTest 
        { 
            Id = t.Id, 
            Name = t.Name,
            UpdatedAt = t.UpdatedAt,
            CreatedAt = t.CreatedAt,
            HasPassword = t.Password != null,
            AccessType = t.AccessType,
            ShowCorrectAnswers = t.ShowCorrectAnswers
        });

        // Act
        await _controller.CreateTest(command);

        // Assert
        _testMock.Verify(x => x.Create(It.Is<Test>(t => 
            t.Tags.Count == 1 && 
            t.Tags.First().Name == "test"
        )), Times.Once);
        _testMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Test]
    public async Task UpdateTest_ValidatorFails_ReturnsBadRequest()
    {
        // Arrange
        SetupUser("10", "User");
        var test = new Test
        {
            Id = 1,
            UserId = 10,
            Name = "Test",
        };
        _testMock.Setup(x => x.GetByIdWithTagsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(test);

        var data = new QuestionData();
        var question = new ApiQuestion
        {
            OrderIndex = 1,
            TestId = 1,
            Type = QuestionType.MatchPairs,
            Description = "test",
            Data = data,
            CorrectData = data
        };
        
        var command = new CreateOrUpdateTestCommand 
        { 
            Questions = new List<ApiQuestion> { question },
            Tags = new List<string>()
        };
        
        _validatorMock.Setup(x => x.Validate(It.IsAny<QuestionData>(), It.IsAny<QuestionData>(), It.IsAny<QuestionType>()))
            .Throws(new Exception("Invalid question data"));

        // Act
        var result = await _controller.UpdateTestAsync(1, command);

        // Assert
        Assert.IsInstanceOf<BadRequestObjectResult>(result);
        Assert.That(((BadRequestObjectResult)result).Value, Is.EqualTo("One or more questions are invalid"));
    }

    private void SetupUser(string id, string role)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, id),
            new Claim(ClaimTypes.Role, role)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        _httpContext.User = new ClaimsPrincipal(identity);
    }
    
    private class MockSession : ISession
    {
        private readonly Dictionary<string, byte[]> _storage = new();
        public bool IsAvailable => true;
        public string Id => "test";
        public IEnumerable<string> Keys => _storage.Keys;
        public void Clear() => _storage.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _storage.Remove(key);
        public void Set(string key, byte[] value) => _storage[key] = value;
        public bool TryGetValue(string key, out byte[] value) => _storage.TryGetValue(key, out value);
    }
}