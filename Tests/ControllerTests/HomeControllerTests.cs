using EduTests.ApiObjects;
using EduTests.Controllers;
using EduTests.Database.Entities;
using EduTests.Database.Enums;
using EduTests.Database.Repositories.Interfaces;
using EduTests.Models;
using EduTests.Services;
using Microsoft.AspNetCore.Mvc;
using MockQueryable;
using Moq;
using NUnit.Framework;

namespace EduTests.Tests.ControllerTests;

[TestFixture]
public class HomeControllerTests
{
    private Mock<ITestRepository> _testMock;
    private Mock<ITagRepository> _tagMock;
    private Mock<ITestStatsService> _statsServiceMock;
    private Mock<IEntityToDtoService> _entityToDtoServiceMock;
    private Mock<IConfiguration> _configMock;
    private HomeController _controller;

    [SetUp]
    public void Setup()
    {
        _testMock = new Mock<ITestRepository>();
        _tagMock = new Mock<ITagRepository>();
        _statsServiceMock = new Mock<ITestStatsService>();
        _entityToDtoServiceMock = new Mock<IEntityToDtoService>();
        _configMock = new Mock<IConfiguration>();
        
        _configMock.Setup(c => c["testsCatalogPageSize"]).Returns("10");
        _configMock.Setup(c => c["testCardTagCount"]).Returns("3");
        _configMock.Setup(c => c["popularTagCount"]).Returns("5");

        _controller = new HomeController(
            _testMock.Object, _statsServiceMock.Object, 
            _tagMock.Object, _entityToDtoServiceMock.Object, _configMock.Object);
    }

    [Test]
    public async Task Index_PageOverflow_AdjustsToLastPage()
    {
        // Arrange
        var test = new Test
        {
            Id = 1,
            UserId = 1,
            Name = "Test"
        };
        var tests = new List<Test> { test }.BuildMock();
        _testMock.Setup(x => x.GetAllWithTags()).Returns(tests);
        
        _entityToDtoServiceMock.Setup(x => x.TestEntityToDto(It.IsAny<Test>())).Returns((Test t) =>
            new ApiTest
            {
                Id = t.Id,
                Name = t.Name,
                UpdatedAt = t.UpdatedAt,
                CreatedAt = t.CreatedAt,
                ShowCorrectAnswers = t.ShowCorrectAnswers,
                HasPassword = t.Password != null,
                AccessType = t.AccessType
            });
        
        _tagMock.Setup(x => x.GetPopularTags()).Returns(new List<Tag>().BuildMock());

        // Act
        var result = await _controller.Index(5, null, new HomeViewModel());

        // Assert
        var viewResult = (ViewResult)result;
        var model = (HomeViewModel)viewResult.Model;
        Assert.That(model.Page, Is.EqualTo(1));
        Assert.That(model.Pages, Is.EqualTo(1));
    }

    [Test]
    public async Task Index_WithTagName_CallsSpecificRepositoryMethod()
    {
        // Arrange
        string tag = "test";
        var tests = new List<Test>().BuildMock();
        _testMock.Setup(x => x.GetAllByTag(tag)).Returns(tests);

        // Act
        await _controller.Index(1, tag, new HomeViewModel());

        // Assert
        _testMock.Verify(x => x.GetAllByTag(tag), Times.Once);
        _testMock.Verify(x => x.GetAllWithTags(), Times.Never);
    }

    [Test]
    public async Task Index_TagsInCard_AreLimitedByConfig()
    {
        // Arrange
        var test = new Test
        {
            Id = 1,
            UserId = 1,
            Name = "Test"
        };
        var tests = new List<Test> { test }.BuildMock();
        _testMock.Setup(x => x.GetAllWithTags()).Returns(tests);

        var tags = new List<ApiTag>();
        for (int i = 0; i < 5; i++)
        {
            tags.Add(new ApiTag
            {
                Id = i + 1,
                Name = $"test{i}",
            });
        }
        var apiTest = new ApiTest 
        { 
            Id = 1,
            Name = "test",
            AccessType = AccessType.Public,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            HasPassword = false,
            ShowCorrectAnswers = false,
            Tags = tags.Select(t => t.Name).ToList()
        };
        
        _entityToDtoServiceMock.Setup(x => x.TestEntityToDto(It.IsAny<Test>())).Returns(apiTest);
        _statsServiceMock.Setup(x => x.GetTestsStatsAsync(It.IsAny<IEnumerable<ApiTest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ApiTest> { apiTest });
        
        _tagMock.Setup(x => x.GetPopularTags()).Returns(new List<Tag>().BuildMock());

        // Act
        var result = await _controller.Index(1, null, new HomeViewModel());

        // Assert
        var model = (HomeViewModel)((ViewResult)result).Model;
        Assert.That(model.Tests.First().Tags.Count, Is.EqualTo(3));
    }

    [Test]
    public async Task PopularTests_IsSortedCorrectly()
    {
        // Arrange
        var test = new Test
        {
            Id = 1,
            UserId = 1,
            Name = "Test"
        };
        var tests = new List<Test> { new Test
        {
            Id = 1,
            UserId = 1,
            Name = "Test"
        }, new Test
        {
            Id = 2,
            UserId = 1,
            Name = "Test"
        }}.BuildMock();
        _testMock.Setup(x => x.GetAllWithTags()).Returns(tests);

        var apiTests = new List<ApiTest> 
        { 
            new ApiTest
            {
                Id = 1,
                Name = "Test",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                AccessType = AccessType.Public,
                HasPassword = false,
                ShowCorrectAnswers = false,
                CompletionCount = 10, Rating = 4
            },
            new ApiTest
            {
                Id = 2,
                Name = "Test",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                AccessType = AccessType.Public,
                HasPassword = false,
                ShowCorrectAnswers = false,
                CompletionCount = 10, Rating = 5
            },
        };
        
        _statsServiceMock.Setup(x => x.GetTestsStatsAsync(It.IsAny<IEnumerable<ApiTest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiTests);
        
        _tagMock.Setup(x => x.GetPopularTags()).Returns(new List<Tag>().BuildMock());
        _entityToDtoServiceMock.Setup(x => x.TestEntityToDto(It.IsAny<Test>())).Returns((Test t) => apiTests.First(a => a.Id == t.Id));

        // Act
        var result = await _controller.PopularTests(1, new HomeViewModel());

        // Assert
        var model = (HomeViewModel)((ViewResult)result).Model;
        Assert.That(model.Tests.First().Id, Is.EqualTo(2));
    }
}