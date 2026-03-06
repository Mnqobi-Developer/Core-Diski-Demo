using Core_Diski_Demo.Data;
using Core_Diski_Demo.Models.Entities;
using Core_Diski_Demo.Models.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Core_Diski_Demo.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProductsController(ApplicationDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index()
    {
        var products = await dbContext.Products
            .Include(p => p.Club).ThenInclude(c => c.League)
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .OrderByDescending(p => p.Id)
            .ToListAsync();

        return View(products);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = new ProductFormViewModel();
        await PopulateLookups(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductFormViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await PopulateLookups(vm);
            return View(vm);
        }

        var product = new Product
        {
            Name = vm.Name.Trim(),
            Price = vm.Price,
            StockQuantity = vm.StockQuantity,
            Description = vm.Description,
            ReleaseSeason = vm.ReleaseSeason,
            ShirtType = vm.ShirtType,
            ClubId = vm.ClubId,
            BrandId = vm.BrandId,
            CategoryId = vm.CategoryId
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        TempData["Success"] = "Product created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await dbContext.Products.FindAsync(id);
        if (product is null)
        {
            return NotFound();
        }

        var vm = new ProductFormViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            Description = product.Description,
            ReleaseSeason = product.ReleaseSeason,
            ShirtType = product.ShirtType,
            ClubId = product.ClubId,
            BrandId = product.BrandId,
            CategoryId = product.CategoryId
        };

        await PopulateLookups(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProductFormViewModel vm)
    {
        if (!ModelState.IsValid || vm.Id is null)
        {
            await PopulateLookups(vm);
            return View(vm);
        }

        var product = await dbContext.Products.FindAsync(vm.Id.Value);
        if (product is null)
        {
            return NotFound();
        }

        product.Name = vm.Name.Trim();
        product.Price = vm.Price;
        product.StockQuantity = vm.StockQuantity;
        product.Description = vm.Description;
        product.ReleaseSeason = vm.ReleaseSeason;
        product.ShirtType = vm.ShirtType;
        product.ClubId = vm.ClubId;
        product.BrandId = vm.BrandId;
        product.CategoryId = vm.CategoryId;

        await dbContext.SaveChangesAsync();
        TempData["Success"] = "Product updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await dbContext.Products.FindAsync(id);
        if (product is null)
        {
            TempData["Error"] = "Product not found.";
            return RedirectToAction(nameof(Index));
        }

        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync();
        TempData["Success"] = "Product deleted.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateLookups(ProductFormViewModel vm)
    {
        vm.Clubs = await dbContext.Clubs
            .OrderBy(c => c.Name)
            .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
            .ToListAsync();

        vm.Brands = await dbContext.Brands
            .OrderBy(b => b.Name)
            .Select(b => new SelectListItem(b.Name, b.Id.ToString()))
            .ToListAsync();

        vm.Categories = await dbContext.Categories
            .OrderBy(c => c.Name)
            .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
            .ToListAsync();
    }
}
