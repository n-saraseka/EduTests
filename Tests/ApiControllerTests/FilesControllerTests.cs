using System.Security.Claims;
using EduTests.Controllers.Api;
using EduTests.Database.Entities;
using EduTests.Database.Enums;
using EduTests.Database.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Moq;
using NUnit.Framework;

namespace EduTests.Tests.ApiControllerTests;

[TestFixture]
public class FilesControllerTests
{
    // Mocks and the tested controller
    private Mock<IWebHostEnvironment> _envMock;
    private Mock<FileExtensionContentTypeProvider> _fileExtensionContentTypeProviderMock;
    private Mock<ITestRepository> _testRepositoryMock;
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<HttpContext> _httpContextMock;
    private FilesController _controller;
    
    // Test Data
    private ClaimsPrincipal _authorizedUser;
    private ClaimsPrincipal _unauthorizedUser;
    private ClaimsPrincipal _wrongUser;
    private ClaimsPrincipal _moderator;
    private Test _test;
    private User _user;

    [SetUp]
    public void Setup()
    {
        _envMock = new Mock<IWebHostEnvironment>();
        _envMock.SetupProperty(x => x.WebRootPath, "a");
        _fileExtensionContentTypeProviderMock = new Mock<FileExtensionContentTypeProvider>();
        _testRepositoryMock = new Mock<ITestRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        
        var createdDate = DateTime.Now;

        _test = new Test
        {
            AccessType = AccessType.Public,
            CreatedAt = createdDate,
            UpdatedAt = createdDate,
            Id = 1,
            UserId = 1,
            Name = "Test",
        };

        _user = new User
        {
            Id = 1,
            Username = "test",
            Login = "test",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("test"),
            RegistrationDate = createdDate,
            Group = UserGroup.User
        };
        
        SetupAllClaims();
        
        _httpContextMock = new Mock<HttpContext>();

        _controller = new FilesController(_envMock.Object, _fileExtensionContentTypeProviderMock.Object,
            _testRepositoryMock.Object, _userRepositoryMock.Object);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = _httpContextMock.Object
        };
    }

    private void SetupAllClaims()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Name, "test"),
            new Claim(ClaimTypes.Role, "User")
        };
        _authorizedUser = SetupClaimsPrincipal(claims);

        claims = new List<Claim>();
        _unauthorizedUser = SetupClaimsPrincipal(claims);
        
        claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "2"),
            new Claim(ClaimTypes.Name, "test2"),
            new Claim(ClaimTypes.Role, "User")
        };
        
        _wrongUser = SetupClaimsPrincipal(claims);
        
        claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "2"),
            new Claim(ClaimTypes.Name, "test2"),
            new Claim(ClaimTypes.Role, "Moderator")
        };
        
        _moderator = SetupClaimsPrincipal(claims);
    }

    private ClaimsPrincipal SetupClaimsPrincipal(List<Claim> claims)
    {
        var identity = new ClaimsIdentity(claims, "Cookies");
        return new ClaimsPrincipal(identity);
    }

    [Test]
    public async Task UploadTestThumbnail_NoTest_ReturnsNotFound()
    {
        // Arrange
        var id = 1;
        _httpContextMock.SetupGet(x => x.User).Returns(_authorizedUser);
        
        _testRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Test?)null);
        
        // Act
        var result = await _controller.UploadTestThumbnailAsync(id, null, CancellationToken.None);
        
        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
        _testRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
    }
    
    [Test]
    public async Task UploadTestThumbnail_NoUser_ReturnsUnauthorized()
    {
        // Arrange
        var id = 1;
        _httpContextMock.Setup(x => x.User).Returns(_unauthorizedUser);
        
        _testRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(_test);
        
        // Act
        var result = await _controller.UploadTestThumbnailAsync(id, null, CancellationToken.None);
        
        // Assert
        Assert.IsInstanceOf<UnauthorizedResult>(result);
        _testRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
    }

    [Test]
    public async Task UploadTestThumbnail_UserIsNotAuthor_ReturnsForbid()
    {
        // Arrange
        var id = 1;
        _httpContextMock.Setup(x => x.User).Returns(_wrongUser);
        
        _testRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(_test);
        
        // Act
        var result = await _controller.UploadTestThumbnailAsync(id, null, CancellationToken.None);
        
        // Assert
        Assert.IsInstanceOf<ForbidResult>(result);
        _testRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
    }
    
    [Test]
    public async Task UploadTestThumbnail_FileDoesntExist_ReturnsBadRequest()
    {
        // Arrange
        var id = 1;
        _httpContextMock.Setup(x => x.User).Returns(_authorizedUser);
        
        _testRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(_test);
        
        // Act
        var result = await _controller.UploadTestThumbnailAsync(id, null, CancellationToken.None);
        
        // Assert
        Assert.IsInstanceOf<BadRequestResult>(result);
        _testRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
    }
    
    [Test]
    public async Task UploadTestThumbnail_FileLengthIsZero_ReturnsBadRequest()
    {
        // Arrange
        var id = 1;
        _httpContextMock.Setup(x => x.User).Returns(_authorizedUser);
        
        //Setup mock file using a memory stream
        string? content = null;
        var fileName = "test.png";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;

        //create FormFile with desired data
        IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);
        
        _testRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(_test);
        
        // Act
        var result = await _controller.UploadTestThumbnailAsync(id, file, CancellationToken.None);
        
        // Assert
        Assert.IsInstanceOf<BadRequestResult>(result);
        _testRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
    }
    
    [Test]
    public async Task UploadTestThumbnail_FileTypeIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        var id = 1;
        _httpContextMock.Setup(x => x.User).Returns(_authorizedUser);
        
        //Setup mock file using a memory stream
        var content = "Test";
        var fileName = "test.png";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;

        //create FormFile with desired data
        IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);
        
        _testRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(_test);
        
        // Act
        var result = await _controller.UploadTestThumbnailAsync(id, file, CancellationToken.None);
        
        // Assert
        Assert.IsInstanceOf<BadRequestResult>(result);
        _testRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
    }
    
    [Test]
    public async Task UploadTestThumbnail_FileTypeIsValid_ReturnsOk()
    {
        // Arrange
        var id = 1;
        _httpContextMock.Setup(x => x.User).Returns(_authorizedUser);
        
        //Setup mock file using a memory stream
        var content = "Test";
        var fileName = "test.png";
        var pngSignature = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        
        stream.Write(pngSignature, 0, pngSignature.Length);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;

        //create FormFile with desired data
        IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);
        
        _testRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(_test);
        _testRepositoryMock.Setup(x => x.Update(_test));
        _testRepositoryMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        
        // Act
        var result = await _controller.UploadTestThumbnailAsync(id, file, CancellationToken.None);
        
        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
        _testRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
        _testRepositoryMock.Verify(x => x.Update(_test), Times.Once);
        _testRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Test]
    public async Task UploadTestThumbnail_FileIsValidAndUserIsMod_ReturnsOk()
    {
        // Arrange
        var id = 1;
        _httpContextMock.Setup(x => x.User).Returns(_moderator);
        
        //Setup mock file using a memory stream
        var content = "Test";
        var fileName = "test.png";
        var pngSignature = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        
        stream.Write(pngSignature, 0, pngSignature.Length);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;

        //create FormFile with desired data
        IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);
        
        _testRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(_test);
        _testRepositoryMock.Setup(x => x.Update(_test));
        _testRepositoryMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        
        // Act
        var result = await _controller.UploadTestThumbnailAsync(id, file, CancellationToken.None);
        
        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
        _testRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
        _testRepositoryMock.Verify(x => x.Update(_test), Times.Once);
        _testRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetTestThumbnail_TestNotFound_ReturnsNotFound()
    {
        // Arrange
        var id = 1;
        _httpContextMock.SetupGet(x => x.User).Returns(_authorizedUser);
        
        _testRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Test?)null);
        
        // Act
        var result = await _controller.GetTestThumbnailAsync(id, CancellationToken.None);
        
        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
        _testRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
    }
    
    [Test]
    public async Task UploadUserAvatar_NoUser_ReturnsNotFound()
    {
        // Arrange
        var id = 1;
        _httpContextMock.SetupGet(x => x.User).Returns(_authorizedUser);
        
        _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((User?)null);
        
        // Act
        var result = await _controller.UploadUserAvatarAsync(id, null, CancellationToken.None);
        
        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
        _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
    }
    
    [Test]
    public async Task UploadUserAvatar_NoUser_ReturnsUnauthorized()
    {
        // Arrange
        var id = 1;
        _httpContextMock.Setup(x => x.User).Returns(_unauthorizedUser);
        
        _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(_user);
        
        // Act
        var result = await _controller.UploadUserAvatarAsync(id, null, CancellationToken.None);
        
        // Assert
        Assert.IsInstanceOf<UnauthorizedResult>(result);
        _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
    }

    [Test]
    public async Task UploadUserAvatar_UserIsDifferent_ReturnsForbid()
    {
        // Arrange
        var id = 1;
        _httpContextMock.Setup(x => x.User).Returns(_wrongUser);
        
        _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(_user);
        
        // Act
        var result = await _controller.UploadUserAvatarAsync(id, null, CancellationToken.None);
        
        // Assert
        Assert.IsInstanceOf<ForbidResult>(result);
        _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
    }
    
    [Test]
    public async Task UploadUserAvatar_FileDoesntExist_ReturnsBadRequest()
    {
        // Arrange
        var id = 1;
        _httpContextMock.Setup(x => x.User).Returns(_authorizedUser);
        
        _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(_user);
        
        // Act
        var result = await _controller.UploadUserAvatarAsync(id, null, CancellationToken.None);
        
        // Assert
        Assert.IsInstanceOf<BadRequestResult>(result);
        _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
    }
    
    [Test]
    public async Task UploadUserAvatar_FileLengthIsZero_ReturnsBadRequest()
    {
        // Arrange
        var id = 1;
        _httpContextMock.Setup(x => x.User).Returns(_authorizedUser);
        
        //Setup mock file using a memory stream
        string? content = null;
        var fileName = "test.png";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;

        //create FormFile with desired data
        IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);
        
        _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(_user);
        
        // Act
        var result = await _controller.UploadUserAvatarAsync(id, file, CancellationToken.None);
        
        // Assert
        Assert.IsInstanceOf<BadRequestResult>(result);
        _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
    }
    
    [Test]
    public async Task UploadUserAvatar_FileTypeIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        var id = 1;
        _httpContextMock.Setup(x => x.User).Returns(_authorizedUser);
        
        //Setup mock file using a memory stream
        var content = "Test";
        var fileName = "test.png";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;

        //create FormFile with desired data
        IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);
        
        _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(_user);
        
        // Act
        var result = await _controller.UploadUserAvatarAsync(id, file, CancellationToken.None);
        
        // Assert
        Assert.IsInstanceOf<BadRequestResult>(result);
        _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
    }
    
    [Test]
    public async Task UploadUserAvatar_FileTypeIsValid_ReturnsOk()
    {
        // Arrange
        var id = 1;
        _httpContextMock.Setup(x => x.User).Returns(_authorizedUser);
        
        //Setup mock file using a memory stream
        var content = "Test";
        var fileName = "test.png";
        var pngSignature = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        
        stream.Write(pngSignature, 0, pngSignature.Length);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;

        //create FormFile with desired data
        IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);
        
        _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(_user);
        _userRepositoryMock.Setup(x => x.Update(_user));
        _userRepositoryMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        
        // Act
        var result = await _controller.UploadUserAvatarAsync(id, file, CancellationToken.None);
        
        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
        _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
        _userRepositoryMock.Verify(x => x.Update(_user), Times.Once);
        _userRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Test]
    public async Task UploadUserThumbnail_FileIsValidAndUserIsMod_ReturnsOk()
    {
        // Arrange
        var id = 1;
        _httpContextMock.Setup(x => x.User).Returns(_moderator);
        
        //Setup mock file using a memory stream
        var content = "Test";
        var fileName = "test.png";
        var pngSignature = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        
        stream.Write(pngSignature, 0, pngSignature.Length);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;

        //create FormFile with desired data
        IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);
        
        _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(_user);
        _userRepositoryMock.Setup(x => x.Update(_user));
        _userRepositoryMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        
        // Act
        var result = await _controller.UploadUserAvatarAsync(id, file, CancellationToken.None);
        
        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
        _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
        _userRepositoryMock.Verify(x => x.Update(_user), Times.Once);
        _userRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}