namespace Core_Diski_Demo.Models.ViewModels.Cart;

public class CartViewModel
{
    public List<CartItemViewModel> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
}

public class CartItemViewModel
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ClubName { get; set; } = string.Empty;
    public string LeagueName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int Quantity { get; set; }
    public int AvailableStock { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
}
