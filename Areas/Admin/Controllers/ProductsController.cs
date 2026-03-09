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
public class ProductsController(ApplicationDbContext dbContext, IWebHostEnvironment environment) : Controller
{
    public async Task<IActionResult> Index()
    {
        var products = await dbContext.Products
            .Include(p => p.Club).ThenInclude(c => c.League)
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.Images)
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
        if (!vm.SelectedSizeIds.Any())
        {
            ModelState.AddModelError(nameof(ProductFormViewModel.SelectedSizeIds), "Select at least one available size.");
        }

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

        foreach (var sizeId in vm.SelectedSizeIds.Distinct())
        {
            dbContext.ProductSizes.Add(new ProductSize
            {
                ProductId = product.Id,
                SizeId = sizeId,
                QuantityInStock = vm.StockQuantity
            });
        }

        var imageUrl = await SaveImageAsync(vm.ImageFile);
        if (imageUrl == "__INVALID__")
        {
            await PopulateLookups(vm);
            return View(vm);
        }

        if (!string.IsNullOrWhiteSpace(imageUrl))
        {
            dbContext.ProductImages.Add(new ProductImage
            {
                ProductId = product.Id,
                ImageUrl = imageUrl,
                IsPrimary = true
            });
        }

        await dbContext.SaveChangesAsync();

        TempData["Success"] = "Product created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await dbContext.Products
            .Include(p => p.Images)
            .Include(p => p.ProductSizes)
            .FirstOrDefaultAsync(p => p.Id == id);

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
            CategoryId = product.CategoryId,
            ExistingImageUrl = product.Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl,
            SelectedSizeIds = product.ProductSizes.Select(ps => ps.SizeId).ToList()
        };

        await PopulateLookups(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProductFormViewModel vm)
    {
        if (!vm.SelectedSizeIds.Any())
        {
            ModelState.AddModelError(nameof(ProductFormViewModel.SelectedSizeIds), "Select at least one available size.");
        }

        if (!ModelState.IsValid || vm.Id is null)
        {
            await PopulateLookups(vm);
            return View(vm);
        }

        var product = await dbContext.Products
            .Include(p => p.Images)
            .Include(p => p.ProductSizes)
            .FirstOrDefaultAsync(p => p.Id == vm.Id.Value);

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

        var selected = vm.SelectedSizeIds.Distinct().ToHashSet();
        var existing = product.ProductSizes.Select(ps => ps.SizeId).ToHashSet();

        var toRemove = product.ProductSizes.Where(ps => !selected.Contains(ps.SizeId)).ToList();
        if (toRemove.Any())
        {
            dbContext.ProductSizes.RemoveRange(toRemove);
        }

        var toAdd = selected.Where(id => !existing.Contains(id)).ToList();
        foreach (var sizeId in toAdd)
        {
            product.ProductSizes.Add(new ProductSize
            {
                ProductId = product.Id,
                SizeId = sizeId,
                QuantityInStock = vm.StockQuantity
            });
        }

        foreach (var productSize in product.ProductSizes)
        {
            productSize.QuantityInStock = vm.StockQuantity;
        }

        var imageUrl = await SaveImageAsync(vm.ImageFile);
        if (imageUrl == "__INVALID__")
        {
            vm.ExistingImageUrl = product.Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl;
            await PopulateLookups(vm);
            return View(vm);
        }

        if (!string.IsNullOrWhiteSpace(imageUrl))
        {
            foreach (var image in product.Images)
            {
                image.IsPrimary = false;
            }

            product.Images.Add(new ProductImage
            {
                ImageUrl = imageUrl,
                IsPrimary = true
            });
        }

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
            .Include(c => c.League)
            .OrderBy(c => c.League.Name)
            .ThenBy(c => c.Name)
            .Select(c => new SelectListItem($"{c.Name} ({c.League.Name})", c.Id.ToString()))
            .ToListAsync();

        vm.Brands = await dbContext.Brands
            .OrderBy(b => b.Name)
            .Select(b => new SelectListItem(b.Name, b.Id.ToString()))
            .ToListAsync();

        vm.Categories = await dbContext.Categories
            .OrderBy(c => c.Name)
            .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
            .ToListAsync();

        vm.Sizes = await dbContext.Sizes
            .OrderBy(s => s.Id)
            .Select(s => new SelectListItem(s.Name, s.Id.ToString()))
            .ToListAsync();
    }

    private async Task<string?> SaveImageAsync(IFormFile? imageFile)
    {
        if (imageFile is null || imageFile.Length == 0)
        {
            return null;
        }

        var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
        var allowed = new HashSet<string> { ".jpg", ".jpeg", ".png", ".webp" };
        if (!allowed.Contains(extension))
        {
            ModelState.AddModelError(nameof(ProductFormViewModel.ImageFile), "Only .jpg, .jpeg, .png, and .webp images are allowed.");
            return "__INVALID__";
        }

        var uploadsFolder = Path.Combine(environment.WebRootPath, "images", "products");
        Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await imageFile.CopyToAsync(stream);

        return $"/images/products/{uniqueFileName}";
    }
}
