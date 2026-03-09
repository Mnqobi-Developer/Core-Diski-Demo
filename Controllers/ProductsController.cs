using Core_Diski_Demo.Data;
using Core_Diski_Demo.Models.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Core_Diski_Demo.Controllers;

public class ProductsController(ApplicationDbContext dbContext) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(ProductIndexViewModel filter)
    {
        var query = dbContext.Products
            .AsNoTracking()
            .Include(p => p.Club).ThenInclude(c => c.League)
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.ProductSizes).ThenInclude(ps => ps.Size)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Query))
        {
            var q = filter.Query.Trim().ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(q)
                || p.Brand.Name.ToLower().Contains(q)
                || p.Club.Name.ToLower().Contains(q)
                || p.Club.League.Name.ToLower().Contains(q));
        }

        if (filter.LeagueId.HasValue)
            query = query.Where(p => p.Club.LeagueId == filter.LeagueId.Value);
        if (filter.ClubId.HasValue)
            query = query.Where(p => p.ClubId == filter.ClubId.Value);
        if (filter.BrandId.HasValue)
            query = query.Where(p => p.BrandId == filter.BrandId.Value);
        if (filter.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == filter.CategoryId.Value);
        if (filter.MinPrice.HasValue)
            query = query.Where(p => p.Price >= filter.MinPrice.Value);
        if (filter.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= filter.MaxPrice.Value);
        if (filter.SizeId.HasValue)
            query = query.Where(p => p.ProductSizes.Any(ps => ps.SizeId == filter.SizeId.Value));

        filter.Products = await query
            .OrderByDescending(p => p.Id)
            .Select(p => new ProductCardViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Club = p.Club.Name,
                League = p.Club.League.Name,
                Brand = p.Brand.Name,
                Price = p.IsOnSale && p.DiscountPercentage > 0 ? p.Price * (1 - (decimal)p.DiscountPercentage / 100) : p.Price,
                OriginalPrice = p.Price,
                IsOnSale = p.IsOnSale,
                DiscountPercentage = p.DiscountPercentage,
                StockQuantity = p.StockQuantity,
                ImageUrl = p.Images.Where(i => i.IsPrimary).Select(i => i.ImageUrl).FirstOrDefault()
            })
            .ToListAsync();

        filter.Leagues = await dbContext.Leagues.OrderBy(x => x.Name).Select(x => new LookupOptionViewModel { Id = x.Id, Name = x.Name }).ToListAsync();
        filter.Clubs = await dbContext.Clubs.OrderBy(x => x.Name).Select(x => new LookupOptionViewModel { Id = x.Id, Name = x.Name }).ToListAsync();
        filter.Brands = await dbContext.Brands.OrderBy(x => x.Name).Select(x => new LookupOptionViewModel { Id = x.Id, Name = x.Name }).ToListAsync();
        filter.Categories = await dbContext.Categories.OrderBy(x => x.Name).Select(x => new LookupOptionViewModel { Id = x.Id, Name = x.Name }).ToListAsync();
        filter.Sizes = await dbContext.Sizes.OrderBy(x => x.Name).Select(x => new LookupOptionViewModel { Id = x.Id, Name = x.Name }).ToListAsync();

        return View(filter);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var product = await dbContext.Products
            .AsNoTracking()
            .Include(p => p.Club).ThenInclude(c => c.League)
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.ProductSizes).ThenInclude(ps => ps.Size)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
            return NotFound();

        var orderedImages = product.Images.OrderByDescending(i => i.IsPrimary).ThenBy(i => i.Id).Select(i => i.ImageUrl).ToList();
        var discounted = product.IsOnSale && product.DiscountPercentage > 0
            ? product.Price * (1 - (decimal)product.DiscountPercentage / 100)
            : product.Price;

        var vm = new ProductDetailsViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Club = product.Club.Name,
            League = product.Club.League.Name,
            Brand = product.Brand.Name,
            Category = product.Category.Name,
            ShirtType = product.ShirtType,
            Price = discounted,
            OriginalPrice = product.Price,
            IsOnSale = product.IsOnSale,
            DiscountPercentage = product.DiscountPercentage,
            StockQuantity = product.StockQuantity,
            ReleaseSeason = product.ReleaseSeason,
            Description = product.Description,
            ImageUrl = orderedImages.FirstOrDefault(),
            ImageUrls = orderedImages,
            Sizes = product.ProductSizes.Select(ps => ps.Size.Name).OrderBy(x => x).ToList()
        };

        return View(vm);
    }
}
