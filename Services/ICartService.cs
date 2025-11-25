using EcommerceApi.Models.Cart;

namespace EcommerceApi.Services
{
    public interface ICartService
    {
        Task<Cart> GetCartAsync(string userId);
        Task AddItemAsync(string userId, CartItem item);
        Task RemoveItemAsync(string userId, Guid variantId);
        Task ClearCartAsync(string userId);
        Task SaveCartAsync(string userId, Cart cart);
    }
}
