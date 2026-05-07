using EduTests.ApiObjects;
using EduTests.Commands.TagCommands;
using EduTests.Controllers.Api;
using EduTests.Database.Entities;
using EduTests.Database.Repositories.Interfaces;
using EduTests.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace EduTests.Tests.ApiControllerTests;

[TestFixture]
public class TagsControllerTests
{
    // Mocks and the tested controller
    private Mock<ITagRepository> _tagRepositoryMock;
    private Mock<IEntityToDtoService> _entityToDtoServiceMock;
    private TagsController _controller;
    
    // Test data
    private Tag _tag;
    private ApiTag _apiTag;

    [SetUp]
    public void Setup()
    {
        _tagRepositoryMock = new Mock<ITagRepository>();
        _entityToDtoServiceMock = new Mock<IEntityToDtoService>();
        
        _controller = new TagsController(_tagRepositoryMock.Object, _entityToDtoServiceMock.Object);

        _tag = new Tag
        {
            Id = 1,
            Name = "Test"
        };

        _apiTag = new ApiTag
        {
            Id = 1,
            Name = "Test"
        };
    }

    [Test]
    public async Task AddTag_TagExists_ReturnsConflict()
    {
        // Arrange
        var command = new CreateTagCommand
        {
            Name = "Test"
        };
        
        _tagRepositoryMock.Setup(x => x.GetByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_tag);
        
        // Act
        var result = await _controller.AddTagAsync(command, CancellationToken.None);
        
        // Assert
        Assert.IsInstanceOf<ConflictObjectResult>(result);
        
        _tagRepositoryMock.Verify(x => x.GetByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _tagRepositoryMock.Verify(x => x.Create(It.IsAny<Tag>()), Times.Never);
    }
    
    [Test]
    public async Task AddTag_TagIsNew_ReturnsCreated()
    {
        // Arrange
        var command = new CreateTagCommand
        {
            Name = "Test"
        };
        
        _tagRepositoryMock.Setup(x => x.GetByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Tag?)null);
        _tagRepositoryMock.Setup(x => x.Create(It.Is<Tag>(t => t.Name == command.Name)))
            .Callback<Tag>(t =>
            {
                t.Id = _tag.Id;
            });
        
        _tagRepositoryMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        _entityToDtoServiceMock.Setup(x => x.TagEntityToDto(It.Is<Tag>(t => t.Id == _tag.Id && t.Name == command.Name)))
            .Returns(_apiTag);
        
        // Act
        var result = await _controller.AddTagAsync(command, CancellationToken.None);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<CreatedAtActionResult>(result);
            var createdAtActionResult = (CreatedAtActionResult)result;
            
            Assert.That(createdAtActionResult, Is.Not.Null);
            
            Assert.That(createdAtActionResult.ActionName, Is.EqualTo("GetTag"));
            Assert.That(createdAtActionResult.RouteValues["id"], Is.EqualTo(_tag.Id));
            
            Assert.IsInstanceOf<ApiTag>(createdAtActionResult.Value);
            var value = (ApiTag)createdAtActionResult.Value;
            Assert.That(value.Id,Is.EqualTo(_apiTag.Id));
            Assert.That(value.Name,Is.EqualTo(_apiTag.Name));
        });
        
        _tagRepositoryMock.Verify(x => x.GetByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _tagRepositoryMock.Verify(x => x.Create(It.IsAny<Tag>()), Times.Once);
        _tagRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetTag_TagDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var id = 1;
        _tagRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((Tag?)null);
        
        // Act
        var result = await _controller.GetTagAsync(id, CancellationToken.None);
        
        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
        
        _tagRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
        _entityToDtoServiceMock.Verify(x => x.TagEntityToDto(It.IsAny<Tag>()), Times.Never);
    }
    
    [Test]
    public async Task GetTag_TagExists_ReturnsOk()
    {
        // Arrange
        var id = 1;
        _tagRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(_tag);
        _entityToDtoServiceMock.Setup(x => x.TagEntityToDto(_tag)).Returns(_apiTag);
        
        // Act
        var result = await _controller.GetTagAsync(id, CancellationToken.None);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            
            Assert.IsInstanceOf<ApiTag>(okResult.Value);
            var tag = (ApiTag)okResult.Value;
            Assert.That(tag.Id,Is.EqualTo(_apiTag.Id));
            Assert.That(tag.Name,Is.EqualTo(_apiTag.Name));
        });
        
        _tagRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
        _entityToDtoServiceMock.Verify(x => x.TagEntityToDto(It.IsAny<Tag>()), Times.Once);
    }
    
    [Test]
    public async Task DeleteTag_TagDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var id = 1;
        _tagRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((Tag?)null);
        
        // Act
        var result = await _controller.DeleteTagAsync(id, CancellationToken.None);
        
        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
        
        _tagRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
        _tagRepositoryMock.Verify(x => x.Delete(It.IsAny<Tag>()), Times.Never);
    }
    
    [Test]
    public async Task DeleteTag_TagExists_ReturnsOk()
    {
        // Arrange
        var id = 1;
        _tagRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(_tag);
        _tagRepositoryMock.Setup(x => x.Delete(It.IsAny<Tag>())).Callback<Tag>(x => Assert.That(x.Name, Is.SameAs(_tag.Name)));
        _tagRepositoryMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        
        // Act
        var result = await _controller.DeleteTagAsync(id, CancellationToken.None);
        
        // Assert
        Assert.IsInstanceOf<OkResult>(result);
        
        _tagRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
        _tagRepositoryMock.Verify(x => x.Delete(It.IsAny<Tag>()), Times.Once);
        _tagRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}