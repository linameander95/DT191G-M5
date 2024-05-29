using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using puremy.Models;
using puremy.Data;

public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly BlogPostContext _context;

    public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, BlogPostContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    [HttpGet]
[AllowAnonymous]
public IActionResult Register()
{
    return View();
}

[HttpPost]
[AllowAnonymous]
public async Task<IActionResult> Register(RegisterViewModel model)
{
    if (ModelState.IsValid)
    {
        var user = new IdentityUser { UserName = model.Email, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("index", "home");
        }

        foreach (var error in result.Errors)
        {
            Console.WriteLine(error.Description);
            ModelState.AddModelError("", error.Description);
        }
    }
    else
    {
        Console.WriteLine("Model validation failed.");
        foreach (var modelState in ViewData.ModelState.Values)
        {
            foreach (var error in modelState.Errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }
        }
    }

    return View(model);
}

public IActionResult Login()
{
    return View();
}

[HttpPost]
[AllowAnonymous]
public async Task<IActionResult> Login(LoginViewModel model)
{
    if (ModelState.IsValid)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            Console.WriteLine("The user does not exist.");
            ModelState.AddModelError("", "Invalid login attempt");
            return View(model);
        }

        if (await _userManager.IsLockedOutAsync(user))
        {
            Console.WriteLine("The user is locked out.");
            ModelState.AddModelError("", "Invalid login attempt");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

        if (!result.Succeeded)
        {
            Console.WriteLine("Invalid login attempt.");
            ModelState.AddModelError("", "Invalid login attempt");
            return View(model);
        }

        return RedirectToAction("index", "home");
    }

    return View(model);
}

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("index", "home");
    }

    [Authorize]
    public async Task<IActionResult> MyPages()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }

        var blogPosts = await _context.BlogPosts
            .Where(post => post.UserId == user.Id)
            .ToListAsync();

        return View(blogPosts);
    }
}