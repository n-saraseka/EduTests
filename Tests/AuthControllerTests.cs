using System.Security.Claims;
using EduTests.ApiObjects;
using EduTests.Commands.AuthCommands;
using NUnit.Framework;
using EduTests.Controllers.Api;
using EduTests.Database.Entities;
using EduTests.Database.Enums;
using EduTests.Database.Repositories.Interfaces;
using EduTests.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EduTests.Tests;

[TestFixture]
public class AuthControllerTests
{
    // Mocks and the tested controller
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IEntityToDtoService> _entityToDtoServiceMock;
    private AuthController _controller;
    
    // Test data
    private User _testUser;
    private ApiUser _testUserDto;
    
    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _entityToDtoServiceMock = new Mock<IEntityToDtoService>();
        
        var httpContextMock = new Mock<HttpContext>();
        var responseMock = new Mock<HttpResponse>();
        var authenticationServiceMock = new Mock<IAuthenticationService>();
        
        httpContextMock.Setup(x => x.Response).Returns(responseMock.Object);
        httpContextMock.Setup(x => x.RequestServices.GetService(typeof(IAuthenticationService)))
            .Returns(authenticationServiceMock.Object);
        
        authenticationServiceMock
            .Setup(x => x.SignInAsync(
                httpContextMock.Object,
                CookieAuthenticationDefaults.AuthenticationScheme,
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask)
            .Verifiable();
        
        _controller = new AuthController(_userRepositoryMock.Object, _entityToDtoServiceMock.Object);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContextMock.Object
        };
        
        var registrationDate = DateTime.Now;

        _testUser = new User
        {
            Login = "test",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("test"),
            RegistrationDate = registrationDate,
            Username = "test",
            Group = UserGroup.User
        };

        _testUserDto = new ApiUser
        {
            Username = "test",
            Id = 1,
            RegistrationDate = registrationDate,
            Group = UserGroup.User
        };
    }

    [Test]
    public async Task Login_UserDoesNotExist_ReturnsBadRequest()
    {
        // Arrange
        var command = new LoginCommand
        {
            Login = "login",
            Password = "password"
        };
        
        // Act
        var result = await _controller.LoginAsync(command, CancellationToken.None);
        
        // Assert
        Assert.IsInstanceOf<BadRequestObjectResult>(result);
        
        _userRepositoryMock.Verify(x => x.GetByLoginAsync(command.Login, CancellationToken.None), Times.Once);
        _entityToDtoServiceMock.Verify(x => x.UserEntityToDto(It.IsAny<User>()), Times.Never);
    }

    [Test]
    public async Task Login_PasswordIsIncorrect_ReturnsBadRequest()
    {
        // Arrange
        var command = new LoginCommand
        {
            Login = "test",
            Password = "password"
        };
        
        _userRepositoryMock.Setup(x => x.GetByLoginAsync(command.Login, CancellationToken.None))
            .ReturnsAsync(_testUser);
        
        // Act
        
        var result = await _controller.LoginAsync(command, CancellationToken.None);
        
        Assert.IsInstanceOf<BadRequestObjectResult>(result);
        
        _userRepositoryMock.Verify(x => x.GetByLoginAsync(command.Login, CancellationToken.None), Times.Once);
        _entityToDtoServiceMock.Verify(x => x.UserEntityToDto(It.IsAny<User>()), Times.Never);
    }

    [Test]
    public async Task Login_PasswordIsCorrect_ReturnsOkWithDto()
    {
        // Arrange
        var command = new LoginCommand
        {
            Login = "test",
            Password = "test"
        };

        _userRepositoryMock.Setup(x => x.GetByLoginAsync(command.Login, CancellationToken.None))
            .ReturnsAsync(_testUser);
        _entityToDtoServiceMock.Setup(x => x.UserEntityToDto(It.IsAny<User>())).Returns(_testUserDto)
            .Callback<User>(user =>
            {
                Assert.That(user, Is.SameAs(_testUser));
            });
        
        // Act
        var result = await _controller.LoginAsync(command, CancellationToken.None);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            
            Assert.That(okResult, Is.Not.Null);
            Assert.IsInstanceOf<ApiUser>(okResult.Value);
            
            var actualDto = (ApiUser)okResult.Value;
            Assert.That(actualDto, Is.Not.Null);
            Assert.That(actualDto.Username, Is.EqualTo(_testUserDto.Username));
            Assert.That(actualDto.Group, Is.EqualTo(_testUserDto.Group));
            Assert.That(actualDto.Id, Is.EqualTo(_testUserDto.Id));
            Assert.That(actualDto.RegistrationDate, Is.EqualTo(_testUserDto.RegistrationDate));
            Assert.That(actualDto.AvatarUrl, Is.EqualTo(_testUserDto.AvatarUrl));
            Assert.That(actualDto.Description, Is.EqualTo(_testUserDto.Description));
        });
        
        _userRepositoryMock.Verify(x => x.GetByLoginAsync(command.Login, CancellationToken.None), Times.Once);
        _entityToDtoServiceMock.Verify(x => x.UserEntityToDto(_testUser), Times.Once);
    }
}