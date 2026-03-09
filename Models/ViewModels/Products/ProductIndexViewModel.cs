namespace Core_Diski_Demo.Models.ViewModels.Products;

public class ProductIndexViewModel
{
    public string? Query { get; set; }
    public int? LeagueId { get; set; }
    public int? ClubId { get; set; }
    public int? BrandId { get; set; }
    public int? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? SizeId { get; set; }

    public List<ProductCardViewModel> Products { get; set; } = new();
    public List<LookupOptionViewModel> Leagues { get; set; } = new();
    public List<LookupOptionViewModel> Clubs { get; set; } = new();
    public List<LookupOptionViewModel> Brands { get; set; } = new();
    public List<LookupOptionViewModel> Categories { get; set; } = new();
    public List<LookupOptionViewModel> Sizes { get; set; } = new();
}

public class ProductCardViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Club { get; set; } = string.Empty;
    public string League { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsOnSale { get; set; }
    public int DiscountPercentage { get; set; }
    public decimal OriginalPrice { get; set; }
    public int StockQuantity { get; set; }
    public string? ImageUrl { get; set; }
}

public class LookupOptionViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
