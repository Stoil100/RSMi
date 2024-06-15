using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Models;
using System.Collections.Generic;
using System.Linq;
using Data;
using WebApplication1.Extensions;

public class ProfileModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public ProfileModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public string Username { get; set; }
    public bool IsAdmin { get; set; }
    public List<User> Users { get; set; } = new List<User>();

    public void OnGet()
    {
        Username = HttpContext.Session.GetString("Username") ?? "Not logged in";
        IsAdmin = HttpContext.Session.GetBool("IsAdmin").GetValueOrDefault();

        if (IsAdmin)
        {
            Users = _context.Users.ToList();
        }
    }

    public IActionResult OnPostLogout()
    {
        // Clear the session
        HttpContext.Session.Clear();

        // Redirect to the home page or login page
        return RedirectToPage("/Index");
    }

    public IActionResult OnPostDelete(int userId)
    {
     
        var user = _context.Users.SingleOrDefault(u => u.UserId == userId);
        if (user != null)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }

        return RedirectToPage();
    }
}
