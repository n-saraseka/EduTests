using EduTests.Database.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduTests.Controllers;

public class CommentController(ICommentRepository commentRepository) : Controller
{
    public async Task<IActionResult> GetCommentBaseAsync(int id, CancellationToken cancellationToken)
    {
        var comment = await commentRepository.GetByIdAsync(id, cancellationToken);
        if (comment == null)
            return NotFound();
        if (comment.UserProfileId != null)
            return RedirectToAction("Profile", "User", new { id = (int)comment.UserProfileId });
        if (comment.TestId != null)
            return RedirectToAction("MainPage", "Test", new { id = (int)comment.TestId });
        return RedirectToAction("Index", "Home");
    }
}