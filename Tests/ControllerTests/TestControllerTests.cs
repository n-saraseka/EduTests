using System.Security.Claims;
using EduTests.Controllers;
using EduTests.Database.Entities;
using EduTests.Database.Enums;
using EduTests.Database.Repositories.Interfaces;
using EduTests.Models;
using EduTests.Services;
using EduTests.ApiObjects;
using Microsoft.AspNetCore.Mvc;
using MockQueryable;
using Moq;
using NUnit.Framework;

namespace EduTests.Tests.ControllerTests;

[TestFixture]
public class TestControllerTests
{
    private Mock<ITestRepository> _testMock;
    private Mock<IUserRepository> _userMock;
    private Mock<ICommentRepository> _commentMock;
    private Mock<IBannedUserRepository> _bannedMock;
    private Mock<ITestCompletionRepository> _completionMock;
    private Mock<IUserAnswerRepository> _answerMock;
    private Mock<IQuestionRepository> _questionMock;
    private Mock<IConfiguration> _configMock;
    private Mock<ITestStatsService> _statsServiceMock;
    private Mock<IEntityToDtoService> _entityToDtoServiceMock;
    private Mock<ConstructorViewModel> _constructorMock;
    private Mock<TestPageViewModel> _testPageMock;
    private Mock<TestStatisticsViewModel> _testStatisticsMock;
    
    private TestController _controller;
    private DefaultHttpContext _httpContext;

    [SetUp]
    public void Setup()
    {
        _testMock = new Mock<ITestRepository>();
        _userMock = new Mock<IUserRepository>();
        _commentMock = new Mock<ICommentRepository>();
        _bannedMock = new Mock<IBannedUserRepository>();
        _completionMock = new Mock<ITestCompletionRepository>();
        _answerMock = new Mock<IUserAnswerRepository>();
        _questionMock = new Mock<IQuestionRepository>();
        _configMock = new Mock<IConfiguration>();
        _statsServiceMock = new Mock<ITestStatsService>();
        _entityToDtoServiceMock = new Mock<IEntityToDtoService>();
        _constructorMock = new Mock<ConstructorViewModel>();
        _testPageMock = new Mock<TestPageViewModel>();
        _testStatisticsMock = new Mock<TestStatisticsViewModel>();

        _configMock.Setup(c => c["commentPageSize"]).Returns("10");
        _configMock.Setup(c => c["testStatisticsRows"]).Returns("20");

        _controller = new TestController(
            _testMock.Object, _userMock.Object, _commentMock.Object,
            _bannedMock.Object, _completionMock.Object, _answerMock.Object,
            _questionMock.Object, _configMock.Object, _statsServiceMock.Object, _entityToDtoServiceMock.Object);

        _httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext { HttpContext = _httpContext };
    }

    [Test]
    public async Task Constructor_ReturnsOnlyLatestVersionQuestions()
    {
        // Arrange
        SetupUser("1");
        var oldDate = DateTime.UtcNow.AddDays(-1);
        var newDate = DateTime.UtcNow;

        var data = new QuestionData();

        var test = new Test { 
            Id = 1, 
            UserId = 1,
            Name = "test",
            Questions = new List<Question> {
                new() { 
                    Id = 1, 
                    UpdatedAt = oldDate, 
                    OrderIndex = 1, 
                    Data = data, 
                    CorrectData = data, 
                    CreatedAt = oldDate, 
                    TestId = 1, 
                    Type = QuestionType.Sequence,
                    Description = "test"
                },
                new() { 
                    Id = 2, 
                    UpdatedAt = newDate, 
                    OrderIndex = 1, 
                    Data = data, 
                    CorrectData = data, 
                    CreatedAt = newDate, 
                    TestId = 1, 
                    Type = QuestionType.Sequence,
                    Description = "test"
                },
            },
            Results = new List<TestResult>()
        };

        var user = new User
        {
            Id = 1,
            Login = "test",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("test"),
            Group = UserGroup.User,
            RegistrationDate = DateTime.Now,
            Username = "test"
        };
        
        _testMock.Setup(x => x.GetByIdWithExtendedDataAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(test);
        _userMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _entityToDtoServiceMock.Setup(x => x.TestEntityToDto(test)).Returns((Test t) => new ApiTest
        {
            Id = t.Id,
            UpdatedAt = t.UpdatedAt,
            CreatedAt = t.CreatedAt,
            ShowCorrectAnswers = t.ShowCorrectAnswers,
            Name = t.Name,
            AccessType = t.AccessType,
            HasPassword = t.Password != null
        });
        _entityToDtoServiceMock.Setup(x => x.QuestionEntityToDto(It.IsAny<Question>())).Returns((Question q) => new ApiQuestion
        {
            Id = q.Id,
            OrderIndex = q.OrderIndex,
            Data = q.Data,
            CorrectData = q.CorrectData,
            Description = q.Description,
            TestId = q.TestId,
            Type = q.Type
        });

        // Act
        var result = await _controller.Constructor(1, _constructorMock.Object);

        // Assert
        var model = (ConstructorViewModel)((ViewResult)result).Model;
        Assert.That(model.Test.Questions.Count, Is.EqualTo(1));
        Assert.That(model.Test.Questions.First().Id, Is.EqualTo(2));
    }

    [Test]
    public async Task TestPage_PasswordRequiredNoCookie_SetsCanStartTestFalse()
    {
        // Arrange
        var test = new Test { 
            Id = 1, 
            UserId = 1,
            Name = "test",
            Password = BCrypt.Net.BCrypt.HashPassword("test"),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        var apiTest = new ApiTest
        {
            Id = test.Id,
            Name = test.Name,
            CreatedAt = test.CreatedAt,
            UpdatedAt = test.UpdatedAt,
            HasPassword = test.Password != null,
            ShowCorrectAnswers = test.ShowCorrectAnswers,
            AccessType = test.AccessType
        };
        
        _testMock.Setup(x => x.GetByIdWithTagsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(test);
        _entityToDtoServiceMock.Setup(x => x.TestEntityToDto(test)).Returns(apiTest);
        _statsServiceMock.Setup(x => x.GetTestStatsAsync(apiTest, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApiTest t, CancellationToken ct) => new ApiTest
            {
                Id = t.Id,
                Name = t.Name,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                HasPassword = t.HasPassword,
                ShowCorrectAnswers = t.ShowCorrectAnswers,
                AccessType = t.AccessType,
                CompletionCount = 0,
                Rating = 0
            });

        // Act
        var result = await _controller.TestPage(1, _testPageMock.Object);

        // Assert
        var model = (TestPageViewModel)((ViewResult)result).Model;
        Assert.That(model.CanStartTest, Is.False);
    }

    [Test]
    public async Task TestStats_CalculatesMedianAndQuartilesCorrectly()
    {
        // Arrange
        SetupUser("1");
        var latestVersion = DateTime.UtcNow;
        var test = new Test { 
            Id = 1, 
            UserId = 1,
            Name = "test",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        var apiTest = new ApiTest
        {
            Id = test.Id,
            Name = test.Name,
            CreatedAt = test.CreatedAt,
            UpdatedAt = test.UpdatedAt,
            HasPassword = test.Password != null,
            ShowCorrectAnswers = test.ShowCorrectAnswers,
            AccessType = test.AccessType
        };
        
        _testMock.Setup(x => x.GetByIdWithTagsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(test);
        _entityToDtoServiceMock.Setup(x => x.TestEntityToDto(test)).Returns(apiTest);
        
        var completions = new List<TestCompletion> {
            new() { Id = 1, TestId = 1, StartedAt = latestVersion, CompletedAt = latestVersion.AddMinutes(10) },
            new() { Id = 2, TestId = 1, StartedAt = latestVersion, CompletedAt = latestVersion.AddMinutes(20) },
            new() { Id = 3, TestId = 1, StartedAt = latestVersion, CompletedAt = latestVersion.AddMinutes(30) },
            new() { Id = 4, TestId = 1, StartedAt = latestVersion, CompletedAt = latestVersion.AddMinutes(40) }
        }.BuildMock();

        var question = new Question
        {
            Id = 1,
            TestId = 1,
            OrderIndex = 1,
            Data = new QuestionData(),
            CorrectData = new QuestionData(),
            Description = "test",
            Type = QuestionType.MatchPairs,
            UpdatedAt = latestVersion,
        };
        
        _completionMock.Setup(x => x.GetByTestId(1)).Returns(completions);
        _questionMock.Setup(x => x.GetByTestIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Question> { question });

        var apiComps = completions.Select(c => new ApiCompletion
        {
            CompletedAt = c.CompletedAt,
            StartedAt = c.StartedAt,
            Id = c.Id,
            TestId = c.TestId,
        }).ToList();
        apiComps[0].CompletionPercentage = 20;
        apiComps[1].CompletionPercentage = 50;
        apiComps[2].CompletionPercentage = 60;
        apiComps[3].CompletionPercentage = 70;
        
        var answers = new Dictionary<int, List<UserAnswer>> {
            { 1, new List<UserAnswer>() },
            { 2, new List<UserAnswer>() },
            { 3, new List<UserAnswer>() },
            { 4, new List<UserAnswer>() }
        };

        _answerMock.Setup(x => x.GetByCompletionIdsAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(answers);
        
        _entityToDtoServiceMock.Setup(x => x.CompletionEntityToDto(It.IsAny<TestCompletion>(), It.IsAny<List<UserAnswer>>(), It.IsAny<List<Question>>()))
            .Returns((TestCompletion tc, List<UserAnswer> ua, List<Question> q) => apiComps.First(c => c.Id == tc.Id));

        // Act
        var result = await _controller.TestStats(1, _testStatisticsMock.Object);

        // Assert
        var model = (TestStatisticsViewModel)((ViewResult)result).Model;
        Assert.That(model.CompletionStats.MedianPercentage, Is.EqualTo(55));
        Assert.That(model.CompletionStats.MaxPercentage, Is.EqualTo(70));
        Assert.That(model.CompletionStats.MinPercentage, Is.EqualTo(20));
        Assert.That(model.CompletionStats.MinTime, Is.EqualTo(TimeSpan.FromMinutes(10)));
        Assert.That(model.CompletionStats.MaxTime, Is.EqualTo(TimeSpan.FromMinutes(40)));
        Assert.That(model.CompletionStats.InterQuartileAverageTime, Is.EqualTo(TimeSpan.FromMinutes(25)));
    }

    private void SetupUser(string id)
    {
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, id) };
        _httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
    }
}