namespace Core_Diski_Demo.Models.Entities;

public class ProductSize
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int SizeId { get; set; }
    public Size Size { get; set; } = null!;

    public int QuantityInStock { get; set; }
}
