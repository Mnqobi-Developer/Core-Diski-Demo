using Core_Diski_Demo.Models.Entities;
using Core_Diski_Demo.Models.ViewModels.Checkout;
using Core_Diski_Demo.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Core_Diski_Demo.Controllers;

public class CheckoutController(
    ICartService cartService,
    IPaymentGatewayService paymentGatewayService,
    UserManager<ApplicationUser> userManager) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var access = await EnsureVerifiedUserAsync();
        if (access is not null)
        {
            return access;
        }

        var cart = await cartService.GetCartAsync();
        if (!cart.Items.Any())
        {
            TempData["Error"] = "Your cart is empty.";
            return RedirectToAction("Index", "Cart");
        }

        return View(new CheckoutViewModel { TotalAmount = cart.TotalAmount });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(CheckoutViewModel vm)
    {
        var access = await EnsureVerifiedUserAsync();
        if (access is not null)
        {
            return access;
        }

        var cart = await cartService.GetCartAsync();
        if (!cart.Items.Any())
        {
            TempData["Error"] = "Your cart is empty.";
            return RedirectToAction("Index", "Cart");
        }

        if (!ModelState.IsValid)
        {
            vm.TotalAmount = cart.TotalAmount;
            return View(vm);
        }

        var orderReference = $"FSS-{DateTime.UtcNow:yyyyMMddHHmmss}";
        var paymentResult = paymentGatewayService.Initiate(vm.PaymentMethod, cart.TotalAmount, orderReference);

        var confirmation = new CheckoutConfirmationViewModel
        {
            OrderReference = paymentResult.OrderReference,
            TotalAmount = paymentResult.Amount,
            ShippingAddress = vm.ShippingAddress,
            PaymentMethod = paymentResult.Method,
            GatewayUrl = paymentResult.GatewayUrl,
            Message = paymentResult.Message
        };

        TempData["CheckoutConfirmation"] = System.Text.Json.JsonSerializer.Serialize(confirmation);

        await cartService.ClearAsync();
        return RedirectToAction(nameof(Confirmation));
    }

    [HttpGet]
    public IActionResult Confirmation()
    {
        if (TempData["CheckoutConfirmation"] is not string payload)
        {
            return RedirectToAction("Index", "Home");
        }

        var vm = System.Text.Json.JsonSerializer.Deserialize<CheckoutConfirmationViewModel>(payload);
        if (vm is null)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(vm);
    }

    private async Task<IActionResult?> EnsureVerifiedUserAsync()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            TempData["Error"] = "Please sign in to access checkout.";
            return Redirect($"/sign-in?returnUrl={Uri.EscapeDataString(Request.Path)}");
        }

        var user = await userManager.GetUserAsync(User);
        if (user is null || !user.EmailConfirmed)
        {
            TempData["Error"] = "Please verify your email before checkout.";
            return Redirect($"/sign-in?returnUrl={Uri.EscapeDataString(Request.Path)}");
        }

        return null;
    }
}
