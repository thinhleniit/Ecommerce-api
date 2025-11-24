using System.Text.Json;
using EcommerceApi.Data;
using EcommerceApi.DTO.Cart;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace EcommerceApi.Services
{
    public class CartService
    {
        private readonly IDistributedCache _cache;
        private readonly AppDbContext _db;

        public CartService(IDistributedCache cache, AppDbContext db)
        {
            _cache = cache;
            _db = db;
        }

        private string GetCartKey(string userId) => $"cart:{userId}";

        public async Task<CartDto> GetCartAsync(string userId)
        {
            var key = GetCartKey(userId);
            var data = await _cache.GetStringAsync(key);

            if (string.IsNullOrEmpty(data))
            {
                return new CartDto();
            }

            var cart = JsonSerializer.Deserialize<CartDto>(data);
            return cart ?? new CartDto();
        }

        public async Task SaveCartAsync(string userId, CartDto cart)
        {
            var key = GetCartKey(userId);
            var json = JsonSerializer.Serialize(cart);

            await _cache.SetStringAsync(
                key,
                json,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
                }
            );
        }

        public async Task<CartDto> AddItemAsync(string userId, CartItemDto newItem)
        {
            var cart = await GetCartAsync(userId);

            var existing = cart.Items.FirstOrDefault(x => x.VariantId == newItem.VariantId);

            if (existing != null)
            {
                existing.Quantity += newItem.Quantity;
            }
            else
            {
                var variant = await _db.ProductVariants
                    .Include(v => v.Product)
                    .FirstOrDefaultAsync(v => v.Id == newItem.VariantId);

                if (variant == null)
                    throw new Exception("Variant not found");

                newItem.ProductId = variant.ProductId;
                newItem.Name = variant.Product.Name;
                newItem.Brand = variant.Product.Brand ?? "";
                newItem.ImageUrl = variant.Product.ImageUrl ?? "";
                newItem.Price = variant.Price;

                cart.Items.Add(newItem);
            }

            await SaveCartAsync(userId, cart);
            return cart;
        }

        public async Task<CartDto> UpdateQuantityAsync(string userId, Guid variantId, int quantity)
        {
            var cart = await GetCartAsync(userId);

            var item = cart.Items.FirstOrDefault(x => x.VariantId == variantId);
            if (item == null)
                return cart;

            if (quantity <= 0)
            {                
                cart.Items.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
            }

            await SaveCartAsync(userId, cart);
            return cart;
        }

        public async Task<CartDto> RemoveItemAsync(string userId, Guid variantId)
        {
            var cart = await GetCartAsync(userId);

            cart.Items.RemoveAll(x => x.VariantId == variantId);

            await SaveCartAsync(userId, cart);
            return cart;
        }
    }
}
