using EcommerceApi.Entities;

namespace EcommerceApi.Models
{
    public class ProductVariant
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;

        public string Sku { get; set; } = default!;
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }
        public string Currency { get; set; } = "VND";
        public bool IsDefault { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Inventory Inventory { get; set; } = default!;

        public string ImageUrl { get; set; } = "/uploads/default.png";

    }
}
