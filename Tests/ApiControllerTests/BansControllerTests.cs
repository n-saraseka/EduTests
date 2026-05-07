using EduTests.ApiObjects;
using EduTests.Controllers.Api;
using EduTests.Database.Entities;
using EduTests.Database.Repositories.Interfaces;
using EduTests.Services;
using Microsoft.AspNetCore.Mvc;
using MockQueryable;
using Moq;
using NUnit.Framework;

namespace EduTests.Tests.ApiControllerTests;

[TestFixture]
public class BansControllerTests
{
    // Mocks and the tested controller
    private Mock<IBannedUserRepository> _bannedRepositoryMock;
    private Mock<IEntityToDtoService> _entityToDtoServiceMock;
    private BansController _controller;
    
    // Test data
    private IQueryable<BannedUser> _bannedUsers;

    [SetUp]
    public void Setup()
    {
        _bannedRepositoryMock = new Mock<IBannedUserRepository>();
        _entityToDtoServiceMock = new Mock<IEntityToDtoService>();
        
        _controller = new BansController(_bannedRepositoryMock.Object, _entityToDtoServiceMock.Object);

        var currentDate = DateTime.Now;
        
        var bannedUsers = new List<BannedUser>();
        for (var i = 0; i < 50; i++)
        {
            bannedUsers.Add(new BannedUser
            {
                Id = i + 1,
                BannedById = 1,
                UserBannedId = i + 2,
                DateBanned = currentDate.AddDays(-i),
                DateUnbanned = i % 2 == 0 ? null : currentDate.AddDays(i),
                BanReason = "Bad guy"
            });
        }

        _bannedUsers = bannedUsers.BuildMock();
    }

    [Test]
    public async Task GetLatestBans_BadPage_ReturnsBadRequest()
    {
        // Arrange
        var active = true;
        var page = 0;
        var amountPerPage = 1;
        
        // Act
        var result = await _controller.GetLatestBansAsync(active, page, amountPerPage, CancellationToken.None);
        
        // Assert
        Assert.IsInstanceOf<BadRequestObjectResult>(result);
        
        _bannedRepositoryMock.Verify(x => x.GetLatestBans(It.IsAny<bool>()), Times.Never);
    }

    [Test]
    public async Task GetLatestBans_BadAmountPerPage_ReturnsBadRequest()
    {
        // Arrange
        var active = true;
        var page = 1;
        var amountPerPage = 0;
        
        // Act
        var result = await _controller.GetLatestBansAsync(active, page, amountPerPage, CancellationToken.None);
        
        // Assert
        Assert.IsInstanceOf<BadRequestObjectResult>(result);
        
        _bannedRepositoryMock.Verify(x => x.GetLatestBans(It.IsAny<bool>()), Times.Never);
    }

    [Test]
    public async Task GetLatestBans_PaginationWorksCorrectly()
    {
        // Arrange
        var active = true;
        var page = 1;
        var amountPerPage = 10;
        
        _bannedRepositoryMock.Setup(x => x.GetLatestBans(active))
            .Returns(_bannedUsers);
        _entityToDtoServiceMock.Setup(x => x.BanEntityToDto(It.IsAny<BannedUser>())).Returns(It.IsAny<ApiBan>());
        
        // Act
        var result = await _controller.GetLatestBansAsync(active, page, amountPerPage, CancellationToken.None);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            
            Assert.That(okResult, Is.Not.Null);
            dynamic response = okResult.Value;
            
            Assert.That(response.bans.Count, Is.EqualTo(amountPerPage));
            Assert.That(response.pages, Is.EqualTo(5));
        });
        
        _bannedRepositoryMock.Verify(x => x.GetLatestBans(It.IsAny<bool>()), Times.Once);
        _entityToDtoServiceMock.Verify(x => x.BanEntityToDto(It.IsAny<BannedUser>()), Times.Exactly(10));
    }
    
    [Test]
    public async Task DeleteBan_BanNotFound_ReturnsNotFound()
    {
        // Arrange
        var id = 51;

        _bannedRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync((BannedUser?)null);
        
        // Act
        var result = await _controller.DeleteBanAsync(id, CancellationToken.None);
        
        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
        _bannedRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
        _bannedRepositoryMock.Verify(x => x.Delete(It.IsAny<BannedUser>()), Times.Never);
        _bannedRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Test]
    public async Task DeleteBan_BanFound_ReturnsOk()
    {
        // Arrange
        var id = 50;

        _bannedRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(_bannedUsers.ElementAt(49));
        _bannedRepositoryMock.Setup(x => x.Delete(_bannedUsers.ElementAt(49)));
        _bannedRepositoryMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        
        // Act
        var result = await _controller.DeleteBanAsync(id, CancellationToken.None);
        
        // Assert
        Assert.IsInstanceOf<OkResult>(result);
        _bannedRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
        _bannedRepositoryMock.Verify(x => x.Delete(It.IsAny<BannedUser>()), Times.Once);
        _bannedRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}