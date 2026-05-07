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
public class UserControllerTests
{
    private Mock<IUserRepository> _userMock;
    private Mock<ICommentRepository> _commentMock;
    private Mock<IReportsRepository> _reportsMock;
    private Mock<IBannedUserRepository> _bannedMock;
    private Mock<ITestRepository> _testMock;
    private Mock<ITestStatsService> _statsServiceMock;
    private Mock<IEntityToDtoService> _entityToDtoServiceMock;
    private Mock<IConfiguration> _configMock;
    private Mock<ProfileViewModel> _profileViewModelMock;
    private Mock<UserSettingsViewModel> _userSettingsViewModelMock;

    private UserController _controller;
    private DefaultHttpContext _httpContext;

    [SetUp]
    public void Setup()
    {
        _userMock = new Mock<IUserRepository>();
        _commentMock = new Mock<ICommentRepository>();
        _reportsMock = new Mock<IReportsRepository>();
        _bannedMock = new Mock<IBannedUserRepository>();
        _testMock = new Mock<ITestRepository>();
        _statsServiceMock = new Mock<ITestStatsService>();
        _entityToDtoServiceMock = new Mock<IEntityToDtoService>();
        _configMock = new Mock<IConfiguration>();
        _profileViewModelMock = new Mock<ProfileViewModel>();
        _userSettingsViewModelMock = new Mock<UserSettingsViewModel>();
        
        _configMock.Setup(c => c["commentPageSize"]).Returns("5");
        _configMock.Setup(c => c["testsProfilePageSize"]).Returns("10");
        _configMock.Setup(c => c["modTablePageSize"]).Returns("20");

        _controller = new UserController(
            _userMock.Object, _commentMock.Object, _reportsMock.Object,
            _bannedMock.Object, _testMock.Object, _statsServiceMock.Object,
            _entityToDtoServiceMock.Object, _configMock.Object);

        _httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext { HttpContext = _httpContext };
    }

    [Test]
    public async Task Profile_UserNotFound_ReturnsNotFound()
    {
        _userMock.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await _controller.Profile(1, _profileViewModelMock.Object);

        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public async Task Profile_ValidUser_PopulatesViewModelWithStats()
    {
        // Arrange
        int targetId = 1;
        var user = new User
        {
            Id = targetId,
            Login = "test",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("test"),
            Username = "test",
            RegistrationDate = DateTime.Now,
            Group = UserGroup.User
        };
        _userMock.Setup(x => x.GetByIdAsync(targetId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _entityToDtoServiceMock.Setup(x => x.UserEntityToDto(user)).Returns((User u) =>
            new ApiUser
            {
                Id = u.Id,
                Username = u.Username,
                RegistrationDate = u.RegistrationDate
            }
        );
        
        var comments = new List<Comment>().BuildMock();
        _commentMock.Setup(x => x.GetProfileComments(targetId)).Returns(comments);

        var test = new Test
        {
            Id = 10,
            UserId = targetId,
            Name = "test"
        };

        var tests = new List<Test> { test }.BuildMock();
        _testMock.Setup(x => x.GetByUserId(targetId)).Returns(tests);

        var apiTest = new ApiTest
        {
            Id = 10,
            Name = "test",
            AccessType = AccessType.Public,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            HasPassword = true,
            ShowCorrectAnswers = false
        };
        var apiTests = new List<ApiTest> { apiTest };
        _entityToDtoServiceMock.Setup(x => x.TestEntityToDto(It.IsAny<Test>())).Returns(apiTests[0]);
        _statsServiceMock.Setup(x => x.GetTestsStatsAsync(It.IsAny<List<ApiTest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiTests);

        // Act
        var result = await _controller.Profile(targetId, _profileViewModelMock.Object);

        // Assert
        var viewResult = (ViewResult)result;
        var model = (ProfileViewModel)viewResult.Model;

        Assert.That(model.User.Id, Is.EqualTo(targetId));
        Assert.That(model.Tests.Count, Is.EqualTo(1));
        _statsServiceMock.Verify(x => x.GetTestsStatsAsync(It.IsAny<List<ApiTest>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Settings_WrongUser_ReturnsForbid()
    {
        // Arrange
        SetupUser("1", "User");
        var user = new User
        {
            Id = 99,
            Login = "test",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("test"),
            Username = "test",
            RegistrationDate = DateTime.Now,
            Group = UserGroup.User
        };
        _userMock.Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        // Act
        var result = await _controller.Settings(99, _userSettingsViewModelMock.Object);

        // Assert
        Assert.IsInstanceOf<ForbidResult>(result);
    }

    [Test]
    public async Task Settings_Moderator_PopulatesModSections()
    {
        // Arrange
        SetupUser("1", "Moderator");
        var user = new User
        {
            Id = 1,
            Login = "test",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("test"),
            Username = "test",
            RegistrationDate = DateTime.Now,
            Group = UserGroup.Moderator
        };
        _userMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _entityToDtoServiceMock.Setup(x => x.UserEntityToDto(user)).Returns((User u) =>
            new ApiUser
            {
                Id = u.Id,
                Username = u.Username,
                RegistrationDate = u.RegistrationDate
            }
        );
        
        var report = new Report
        {
            Id = 1,
            ReportingUserId = 2,
            ReportStatus = ReportStatus.Pending,
            Text = "test",
            DateTime = DateTime.Now
        };
        
        var reports = new List<Report> { report }.BuildMock();
        _reportsMock.Setup(x => x.GetLatest()).Returns(reports);

        var ban = new BannedUser
        {
            Id = 1,
            BannedById = 1,
            UserBannedId = 2,
            BanReason = "test",
            DateBanned = DateTime.Now
        };

        var bans = new List<BannedUser> { ban }.BuildMock();
        _bannedMock.Setup(x => x.GetLatestBans(true)).Returns(bans);

        var test = new Test
        {
            Id = 1,
            UserId = 1,
            Name = "test"
        };

        var tests = new List<Test> { test }.BuildMock();
        _testMock.Setup(x => x.GetByUserId(1)).Returns(tests);

        // Act
        var result = await _controller.Settings(1, _userSettingsViewModelMock.Object);

        // Assert
        var viewResult = (ViewResult)result;
        var model = (UserSettingsViewModel)viewResult.Model;

        Assert.That(model.ReportPages, Is.EqualTo(1));
        Assert.That(model.BanPages, Is.EqualTo(1));
        _reportsMock.Verify(x => x.GetLatest(), Times.Once);
    }

    [Test]
    public async Task Settings_RegularUser_DoesNotSeeModSections()
    {
        // Arrange
        SetupUser("1", "User");
        var user = new User
        {
            Id = 1,
            Login = "test",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("test"),
            Username = "test",
            RegistrationDate = DateTime.Now,
            Group = UserGroup.User
        };
        _userMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _entityToDtoServiceMock.Setup(x => x.UserEntityToDto(user)).Returns((User u) =>
            new ApiUser
            {
                Id = u.Id,
                Username = u.Username,
                RegistrationDate = u.RegistrationDate
            }
        );

        // Act
        var result = await _controller.Settings(1, _userSettingsViewModelMock.Object);

        // Assert
        var viewResult = (ViewResult)result;
        var model = (UserSettingsViewModel)viewResult.Model;
        
        Assert.IsNull(model.Reports);
        Assert.IsNull(model.Bans);
        _reportsMock.Verify(x => x.GetLatest(), Times.Never);
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
}