using System.Text.Json;
using EcommerceApi.Models;
using EcommerceApi.Models.Cart;
using Microsoft.Extensions.Caching.Distributed;

namespace EcommerceApi.Services
{
    public class RedisCartService : ICartService
    {
        private readonly IDistributedCache _cache;

        public RedisCartService(IDistributedCache cache)
        {
            _cache = cache;
        }

        private string Key(string userId) => $"cart:{userId}";

        public async Task<Cart> GetCartAsync(string userId)
        {
            var raw = await _cache.GetStringAsync(Key(userId));

            if (string.IsNullOrEmpty(raw))
                return new Cart { UserId = userId };

            return JsonSerializer.Deserialize<Cart>(raw) ?? new Cart { UserId = userId };
        }

        private async Task Save(Cart cart)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
            };

            var json = JsonSerializer.Serialize(cart);
            await _cache.SetStringAsync(Key(cart.UserId), json, options);
        }

        public async Task AddItemAsync(string userId, CartItem item)
        {
            var cart = await GetCartAsync(userId);

            var exist = cart.Items.FirstOrDefault(x => x.VariantId == item.VariantId);
            if (exist == null)
            {
                cart.Items.Add(item);
            }
            else
            {
                exist.Quantity += item.Quantity;
            }

            await Save(cart);
        }

        public async Task RemoveItemAsync(string userId, Guid variantId)
        {
            var cart = await GetCartAsync(userId);

            var exist = cart.Items.FirstOrDefault(x => x.VariantId == variantId);
            if (exist != null)
            {
                cart.Items.Remove(exist);
            }

            await Save(cart);
        }

        public async Task ClearCartAsync(string userId)
        {
            await _cache.RemoveAsync(Key(userId));
        }

        public async Task SaveCartAsync(string userId, Cart cart)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
            };

            var json = JsonSerializer.Serialize(cart);
            await _cache.SetStringAsync(Key(userId), json, options);
        }

    }
}
