using EcommerceApi.DTO.Inventory;

namespace EcommerceApi.DTO.Variants
{
    public class VariantResponseDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string Sku { get; set; } = default!;
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }
        public string Currency { get; set; } = "USD";
        public bool IsDefault { get; set; }

        public InventoryResponseDto? Inventory { get; set; }
    }
}
