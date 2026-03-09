using Core_Diski_Demo.Services;
using Microsoft.AspNetCore.Mvc;

namespace Core_Diski_Demo.Controllers;

public class CartController(ICartService cartService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var vm = await cartService.GetCartAsync();
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productId, int quantity = 1)
    {
        var result = await cartService.AddToCartAsync(productId, quantity);
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction("Details", "Products", new { id = productId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int productId, int quantity)
    {
        var result = await cartService.UpdateQuantityAsync(productId, quantity);
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int productId)
    {
        await cartService.RemoveItemAsync(productId);
        TempData["Success"] = "Item removed from cart.";
        return RedirectToAction(nameof(Index));
    }
}
