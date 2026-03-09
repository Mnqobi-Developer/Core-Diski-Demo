using Core_Diski_Demo.Models.ViewModels.Cart;

namespace Core_Diski_Demo.Services;

public interface ICartService
{
    Task<CartViewModel> GetCartAsync();
    Task<(bool Success, string Message)> AddToCartAsync(int productId, int quantity);
    Task<(bool Success, string Message)> UpdateQuantityAsync(int productId, int quantity);
    Task RemoveItemAsync(int productId);
    Task ClearAsync();
}
