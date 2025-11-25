namespace EcommerceApi.Models.Cart
{
    public class CartItem
    {
        public Guid ProductId { get; set; }
        public Guid VariantId { get; set; }
        public int Quantity { get; set; }

        public string Name { get; set; } = "";
        public string Brand { get; set; } = "";
        public string? ImageUrl { get; set; }

        public decimal Price { get; set; }
    }
}
