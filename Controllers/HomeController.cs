using System.Diagnostics;
using EduTests.Database.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using EduTests.Models;
using EduTests.Services;
using Microsoft.EntityFrameworkCore;

namespace EduTests.Controllers;

public class HomeController(ITestRepository testRepository,
    IUserRatingRepository userRatingRepository,
    ITestCompletionRepository testCompletionRepository,
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
        actualPage = Math.Min(actualPage, model.Pages);
        model.Page = actualPage;
        
        var tests = await testsQuery.Skip((actualPage - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        
        var apiTests = tests.Select(entityToDtoService.TestEntityToDto);
        
        var ids = apiTests.Select(t => t.Id);
        var ratings = await userRatingRepository.GetTestRatingsAsync(ids, cancellationToken);
        var completions = await testCompletionRepository.GetTestCompletionCountsAsync(ids, cancellationToken);

        var testList = apiTests.Select(t =>
        {
            t.Rating = ratings[t.Id];
            t.CompletionCount = completions[t.Id];
            return t;
        }).ToList();

        model.Tests = testList;
        if (tagName == null)
        {
            var tags = await tagRepository.GetPopularTags().Take(pageSize).ToListAsync(cancellationToken);
            var apiTags = tags.Select(entityToDtoService.TagEntityToDto).ToList();

            model.PopularTags = apiTags;
        }
        
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