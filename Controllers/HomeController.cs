using System.Diagnostics;
using EduTests.Database.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using EduTests.Models;
using EduTests.Services;
using Microsoft.EntityFrameworkCore;

namespace EduTests.Controllers;

public class HomeController(ITestRepository testRepository,
    ITestStatsService testStatsService,
    ITagRepository tagRepository,
    IEntityToDtoService entityToDtoService,
    IConfiguration config) : Controller
{
    public async Task<IActionResult> Index(int? page, string? tagName, HomeViewModel model, CancellationToken cancellationToken = default)
    {
        var actualPage = page ?? 1;
        
        var testsQuery = tagName == null ? testRepository.GetAllWithTags() : testRepository.GetAllByTag(tagName);
        
        var pageSize = int.Parse(config["testsCatalogPageSize"]);
        var testCount = await testsQuery.CountAsync(cancellationToken);
        model.Pages = (int)Math.Ceiling((double)testCount / pageSize);
        // To avoid querying for empty pages
        actualPage = Math.Max(Math.Min(actualPage, model.Pages), 1);
        model.Page = actualPage;
        
        var tests = await testsQuery.Skip((actualPage - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        
        var apiTests = tests.Select(entityToDtoService.TestEntityToDto);

        if (apiTests.Count() > 0)
        {
            apiTests = await testStatsService.GetTestsStatsAsync(apiTests, cancellationToken);
            var testCardTagCount = int.Parse(config["testCardTagCount"]);
            foreach (var test in apiTests)
            {
                test.Tags = test.Tags.Take(testCardTagCount).ToList();
            }
        }

        var testList = apiTests.ToList();

        model.Tests = testList;
        if (tagName == null)
        {
            var tagCount = int.Parse(config["popularTagCount"]);
            var tags = await tagRepository.GetPopularTags().Take(tagCount).ToListAsync(cancellationToken);
            var apiTags = tags.Select(entityToDtoService.TagEntityToDto).ToList();

            model.PopularTags = apiTags;
        }
        
        return View(model);
    }

    public async Task<IActionResult> PopularTests(int? page, HomeViewModel model,
        CancellationToken cancellationToken = default)
    {
        var actualPage = page ?? 1;
        
        var testsQuery = testRepository.GetAllWithTags();
        
        var pageSize = int.Parse(config["testsCatalogPageSize"]);
        var testCount = await testsQuery.CountAsync(cancellationToken);
        model.Pages = (int)Math.Ceiling((double)testCount / pageSize);
        // To avoid querying for empty pages
        actualPage = Math.Max(Math.Min(actualPage, model.Pages), 1);
        model.Page = actualPage;
        
        var tests = await testsQuery.Skip((actualPage - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        
        var apiTests = tests.Select(entityToDtoService.TestEntityToDto);

        if (apiTests.Count() > 0)
        {
            apiTests = await testStatsService.GetTestsStatsAsync(apiTests, cancellationToken);
            var testCardTagCount = int.Parse(config["testCardTagCount"]);
            foreach (var test in apiTests)
            {
                test.Tags = test.Tags.Take(testCardTagCount).ToList();
            }
        }

        var testList = apiTests
            .OrderByDescending(t => t.CompletionCount)
            .ThenByDescending(t => t.Rating)
            .ToList();

        model.Tests = testList;
        
        var tagCount = int.Parse(config["popularTagCount"]);
        var tags = await tagRepository.GetPopularTags().Take(tagCount).ToListAsync(cancellationToken);
        var apiTags = tags.Select(entityToDtoService.TagEntityToDto).ToList();

        model.PopularTags = apiTags;
        
        return View(model);
    }

    public async Task<IActionResult> Search(string query, int? page, SearchViewModel model,
        CancellationToken cancellationToken = default)
    {
        model.SearchQuery = query;
        
        var actualPage = page ?? 1;
        
        var testsQuery = testRepository.Search(query);
        
        var pageSize = int.Parse(config["testsCatalogPageSize"]);
        var testCount = await testsQuery.CountAsync(cancellationToken);
        model.Pages = (int)Math.Ceiling((double)testCount / pageSize);
        // To avoid querying for empty pages
        actualPage = Math.Max(Math.Min(actualPage, model.Pages), 1);
        model.Page = actualPage;
        
        var tests = await testsQuery.Skip((actualPage - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        var apiTests = tests.Select(entityToDtoService.TestEntityToDto);

        if (apiTests.Count() > 0)
        {
            apiTests = await testStatsService.GetTestsStatsAsync(apiTests, cancellationToken);
            var testCardTagCount = int.Parse(config["testCardTagCount"]);
            foreach (var test in apiTests)
            {
                test.Tags = test.Tags.Take(testCardTagCount).ToList();
            }
        }

        var testList = apiTests.ToList();

        model.Tests = testList;
        
        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}