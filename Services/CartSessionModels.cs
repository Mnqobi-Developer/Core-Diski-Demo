namespace Core_Diski_Demo.Services;

public class SessionCart
{
    public List<SessionCartItem> Items { get; set; } = new();
}

public class SessionCartItem
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
