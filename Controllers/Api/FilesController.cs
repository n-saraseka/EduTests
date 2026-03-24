using System.Security.Claims;
using EduTests.Database.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace EduTests.Controllers.Api;

[ApiController]
[Route("[controller]")]
public class FilesController(IWebHostEnvironment env,
    ITestRepository testRepository,
    IUserRepository userRepository) : Controller
{
    private readonly List<byte[]> _validSignatures = new List<byte[]>
    {
        new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, // PNG
        new byte[] { 0xFF, 0xD8, 0xFF, 0xDB }, // JPEG (JFIF)
        new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }, // JPEG (JFIF)
        new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 }  // JPEG (EXIF)
    };
    
    [HttpPut("tests/{id}")]
    [Authorize]
    [RequestSizeLimit(8_388_608)]
    public async Task<IActionResult> UploadTestThumbnailAsync(int id, IFormFile? file, 
        CancellationToken cancellationToken = default)
    {
        var test = await testRepository.GetByIdAsync(id, cancellationToken);
        if (test == null)
            return NotFound();
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized();
        
        var userIdInt = int.Parse(userId);
        
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        
        if (test.UserId != userIdInt && userRole != "Moderator" && userRole != "Administrator")
            return Forbid();
        
        if (file == null || file.Length == 0 || !IsValidFileType(file))
            return BadRequest();

        var url = await UploadFileAsync(file, test.ThumbnailUrl, "tests", cancellationToken);
        
        test.ThumbnailUrl = url;
        testRepository.Update(test);
        await testRepository.SaveChangesAsync(cancellationToken);

        return Ok(new { url });
    }
    
    [HttpPut("users/{id}")]
    [Authorize]
    [RequestSizeLimit(8_388_608)]
    public async Task<IActionResult> UploadUserAvatarAsync(int id, IFormFile? file, 
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
            return NotFound();
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized();
        
        var userIdInt = int.Parse(userId);
        
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        
        if (user.Id != userIdInt && userRole != "Moderator" && userRole != "Administrator")
            return Forbid();
        
        if (file == null || file.Length == 0 || !IsValidFileType(file))
            return BadRequest();

        var url = await UploadFileAsync(file, user.AvatarUrl, "users", cancellationToken);
        
        user.AvatarUrl = url;
        userRepository.Update(user);
        await userRepository.SaveChangesAsync(cancellationToken);

        return Ok(new { url });
    }

    private bool IsValidFileType(IFormFile file) {
        using var readStr = file.OpenReadStream();
        var buffer = new byte[10];
        readStr.ReadExactly(buffer, 0 , 10);
        foreach (var sig in _validSignatures)
            if (buffer.Take(sig.Length).SequenceEqual(sig))
                return true;
        return false;
    }

    private async Task<string> UploadFileAsync(IFormFile file, string? oldFileUrl, string subfolderName,
        CancellationToken cancellationToken = default)
    {
        var uploadFolder = Path.Combine(env.WebRootPath, "uploads", subfolderName);
        
        if (!Directory.Exists(uploadFolder))
            Directory.CreateDirectory(uploadFolder);

        var extension = Path.GetExtension(file.FileName);
        var name = Base64UrlTextEncoder.Encode(Guid.NewGuid().ToByteArray()).Replace("+", "-").Replace("/", "_");
        var fileName = $"{name}{extension}";
        
        var uploadPath = Path.Combine(uploadFolder, fileName);
        
        using (var fileStream = new FileStream(uploadPath, FileMode.Create))
            await file.CopyToAsync(fileStream, cancellationToken);
        
        if (!string.IsNullOrEmpty(oldFileUrl))
        {
            var oldFilePath = Path.Combine(env.WebRootPath, oldFileUrl.TrimStart('/'));
            if (System.IO.File.Exists(oldFilePath))
                System.IO.File.Delete(oldFilePath);
        }
        
        return $"/uploads/{subfolderName}/{fileName}";
    }
}