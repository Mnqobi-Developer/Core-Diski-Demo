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
        ValidateBusinessRules(vm, existingImageCount: 0);

        if (!ModelState.IsValid)
        {
            await PopulateLookups(vm);
            return View(vm);
        }

        var images = await SaveImagesAsync(vm.ImageFiles);
        if (images is null)
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
            IsOnSale = vm.IsOnSale,
            DiscountPercentage = vm.IsOnSale ? vm.DiscountPercentage : 0,
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

        for (var i = 0; i < images.Count; i++)
        {
            dbContext.ProductImages.Add(new ProductImage
            {
                ProductId = product.Id,
                ImageUrl = images[i],
                IsPrimary = i == 0
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
            IsOnSale = product.IsOnSale,
            DiscountPercentage = product.DiscountPercentage,
            ClubId = product.ClubId,
            BrandId = product.BrandId,
            CategoryId = product.CategoryId,
            ExistingImageUrls = product.Images.OrderByDescending(i => i.IsPrimary).Select(i => i.ImageUrl).ToList(),
            SelectedSizeIds = product.ProductSizes.Select(ps => ps.SizeId).ToList()
        };

        await PopulateLookups(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProductFormViewModel vm)
    {
        if (vm.Id is null)
        {
            return NotFound();
        }

        var product = await dbContext.Products
            .Include(p => p.Images)
            .Include(p => p.ProductSizes)
            .FirstOrDefaultAsync(p => p.Id == vm.Id.Value);

        if (product is null)
        {
            return NotFound();
        }

        vm.ExistingImageUrls = product.Images.OrderByDescending(i => i.IsPrimary).Select(i => i.ImageUrl).ToList();
        ValidateBusinessRules(vm, existingImageCount: vm.ExistingImageUrls.Count);

        if (!ModelState.IsValid)
        {
            await PopulateLookups(vm);
            return View(vm);
        }

        product.Name = vm.Name.Trim();
        product.Price = vm.Price;
        product.StockQuantity = vm.StockQuantity;
        product.Description = vm.Description;
        product.ReleaseSeason = vm.ReleaseSeason;
        product.ShirtType = vm.ShirtType;
        product.IsOnSale = vm.IsOnSale;
        product.DiscountPercentage = vm.IsOnSale ? vm.DiscountPercentage : 0;
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

        var images = await SaveImagesAsync(vm.ImageFiles);
        if (images is null)
        {
            await PopulateLookups(vm);
            return View(vm);
        }

        if (images.Any())
        {
            var hasPrimary = product.Images.Any(i => i.IsPrimary);
            foreach (var imageUrl in images)
            {
                product.Images.Add(new ProductImage
                {
                    ImageUrl = imageUrl,
                    IsPrimary = !hasPrimary
                });
                hasPrimary = true;
            }
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


    private void ValidateBusinessRules(ProductFormViewModel vm, int existingImageCount)
    {
        if (!vm.SelectedSizeIds.Any())
        {
            ModelState.AddModelError(nameof(ProductFormViewModel.SelectedSizeIds), "Select at least one available size.");
        }

        if (vm.IsOnSale && vm.DiscountPercentage <= 0)
        {
            ModelState.AddModelError(nameof(ProductFormViewModel.DiscountPercentage), "Enter a discount % greater than 0 when item is on sale.");
        }

        var uploadedCount = vm.ImageFiles?.Count(f => f is { Length: > 0 }) ?? 0;
        var total = existingImageCount + uploadedCount;
        if (total < 3 || total > 5)
        {
            ModelState.AddModelError(nameof(ProductFormViewModel.ImageFiles), "Each jersey must have between 3 and 5 images in total.");
        }
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

    private async Task<List<string>?> SaveImagesAsync(List<IFormFile> imageFiles)
    {
        var files = imageFiles.Where(f => f is { Length: > 0 }).ToList();
        var urls = new List<string>();

        foreach (var imageFile in files)
        {
            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            var allowed = new HashSet<string> { ".jpg", ".jpeg", ".png", ".webp" };
            if (!allowed.Contains(extension))
            {
                ModelState.AddModelError(nameof(ProductFormViewModel.ImageFiles), "Only .jpg, .jpeg, .png, and .webp images are allowed.");
                return null;
            }

            var uploadsFolder = Path.Combine(environment.WebRootPath, "images", "products");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await imageFile.CopyToAsync(stream);

            urls.Add($"/images/products/{uniqueFileName}");
        }

        return urls;
    }
}
