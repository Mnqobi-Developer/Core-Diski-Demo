using Core_Diski_Demo.Models.Entities;
using Core_Diski_Demo.Models.ViewModels.Account;
using Core_Diski_Demo.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Core_Diski_Demo.Controllers;

public class AccountController(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    IEmailVerificationService emailVerificationService) : Controller
{
    [HttpGet("/sign-in")]
    public IActionResult SignIn(string? returnUrl = null)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost("/sign-in")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SignIn(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

        if (result.IsNotAllowed)
        {
            ModelState.AddModelError(string.Empty, "Please verify your email before signing in.");
            return View(model);
        }

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        TempData["Success"] = "Signed in successfully.";

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet("/register")]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost("/register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            ShippingAddress = model.ShippingAddress
        };

        var createResult = await userManager.CreateAsync(user, model.Password);
        if (!createResult.Succeeded)
        {
            foreach (var error in createResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var callbackUrl = Url.Action(nameof(ConfirmEmail), "Account", new { userId = user.Id, token }, protocol: Request.Scheme);

        if (!string.IsNullOrWhiteSpace(callbackUrl))
        {
            await emailVerificationService.SendVerificationEmailAsync(user.Email!, callbackUrl);
        }

        TempData["Success"] = "Registration complete. Please verify your email before signing in. (Verification link has been logged by the app.)";
        return RedirectToAction(nameof(SignIn));
    }

    [HttpGet("/confirm-email")]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            TempData["Error"] = "Invalid email confirmation request.";
            return RedirectToAction(nameof(SignIn));
        }

        var result = await userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            TempData["Error"] = "Email verification failed. Request a new verification email.";
            return RedirectToAction(nameof(SignIn));
        }

        TempData["Success"] = "Email verified successfully. You can now sign in.";
        return RedirectToAction(nameof(SignIn));
    }

    [HttpGet("/admin-login")]
    public IActionResult AdminLogin(string? returnUrl = null)
    {
        return View("Login", new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost("/admin-login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdminLogin(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Login", model);
        }

        var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Invalid admin login attempt.");
            return View("Login", model);
        }

        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        TempData["Success"] = "You have been logged out.";
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
