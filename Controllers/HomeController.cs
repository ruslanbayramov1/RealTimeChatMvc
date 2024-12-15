using ChatSignalR.Contexts;
using ChatSignalR.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatSignalR.Controllers;

public class HomeController : Controller
{
    [Authorize]
    public IActionResult Index()
    { 
        return View();
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(UserRegisterVM vm)
    {
        if (SharedDb.Users.Any(x => x.UserName == vm.UserName))
        {
            ModelState.AddModelError("", "The username already defined");
            return View();
        }
        ChatUser user = new ChatUser
        {
            Id = Guid.NewGuid().ToString(),
            ProfileImageUrl = vm.ProfileImageUrl,
            UserName = vm.UserName,
            FullName = vm.FullName,
        };

        SharedDb.Users.Add(user);
        await AddClaims(user);

        return RedirectToAction(nameof(Index));
    }

    private async Task AddClaims(ChatUser user)
    { 
        List<Claim> claims = new();

        claims.Add(new Claim(ClaimTypes.Name, user.UserName));
        claims.Add(new Claim("imageurl", user.ProfileImageUrl));
        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
        claims.Add(new Claim("fullname", user.FullName));

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = false,
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties
            );
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        HttpContext.Response.Cookies.Delete(".AspNetCore.Cookies");

        return RedirectToAction(nameof(Index));
    }
}
