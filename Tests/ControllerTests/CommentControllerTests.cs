using EduTests.Controllers;
using EduTests.Database.Entities;
using EduTests.Database.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace EduTests.Tests.ControllerTests;

[TestFixture]
public class CommentControllerTests
{
    private Mock<ICommentRepository> _commentMock;
    private CommentController _controller;

    [SetUp]
    public void Setup()
    {
        _commentMock = new Mock<ICommentRepository>();
        _controller = new CommentController(_commentMock.Object);
    }

    [Test]
    public async Task GetCommentBaseAsync_CommentNotFound_ReturnsNotFound()
    {
        // Arrange
        _commentMock.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Comment?)null);

        // Act
        var result = await _controller.GetCommentBaseAsync(1, CancellationToken.None);

        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public async Task GetCommentBaseAsync_UserProfileComment_RedirectsToUserProfile()
    {
        // Arrange
        var comment = new Comment
        {
            Id = 1,
            CommenterId = 1,
            UserProfileId = 1,
            Content = "test"
        };
        
        _commentMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(comment);

        // Act
        var result = await _controller.GetCommentBaseAsync(1, CancellationToken.None);

        // Assert
        var redirect = (RedirectToActionResult)result;
        Assert.That(redirect.ActionName, Is.EqualTo("Profile"));
        Assert.That(redirect.ControllerName, Is.EqualTo("User"));
        Assert.That(redirect.RouteValues["id"], Is.EqualTo(1));
    }

    [Test]
    public async Task GetCommentBaseAsync_TestComment_RedirectsToTestMainPage()
    {
        // Arrange
        var comment = new Comment
        {
            Id = 1,
            CommenterId = 1,
            TestId = 1,
            Content = "test"
        };
        
        _commentMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(comment);

        // Act
        var result = await _controller.GetCommentBaseAsync(1, CancellationToken.None);

        // Assert
        var redirect = (RedirectToActionResult)result;
        Assert.That(redirect.ActionName, Is.EqualTo("MainPage"));
        Assert.That(redirect.ControllerName, Is.EqualTo("Test"));
        Assert.That(redirect.RouteValues["id"], Is.EqualTo(1));
    }

    [Test]
    public async Task GetCommentBaseAsync_NoLinks_RedirectsToHome()
    {
        // Arrange
        var comment = new Comment
        {
            Id = 1,
            CommenterId = 1,
            Content = "test"
        };
        
        _commentMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(comment);

        // Act
        var result = await _controller.GetCommentBaseAsync(1, CancellationToken.None);

        // Assert
        var redirect = (RedirectToActionResult)result;
        Assert.That(redirect.ActionName, Is.EqualTo("Index"));
        Assert.That(redirect.ControllerName, Is.EqualTo("Home"));
    }
}