using EduTests;
using EduTests.Database;
using EduTests.Database.Enums;
using EduTests.Database.Repositories;
using EduTests.Database.Repositories.Interfaces;
using EduTests.Services;
using EduTests.Services.Questions;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DatabaseContext>(options =>
    options
        .UseSnakeCaseNamingConvention()
        .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), 
        o =>
        {
            o.MapEnum<AccessType>("AccessType")
                .MapEnum<QuestionType>("QuestionType")
                .MapEnum<ReportStatus>("ReportStatus")
                .MapEnum<UserGroup>("UserGroup");
        }));
// repositories
builder.Services.AddScoped<ITestRepository, TestRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IUserRatingRepository, UserRatingRepository>();
builder.Services.AddScoped<ITestCompletionRepository, TestCompletionRepository>();
builder.Services.AddScoped<IBannedUserRepository, BannedUserRepository>();
builder.Services.AddScoped<IReportsRepository, ReportsRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<ITestResultRepository, TestResultRepository>();
builder.Services.AddScoped<IUserAnswerRepository, UserAnswerRepository>();
builder.Services.AddScoped<IAnonymousUserRepository, AnonymousUserRepository>();
// services
builder.Services.AddScoped<IQuestionValidatorService, QuestionValidatorService>();
builder.Services.AddScoped<IAnswerValidatorService, AnswerValidatorService>();
builder.Services.AddScoped<IAnswerVerifierService, AnswerVerifierService>();
builder.Services.AddScoped<IDatabaseSeederService, DatabaseSeederService>();
builder.Services.AddScoped<IEntityToDtoService, EntityToDtoService>();
builder.Services.AddScoped<ITestStatsService, TestStatsService>();
builder.Services.AddScoped<FileExtensionContentTypeProvider>();
builder.Services.AddHostedService<DbInitializerHostedService>();

builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication("Cookies")
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/login";
        options.LogoutPath = "/Account/logout";
        options.AccessDeniedPath = "/Account/accessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
    });

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.UseAuthentication();
app.UseMiddleware<AnonymousAuthenticationMiddleware>();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "login",
    pattern: "login",
    defaults: new {controller = "Account", action = "Login"}) 
    .WithStaticAssets();

app.MapControllerRoute(
        name: "register",
        pattern: "register",
        defaults: new {controller = "Account", action = "Register"})
    .WithStaticAssets();

app.MapControllerRoute(
        name: "logout",
        pattern: "logout",
        defaults: new {controller = "Account", action = "Logout"})
    .WithStaticAssets();

app.MapControllerRoute(
    name: "profile",
    pattern: "{controller}/{id}",
    defaults: new {controller = "User", action = "Profile"});

app.MapControllerRoute(
    name: "my_account",
    pattern: "{controller}/{id}/my_account",
    defaults: new {controller = "User", action = "Settings"})
    .WithStaticAssets();

app.MapControllerRoute(
    name: "comment_base",
    pattern: "{controller}/{id}",
    defaults: new {controller = "Comment", action = "GetCommentBase"})
    .WithStaticAssets();

app.MapControllerRoute(
    name: "test_constructor",
    pattern: "{controller}/{id}/constructor",
    defaults: new {controller = "Test", action = "Constructor"})
    .WithStaticAssets();

app.MapControllerRoute(
    name: "constructor",
    pattern: "constructor",
    defaults: new {controller = "Constructor", action = "BaseConstructor"})
    .WithStaticAssets();

app.MapControllerRoute(
    name: "test-page",
    pattern: "{controller}/{id}",
    defaults: new { controller = "Test", action = "TestPage" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "playthrough",
    pattern: "{controller}/{id}/playthrough/{playthroughId}",
    defaults: new { controller = "Test", action = "TestPlaythrough" });

app.MapControllerRoute(
    name: "result",
    pattern: "{controller}/{id}/playthrough/{playthroughId}/result",
    defaults: new { controller = "Test", action = "TestResult" });

app.MapControllerRoute(
    name: "popular",
    pattern: "popular",
    defaults: new { controller = "Home", action = "PopularTests" });

app.MapControllerRoute(
    name: "search",
    pattern: "search",
    defaults: new { controller = "Home", action = "Search"});

app.Run();