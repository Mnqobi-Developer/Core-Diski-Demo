using Core_Diski_Demo.Data;
using Core_Diski_Demo.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Core_Diski_Demo.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CatalogController(ApplicationDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index()
    {
        ViewBag.Leagues = await dbContext.Leagues.OrderBy(x => x.Name).ToListAsync();
        ViewBag.Brands = await dbContext.Brands.OrderBy(x => x.Name).ToListAsync();
        ViewBag.Categories = await dbContext.Categories.OrderBy(x => x.Name).ToListAsync();
        ViewBag.Clubs = await dbContext.Clubs.Include(c => c.League).OrderBy(x => x.Name).ToListAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddLeague(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            TempData["Error"] = "League name is required.";
            return RedirectToAction(nameof(Index));
        }

        dbContext.Leagues.Add(new League { Name = name.Trim() });
        await dbContext.SaveChangesAsync();
        TempData["Success"] = "League added.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddBrand(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            TempData["Error"] = "Brand name is required.";
            return RedirectToAction(nameof(Index));
        }

        dbContext.Brands.Add(new Brand { Name = name.Trim() });
        await dbContext.SaveChangesAsync();
        TempData["Success"] = "Brand added.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCategory(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            TempData["Error"] = "Category name is required.";
            return RedirectToAction(nameof(Index));
        }

        dbContext.Categories.Add(new Category { Name = name.Trim() });
        await dbContext.SaveChangesAsync();
        TempData["Success"] = "Category added.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddClub(string name, int leagueId, string? country)
    {
        if (string.IsNullOrWhiteSpace(name) || leagueId <= 0)
        {
            TempData["Error"] = "Club name and league are required.";
            return RedirectToAction(nameof(Index));
        }

        dbContext.Clubs.Add(new Club
        {
            Name = name.Trim(),
            LeagueId = leagueId,
            Country = string.IsNullOrWhiteSpace(country) ? null : country.Trim()
        });

        await dbContext.SaveChangesAsync();
        TempData["Success"] = "Club added.";
        return RedirectToAction(nameof(Index));
    }
}
