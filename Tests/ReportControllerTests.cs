using System.Security.Claims;
using EduTests.ApiObjects;
using EduTests.Commands.ReportCommands;
using EduTests.Controllers.Api;
using EduTests.Database.Entities;
using EduTests.Database.Enums;
using EduTests.Database.Repositories.Interfaces;
using EduTests.Services;
using Microsoft.AspNetCore.Mvc;
using MockQueryable;
using Moq;
using NUnit.Framework;

namespace EduTests.Tests;

[TestFixture]
public class ReportControllerTests
{
    // Mocks and the tested controller
    private Mock<IReportsRepository> _reportsRepositoryMock;
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IAnonymousUserRepository> _anonymousUserRepositoryMock;
    private Mock<ICommentRepository> _commentRepositoryMock;
    private Mock<ITestRepository> _testRepositoryMock;
    private Mock<IEntityToDtoService> _entityToDtoServiceMock;
    private ReportController _controller;

    [SetUp]
    public void Setup()
    {
        _reportsRepositoryMock = new Mock<IReportsRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _anonymousUserRepositoryMock = new Mock<IAnonymousUserRepository>();
        _commentRepositoryMock = new Mock<ICommentRepository>();
        _testRepositoryMock = new Mock<ITestRepository>();
        _entityToDtoServiceMock = new Mock<IEntityToDtoService>();

        _controller = new ReportController(
            _reportsRepositoryMock.Object,
            _userRepositoryMock.Object,
            _anonymousUserRepositoryMock.Object,
            _commentRepositoryMock.Object,
            _testRepositoryMock.Object,
            _entityToDtoServiceMock.Object);
        
        var user = new ClaimsPrincipal(new ClaimsIdentity());
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Test]
    public async Task CreateReport_EntityNotFound_ReturnsNotFound()
    {
        // Arrange
        var command = new CreateReportCommand { EntityType = EntityType.Test, EntityId = 999 };
        _testRepositoryMock.Setup(x => x.GetByIdAsync(command.EntityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Test?)null);

        // Act
        var result = await _controller.CreateReportAsync(command);

        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public async Task CreateReport_SelfReportingUser_ReturnsBadRequest()
    {
        // Arrange
        var userId = "10";
        var command = new CreateReportCommand { EntityType = EntityType.User, EntityId = 10 };

        var user = new User
        {
            Id = 10,
            Group = UserGroup.User,
            Login = "test",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("test"),
            Username = "test"
        };
        
        SetupUser(userId);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.CreateReportAsync(command);

        // Assert
        Assert.IsInstanceOf<BadRequestObjectResult>(result);
        var badRequest = result as BadRequestObjectResult;
        Assert.That(badRequest.Value, Is.EqualTo("Self reporting is not allowed"));
    }

    [Test]
    public async Task CreateReport_RecentReportExists_ReturnsBadRequest()
    {
        // Arrange
        var command = new CreateReportCommand { EntityType = EntityType.Test, EntityId = 1 };
        var existingReport = new Report
        {
            DateTime = DateTime.UtcNow.AddHours(-5),
            Text = "test",
            ReportStatus = ReportStatus.Pending
        }; // Less than a day ago

        var test = new Test
        {
            Id = 1,
            UserId = 1,
            Name = "test"
        };

        _testRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(test);
        _reportsRepositoryMock.Setup(x => x.GetByTestAndReporterIdAsync(1, It.IsAny<int?>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingReport);

        // Act
        var result = await _controller.CreateReportAsync(command);

        // Assert
        Assert.IsInstanceOf<BadRequestObjectResult>(result);
        Assert.That(((BadRequestObjectResult)result).Value, Is.EqualTo("The entity has been recently reported by user"));
    }

    [Test]
    public async Task CreateReport_ValidAuthenticatedRequest_ReturnsCreated()
    {
        // Arrange
        var userId = "5";
        var command = new CreateReportCommand { EntityType = EntityType.Comment, EntityId = 1, Text = "Spam" };

        var comment = new Comment
        {
            Id = 1, 
            CommenterId = 1, 
            Content = "test"
        };
        
        SetupUser(userId);
        _commentRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(comment);
        _entityToDtoServiceMock.Setup(x => x.ReportEntityToDto(It.IsAny<Report>()))
            .Returns(It.IsAny<ApiReport>());

        // Act
        var result = await _controller.CreateReportAsync(command);

        // Assert
        Assert.IsInstanceOf<CreatedResult>(result);
        _reportsRepositoryMock.Verify(x => x.Create(It.IsAny<Report>()), Times.Once);
        _reportsRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetLatestReports_InvalidPagination_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.GetLatestReportsAsync(0, 10, null);

        // Assert
        Assert.IsInstanceOf<BadRequestObjectResult>(result);
    }

    [Test]
    public async Task GetLatestReports_ReturnsCorrectPage()
    {
        // Arrange
        var reports = new List<Report>();
        for (int i = 0; i < 15; i++) reports.Add(new Report
        {
            Id = i,
            Text = "test",
            DateTime = DateTime.Now,
            ReportStatus = ReportStatus.Pending
        });
        
        var mock = reports.BuildMock();
        _reportsRepositoryMock.Setup(x => x.GetLatest()).Returns(mock);
        _entityToDtoServiceMock.Setup(x => x.ReportEntityToDto(It.IsAny<Report>())).Returns(It.IsAny<ApiReport>());

        // Act
        var result = await _controller.GetLatestReportsAsync(1, 10, null);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
        dynamic val = ((OkObjectResult)result).Value;
        Assert.That(val.reports.Count, Is.EqualTo(10));
        Assert.That(val.pages, Is.EqualTo(2));
    }

    [Test]
    public async Task ChangeStatus_ReportNotFound_ReturnsNotFound()
    {
        // Arrange
        _reportsRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Report?)null);

        // Act
        var result = await _controller.ChangeReportStatusAsync(1, new ChangeReportStatusCommand());

        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public async Task ChangeStatus_ValidRequest_UpdatesStatus()
    {
        // Arrange
        var report = new Report { Id = 1, Text = "test", ReportStatus = ReportStatus.Pending, DateTime = DateTime.Now };
        var command = new ChangeReportStatusCommand { ReportStatus = (int)ReportStatus.Accepted };

        _reportsRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(report);
        _entityToDtoServiceMock.Setup(x => x.ReportEntityToDto(report))
            .Returns(It.IsAny<ApiReport>());

        // Act
        var result = await _controller.ChangeReportStatusAsync(1, command);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
        Assert.That(report.ReportStatus, Is.EqualTo(ReportStatus.Accepted));
        _reportsRepositoryMock.Verify(x => x.Update(report), Times.Once);
        _reportsRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    private void SetupUser(string userId)
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);
    }

    private void SetupAnonymous(Guid anonId)
    {
        _controller.ControllerContext.HttpContext.Items["AnonymousId"] = anonId.ToString();
    }
}