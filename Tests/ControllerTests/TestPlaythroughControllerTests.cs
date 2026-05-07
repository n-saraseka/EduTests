using System.Security.Claims;
using EduTests.Controllers;
using EduTests.Database.Entities;
using EduTests.Database.Repositories.Interfaces;
using EduTests.Models;
using EduTests.Services;
using EduTests.ApiObjects;
using EduTests.Database.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace EduTests.Tests.ControllerTests;

[TestFixture]
public class TestPlaythroughControllerTests
{
    private Mock<ITestRepository> _testMock;
    private Mock<ITestCompletionRepository> _completionMock;
    private Mock<IUserAnswerRepository> _answerMock;
    private Mock<IQuestionRepository> _questionMock;
    private Mock<IUserRatingRepository> _ratingMock;
    private Mock<IBannedUserRepository> _bannedMock;
    private Mock<ICommentRepository> _commentMock;
    private Mock<IConfiguration> _configMock;
    private Mock<IEntityToDtoService> _entityToDtoServiceMock;
    private Mock<TestPlaythroughViewModel> _testPlaythroughMock;
    private Mock<TestResultViewModel> _testResultMock;
    private Mock<ResultDetailsViewModel> _resultDetailsMock;

    private TestPlaythroughController _controller;
    private DefaultHttpContext _httpContext;

    [SetUp]
    public void Setup()
    {
        _testMock = new Mock<ITestRepository>();
        _completionMock = new Mock<ITestCompletionRepository>();
        _answerMock = new Mock<IUserAnswerRepository>();
        _questionMock = new Mock<IQuestionRepository>();
        _ratingMock = new Mock<IUserRatingRepository>();
        _bannedMock = new Mock<IBannedUserRepository>();
        _commentMock = new Mock<ICommentRepository>();
        _configMock = new Mock<IConfiguration>();
        _entityToDtoServiceMock = new Mock<IEntityToDtoService>();
        _testPlaythroughMock = new Mock<TestPlaythroughViewModel>();
        _testResultMock = new Mock<TestResultViewModel>();
        _resultDetailsMock = new Mock<ResultDetailsViewModel>();
        
        _configMock.Setup(c => c["commentPageSize"]).Returns("10");
        _configMock.Setup(c => c["testStatisticsRows"]).Returns("20");

        _controller = new TestPlaythroughController(
            _testMock.Object, _commentMock.Object, _bannedMock.Object, _completionMock.Object, 
            _answerMock.Object, _questionMock.Object, _ratingMock.Object, 
            _configMock.Object, _entityToDtoServiceMock.Object);

        _httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext { HttpContext = _httpContext };
    }

    [Test]
    public async Task TestPlaythrough_AnonymousUser_ValidId_ReturnsView()
    {
        // Arrange
        var anonId = Guid.NewGuid();
        _httpContext.Items["AnonymousId"] = anonId.ToString();
        
        var startedAt = DateTime.UtcNow;
        var completion = new TestCompletion { 
            Id = 1, 
            AnonymousUserId = anonId, 
            TestId = 10,
            StartedAt = startedAt
        };

        _completionMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(completion);
        
        var test = new Test {
            Id = 10,
            UserId = 1,
            Name = "test",
            Questions = new List<Question> {
                new() { Id = 1, 
                    TestId = 10, 
                    OrderIndex = 1,
                    Description = "test",
                    Type = QuestionType.MatchPairs,
                    CreatedAt = startedAt.AddMinutes(-5),
                    UpdatedAt = startedAt.AddMinutes(-5), 
                    Data = new(),
                    CorrectData = new() }
            }
        };
        _testMock.Setup(x => x.GetByIdWithExtendedDataAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(test);

        _entityToDtoServiceMock.Setup(x => x.CompletionEntityToDto(It.IsAny<TestCompletion>(), null, null))
            .Returns((TestCompletion tc, List<UserAnswer>? ua, List<Question>? q) => new ApiCompletion
            {
                Id = tc.Id,
                StartedAt = tc.StartedAt,
                TestId = tc.TestId
            });
        _entityToDtoServiceMock.Setup(x => x.TestEntityToDto(It.IsAny<Test>())).Returns((Test t) => new ApiTest
        {
            Id = t.Id,
            UpdatedAt = t.UpdatedAt,
            CreatedAt = t.CreatedAt,
            Name = t.Name,
            HasPassword = t.Password != null,
            ShowCorrectAnswers = t.ShowCorrectAnswers,
            AccessType = t.AccessType
        });
        _entityToDtoServiceMock.Setup(x => x.QuestionEntityToDto(It.IsAny<Question>())).Returns((Question q) => new ApiQuestion
        {
            Id = q.Id,
            OrderIndex = q.OrderIndex,
            Description = q.Description,
            Data = q.Data,
            CorrectData = q.CorrectData,
            Type = q.Type,
            TestId = q.TestId
        });
        _answerMock.Setup(x => x.GetByCompletionIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserAnswer>());

        // Act
        var result = await _controller.TestPlaythrough(1, _testPlaythroughMock.Object);

        // Assert
        Assert.IsInstanceOf<ViewResult>(result);
        _completionMock.Verify(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task TestResult_NotCompleted_ReturnsBadRequest()
    {
        // Arrange
        var startedAt = DateTime.UtcNow;
        var completion = new TestCompletion { 
            Id = 1,
            TestId = 10,
            StartedAt = startedAt
        };
        _completionMock.Setup(x => x.GetWithExtendedDataAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(completion);

        // Act
        var result = await _controller.TestResult(1, _testResultMock.Object);

        // Assert
        Assert.IsInstanceOf<BadRequestObjectResult>(result);
    }

    [Test]
    public async Task ResultDetails_ForbiddenForOtherUsers()
    {
        // Arrange
        SetupUser("999");
        var completion = new TestCompletion { 
            Id = 1, 
            UserId = 1,
            TestId = 2,
            StartedAt = DateTime.UtcNow.AddMinutes(-10),
            CompletedAt = DateTime.UtcNow,
            Test = new Test
            {
                Id = 2,
                UserId = 20,
                Name = "test"
            }
        };

        _completionMock.Setup(x => x.GetWithExtendedDataAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(completion);

        // Act
        var result = await _controller.ResultDetails(1, _resultDetailsMock.Object);

        // Assert
        Assert.IsInstanceOf<ForbidResult>(result);
    }
    
    private void SetupUser(string id)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, id)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        _httpContext.User = new ClaimsPrincipal(identity);
    }
}