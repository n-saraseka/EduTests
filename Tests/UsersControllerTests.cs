using System.Security.Claims;
using EduTests.ApiObjects;
using EduTests.Commands.CommentCommands;
using EduTests.Commands.UserCommands;
using EduTests.Controllers.Api;
using EduTests.Database.Entities;
using EduTests.Database.Enums;
using EduTests.Database.Repositories.Interfaces;
using EduTests.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MockQueryable;
using Moq;
using NUnit.Framework;

namespace EduTests.Tests;

[TestFixture]
public class UsersControllerTests
{
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IBannedUserRepository> _bannedUserRepositoryMock;
    private Mock<ICommentRepository> _commentRepositoryMock;
    private Mock<IEntityToDtoService> _entityToDtoServiceMock;
    private Mock<IServiceProvider> _serviceProviderMock;
    private UsersController _controller;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _bannedUserRepositoryMock = new Mock<IBannedUserRepository>();
        _commentRepositoryMock = new Mock<ICommentRepository>();
        _entityToDtoServiceMock = new Mock<IEntityToDtoService>();
        _serviceProviderMock = new Mock<IServiceProvider>();

        _controller = new UsersController(
            _userRepositoryMock.Object,
            _bannedUserRepositoryMock.Object,
            _commentRepositoryMock.Object,
            _entityToDtoServiceMock.Object);

        var httpContext = new DefaultHttpContext();

        // For SignInAsync / SignOutAsync
        var authServiceMock = new Mock<IAuthenticationService>();
        _serviceProviderMock
            .Setup(s => s.GetService(typeof(IAuthenticationService)))
            .Returns(authServiceMock.Object);
        httpContext.RequestServices = _serviceProviderMock.Object;

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    [Test]
    public async Task Register_ValidCommand_ReturnsCreatedAtAction()
    {
        // Arrange
        var command = new RegistrationCommand { Username = "Test", Login = "test_login", Password = "password123" };
        _entityToDtoServiceMock.Setup(x => x.UserEntityToDto(It.IsAny<User>())).Returns(It.IsAny<ApiUser>());

        // Act
        var result = await _controller.RegisterAsync(command, CancellationToken.None);

        // Assert
        Assert.IsInstanceOf<CreatedAtActionResult>(result);
        _userRepositoryMock.Verify(x => x.Create(It.IsAny<User>()), Times.Once);
        _userRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Register_DuplicateLogin_ReturnsBadRequest()
    {
        // Arrange
        var command = new RegistrationCommand { Login = "existing" };
        _userRepositoryMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Duplicate"));

        // Act
        var result = await _controller.RegisterAsync(command, CancellationToken.None);

        // Assert
        Assert.IsInstanceOf<BadRequestObjectResult>(result);
    }

    [Test]
    public async Task GetUser_UserExists_ReturnsOk()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Login = "test",
            Username = "test",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("test"),
            Group = UserGroup.User
        };
        _userRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _entityToDtoServiceMock.Setup(x => x.UserEntityToDto(user)).Returns(It.IsAny<ApiUser>());

        // Act
        var result = await _controller.GetUserAsync(1);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
    }

    [Test]
    public async Task ChangeUsername_OwnProfile_UpdatesAndReturnsOk()
    {
        // Arrange
        var userId = 1;
        var login = "user_login";
        var user = new User
        {
            Id = 1,
            Login = "test",
            Username = "test",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("test"),
            Group = UserGroup.User
        };
        var command = new ChangeUsernameCommand { Username = "NewName" };

        SetupUserClaims(userId.ToString(), login, "User");
        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _entityToDtoServiceMock.Setup(x => x.UserEntityToDto(user)).Returns(It.IsAny<ApiUser>());

        // Act
        var result = await _controller.ChangeUsernameAsync(userId, command);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
        Assert.That(user.Username, Is.EqualTo("NewName"));
        _userRepositoryMock.Verify(x => x.Update(user), Times.Once);
    }

    [Test]
    public async Task ChangeUsername_OtherProfileAndNotAdmin_ReturnsForbid()
    {
        // Arrange
        SetupUserClaims("1", "attacker", "User");
        var user = new User
        {
            Id = 2,
            Login = "test",
            Username = "test",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("test"),
            Group = UserGroup.User
        };
        _userRepositoryMock.Setup(x => x.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.ChangeUsernameAsync(2, new ChangeUsernameCommand());

        // Assert
        Assert.IsInstanceOf<ForbidResult>(result);
    }

    [Test]
    public async Task GetProfileComments_PaginationWorks()
    {
        // Arrange
        var comments = new List<Comment>();
        for (int i = 0; i < 5; i++)
            comments.Add(new Comment
            {
                Id = i + 1,
                CommenterId = i + 1,
                Content = "test"
            });

        var mockQuery = comments.BuildMock();
        _commentRepositoryMock.Setup(x => x.GetProfileComments(It.IsAny<int>())).Returns(mockQuery);

        // Act
        var result = await _controller.GetProfileCommentsAsync(1, 1, 2, CancellationToken.None);

        // Assert
        var okResult = (OkObjectResult)result;
        dynamic response = okResult.Value;
        Assert.That(response.comments.Count, Is.EqualTo(2));
        Assert.That(response.pages, Is.EqualTo(3));
    }

    [Test]
    public async Task CreateProfileComment_Authenticated_ReturnsCreated()
    {
        // Arrange
        SetupUserClaims("1", "login", "User");
        var user = new User
        {
            Id = 2,
            Login = "test",
            Username = "test",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("test"),
            Group = UserGroup.User
        };
        var comment = new Comment
        {
            Id = 100,
            CommenterId = 1,
            Content = "test"
        };
        _userRepositoryMock.Setup(x => x.GetByIdAsync(2, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _commentRepositoryMock.Setup(x => x.GetWithLoadedCommenter(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(comment);
        _entityToDtoServiceMock.Setup(x => x.CommentEntityToDto(It.IsAny<Comment>())).Returns(It.IsAny<ApiComment>());

        // Act
        var result = await _controller.CreateProfileCommentAsync(2, new CreateCommentCommand { Content = "Hello" });

        // Assert
        Assert.IsInstanceOf<CreatedAtActionResult>(result);
        _commentRepositoryMock.Verify(x => x.Create(It.IsAny<Comment>()), Times.Once);
    }

    [Test]
    public async Task BanUser_AlreadyBanned_ReturnsBadRequest()
    {
        // Arrange
        SetupUserClaims("1", "admin", "Administrator");
        var user = new User
        {
            Id = 2,
            Login = "test",
            Username = "test",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("test"),
            Group = UserGroup.User
        };
        _userRepositoryMock.Setup(x => x.GetByIdAsync(2, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _bannedUserRepositoryMock.Setup(x => x.GetUsersActiveBanAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(It.IsAny<BannedUser>());

        // Act
        var result = await _controller.BanUserAsync(2, new BanUserCommand());

        // Assert
        Assert.IsInstanceOf<BadRequestObjectResult>(result);
        Assert.That(((BadRequestObjectResult)result).Value, Is.EqualTo("User has already been banned"));
    }

    private void SetupUserClaims(string id, string name, string role)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, id),
            new Claim(ClaimTypes.Name, name),
            new Claim(ClaimTypes.Role, role),
            new Claim(ClaimTypes.GivenName, "OldUsername")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);

    }
}