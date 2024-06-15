using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Data;
using WebApplication1.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OAuth.GitHub;
using WebApplication1.Extensions;

public class LoginModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public LoginModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public User LoginUser { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == LoginUser.Username && u.Password == LoginUser.Password);

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }

        // Set session here
        HttpContext.Session.SetString("Username", user.Username);
        HttpContext.Session.SetBool("IsAdmin", user.IsAdmin);

        return RedirectToPage("./Profile");
    }

    public IActionResult OnPostGoogleLogin()
    {
        var redirectUrl = Url.Page("/Login", pageHandler: "GoogleResponse");
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return new ChallengeResult(GoogleDefaults.AuthenticationScheme, properties);
    }

    public async Task<IActionResult> OnGetGoogleResponseAsync()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!authenticateResult.Succeeded)
            return RedirectToPage("./Login");

        var claimsPrincipal = authenticateResult.Principal;

        var email = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;
        var name = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;

        if (email == null)
            return RedirectToPage("./Login");

        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            user = new User
            {
                Username = email,
                Email = email,
                DisplayName = name,
                Password = null // Set Password to null for Google users
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

        HttpContext.Session.SetString("Username", user.Username);
        HttpContext.Session.SetBool("IsAdmin", user.IsAdmin);

        return RedirectToPage("./Profile"); // Redirect to the Profile page
    }

    public IActionResult OnPostGitHubLogin()
    {
        var redirectUrl = Url.Page("/Login", pageHandler: "GitHubResponse");
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return new ChallengeResult(GitHubAuthenticationDefaults.AuthenticationScheme, properties);
    }

    public async Task<IActionResult> OnGetGitHubResponseAsync()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!authenticateResult.Succeeded)
            return RedirectToPage("./Login");

        var claimsPrincipal = authenticateResult.Principal;

        var email = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;
        var name = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;

        if (email == null)
            return RedirectToPage("./Login");

        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            user = new User
            {
                Username = email,
                Email = email,
                DisplayName = name,
                Password = null // Set Password to null for GitHub users
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

        HttpContext.Session.SetString("Username", user.Username);
        HttpContext.Session.SetBool("IsAdmin", user.IsAdmin);

        return RedirectToPage("./Profile"); // Redirect to the Profile page
    }
}
