using Core_Diski_Demo.Models.Entities;

namespace Core_Diski_Demo.Models.ViewModels.Products;

public class ProductDetailsViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Club { get; set; } = string.Empty;
    public string League { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public ShirtType ShirtType { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? ReleaseSeason { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public List<string> Sizes { get; set; } = new();
}
