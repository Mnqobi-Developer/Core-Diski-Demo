using Core_Diski_Demo.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Core_Diski_Demo.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DashboardController(ApplicationDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index()
    {
        ViewBag.ProductsCount = await dbContext.Products.CountAsync();
        ViewBag.LeaguesCount = await dbContext.Leagues.CountAsync();
        ViewBag.ClubsCount = await dbContext.Clubs.CountAsync();
        return View();
    }
}
