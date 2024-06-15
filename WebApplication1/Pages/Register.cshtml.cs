using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Data;
using WebApplication1.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AspNet.Security.OAuth.GitHub;
using Microsoft.Extensions.Logging;

public class RegisterModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(ApplicationDbContext context, ILogger<RegisterModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    [BindProperty]
    public User RegisterUser { get; set; }

    [BindProperty]
    public string ConfirmPassword { get; set; }

    public string ErrorMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Model state is invalid");
            return Page();
        }

        if (RegisterUser.Password != ConfirmPassword)
        {
            ErrorMessage = "Passwords do not match.";
            _logger.LogWarning("Passwords do not match");
            return Page();
        }

        // Check if the username already exists
        bool userExists = await _context.Users.AnyAsync(u => u.Username == RegisterUser.Username);
        if (userExists)
        {
            ErrorMessage = "An account with this username already exists.";
            _logger.LogWarning("An account with this username already exists");
            return Page(); // Stay on the same page to display the error message
        }

        // Add the new user to the database
        _context.Users.Add(RegisterUser);
        await _context.SaveChangesAsync();

        // Log in the user by setting the username in the session
        HttpContext.Session.SetString("Username", RegisterUser.Username);

        // Redirect to the home page or a specific page
        return RedirectToPage("./Profile");
    }

    public IActionResult OnPostGoogleLogin()
    {
        var redirectUrl = Url.Page("/Register", pageHandler: "GoogleResponse");
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return new ChallengeResult(GoogleDefaults.AuthenticationScheme, properties);
    }

    public async Task<IActionResult> OnGetGoogleResponseAsync()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!authenticateResult.Succeeded)
        {
            return RedirectToPage("./Register");
        }

        var claimsPrincipal = authenticateResult.Principal;
        var email = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;
        var name = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;

        if (email == null)
        {
            return RedirectToPage("./Register");
        }

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
        return RedirectToPage("./Profile"); // Redirect to the Profile page
    }

    public IActionResult OnPostGitHubLogin()
    {
        var redirectUrl = Url.Page("/Register", pageHandler: "GitHubResponse");
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return new ChallengeResult(GitHubAuthenticationDefaults.AuthenticationScheme, properties);
    }

    public async Task<IActionResult> OnGetGitHubResponseAsync()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!authenticateResult.Succeeded)
        {
            return RedirectToPage("./Register");
        }

        var claimsPrincipal = authenticateResult.Principal;
        var email = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;
        var name = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;

        if (email == null)
        {
            return RedirectToPage("./Register");
        }

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
        return RedirectToPage("./Profile"); // Redirect to the Profile page
    }
}
