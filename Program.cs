using System.Security.Claims;
using EduTests.Controllers.Api;
using EduTests.Database;
using EduTests.Database.Enums;
using EduTests.Database.Repositories;
using EduTests.Database.Repositories.Interfaces;
using EduTests.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), 
        o => o.MapEnum<AccessType>("AccessType")
                                        .MapEnum<QuestionType>("QuestionType")
                                        .MapEnum<ReportStatus>("ReportStatus")
                                        .MapEnum<UserGroup>("UserGroup")));
// repositories
builder.Services.AddScoped<ITestRepository, TestRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IUserRatingRepository, UserRatingRepository>();
builder.Services.AddScoped<ITestCompletionRepository, TestCompletionRepository>();
// services
builder.Services.AddScoped<IQuestionValidatorService, QuestionValidatorService>();
builder.Services.AddScoped<IAuthentificationService, AuthentificationService>();
builder.Services.AddControllersWithViews();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication("Cookies")
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
    });

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/login", async (string? returnUrl, IAuthentificationService service, HttpContext context, CancellationToken cancellationToken) =>
{
    var form = context.Request.Form;
    
    if (!form.ContainsKey("login") || !form.ContainsKey("password"))
        return Results.BadRequest("Login and Password are required.");
    
    string login = form["login"];
    string password = form["password"];
    
    var user = await service.ValidateUserAsync(login, password, cancellationToken);
    if (user == null)
        return Results.Unauthorized();
    
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Login)
    };
    var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
    
    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
    return Results.Redirect(returnUrl??"/");
});

app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/");
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();