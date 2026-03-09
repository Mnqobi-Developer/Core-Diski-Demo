using System.Text.Json;
using Core_Diski_Demo.Data;
using Core_Diski_Demo.Models.ViewModels.Cart;
using Microsoft.EntityFrameworkCore;

namespace Core_Diski_Demo.Services;

public class SessionCartService(IHttpContextAccessor contextAccessor, ApplicationDbContext dbContext) : ICartService
{
    private const string SessionKey = "CUSTOMER_CART";

    public async Task<CartViewModel> GetCartAsync()
    {
        var sessionCart = GetSessionCart();
        var productIds = sessionCart.Items.Select(i => i.ProductId).Distinct().ToList();

        var products = await dbContext.Products
            .Include(p => p.Images)
            .Include(p => p.Club).ThenInclude(c => c.League)
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync();

        var items = sessionCart.Items
            .Select(item =>
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product is null)
                {
                    return null;
                }

                var unitPrice = product.Price;
                return new CartItemViewModel
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ClubName = product.Club.Name,
                    LeagueName = product.Club.League.Name,
                    ImageUrl = product.Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl,
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice,
                    Subtotal = unitPrice * item.Quantity,
                    AvailableStock = product.StockQuantity
                };
            })
            .Where(x => x is not null)
            .Cast<CartItemViewModel>()
            .ToList();

        return new CartViewModel
        {
            Items = items,
            TotalAmount = items.Sum(i => i.Subtotal)
        };
    }

    public async Task<(bool Success, string Message)> AddToCartAsync(int productId, int quantity)
    {
        if (quantity <= 0)
        {
            return (false, "Quantity must be at least 1.");
        }

        var product = await dbContext.Products.FindAsync(productId);
        if (product is null)
        {
            return (false, "Product not found.");
        }

        if (product.StockQuantity <= 0)
        {
            return (false, "This shirt is currently out of stock.");
        }

        var sessionCart = GetSessionCart();
        var existing = sessionCart.Items.FirstOrDefault(i => i.ProductId == productId);

        var requestedQty = (existing?.Quantity ?? 0) + quantity;
        if (requestedQty > product.StockQuantity)
        {
            return (false, $"Only {product.StockQuantity} items available in stock.");
        }

        if (existing is null)
        {
            sessionCart.Items.Add(new SessionCartItem { ProductId = productId, Quantity = quantity });
        }
        else
        {
            existing.Quantity = requestedQty;
        }

        SaveSessionCart(sessionCart);
        return (true, "Item added to cart.");
    }

    public async Task<(bool Success, string Message)> UpdateQuantityAsync(int productId, int quantity)
    {
        var sessionCart = GetSessionCart();
        var existing = sessionCart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (existing is null)
        {
            return (false, "Cart item not found.");
        }

        if (quantity <= 0)
        {
            sessionCart.Items.Remove(existing);
            SaveSessionCart(sessionCart);
            return (true, "Item removed from cart.");
        }

        var product = await dbContext.Products.FindAsync(productId);
        if (product is null)
        {
            return (false, "Product not found.");
        }

        if (quantity > product.StockQuantity)
        {
            return (false, $"Only {product.StockQuantity} items available in stock.");
        }

        existing.Quantity = quantity;
        SaveSessionCart(sessionCart);
        return (true, "Cart updated.");
    }

    public Task RemoveItemAsync(int productId)
    {
        var sessionCart = GetSessionCart();
        var existing = sessionCart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (existing is not null)
        {
            sessionCart.Items.Remove(existing);
            SaveSessionCart(sessionCart);
        }

        return Task.CompletedTask;
    }

    public Task ClearAsync()
    {
        SaveSessionCart(new SessionCart());
        return Task.CompletedTask;
    }

    private SessionCart GetSessionCart()
    {
        var json = contextAccessor.HttpContext?.Session.GetString(SessionKey);
        return string.IsNullOrWhiteSpace(json)
            ? new SessionCart()
            : JsonSerializer.Deserialize<SessionCart>(json) ?? new SessionCart();
    }

    private void SaveSessionCart(SessionCart cart)
    {
        var json = JsonSerializer.Serialize(cart);
        contextAccessor.HttpContext?.Session.SetString(SessionKey, json);
    }
}
