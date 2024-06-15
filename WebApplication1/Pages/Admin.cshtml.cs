using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Models;
using System.Linq;
using Data;
using WebApplication1.Extensions;

public class AdminModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public AdminModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public string Username { get; set; }

    public string Message { get; set; }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        var user = _context.Users.SingleOrDefault(u => u.Username == Username);
        if (user != null)
        {
            user.IsAdmin = true;
            _context.SaveChanges();

            // Update session
            HttpContext.Session.SetBool("IsAdmin", user.IsAdmin);

            return RedirectToPage("/Profile");
        }

        // Handle user not found case
        Message = "User not found";
        return Page();
    }
}
