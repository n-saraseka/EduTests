using System.Security.Claims;
using EduTests.Controllers;
using EduTests.Database.Entities;
using EduTests.Database.Repositories.Interfaces;
using EduTests.Models;
using EduTests.Services;
using EduTests.ApiObjects;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace EduTests.Tests.ControllerTests;

[TestFixture]
public class ConstructorControllerTests
{
    private Mock<IUserRepository> _userRepo;
    private Mock<IEntityToDtoService> _dtoService;
    private Mock<ConstructorViewModel> _constructorViewModelMock;
    private ConstructorController _controller;
    private DefaultHttpContext _httpContext;

    [SetUp]
    public void Setup()
    {
        _userRepo = new Mock<IUserRepository>();
        _dtoService = new Mock<IEntityToDtoService>();
        _constructorViewModelMock = new Mock<ConstructorViewModel>();
        
        _controller = new ConstructorController(_userRepo.Object, _dtoService.Object);
        
        _httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = _httpContext
        };
    }

    [Test]
    public async Task BaseConstructor_ValidUser_ReturnsViewWithUserDto()
    {
        // Arrange
        int userId = 10;
        SetupUser(userId.ToString());

        var user = new User
        {
            Id = userId,
            Login = "test",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("test"),
            Username = "test"
        };
        
        var apiUser = new ApiUser
        {
            Id = userId,
            Username = user.Username
        };

        _userRepo.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _dtoService.Setup(x => x.UserEntityToDto(user))
            .Returns(apiUser);

        // Act
        var result = await _controller.BaseConstructor(_constructorViewModelMock.Object);

        // Assert
        var viewResult = (ViewResult)result;
        var model = (ConstructorViewModel)viewResult.Model;
        
        Assert.IsNotNull(model.User);
        Assert.That(model.User.Id, Is.EqualTo(userId));
        _userRepo.Verify(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task BaseConstructor_UserNotFoundInDb_ReturnsBadRequest()
    {
        // Arrange
        int userId = 10;
        SetupUser(userId.ToString());

        _userRepo.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _controller.BaseConstructor(_constructorViewModelMock.Object);

        // Assert
        Assert.IsInstanceOf<BadRequestObjectResult>(result);
        var badRequest = (BadRequestObjectResult)result;
        Assert.That(badRequest.Value, Is.EqualTo("User not found"));
    }

    [Test]
    public async Task BaseConstructor_InvalidUserIdClaim_ReturnsViewWithoutUser()
    {
        // Arrange
        SetupUser("not-a-number");

        // Act
        var result = await _controller.BaseConstructor(_constructorViewModelMock.Object);

        // Assert
        var viewResult = (ViewResult)result;
        var model = (ConstructorViewModel)viewResult.Model;
        
        Assert.IsNull(model.User);
        _userRepo.Verify(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
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