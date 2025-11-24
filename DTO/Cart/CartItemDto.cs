namespace EcommerceApi.DTO.Cart
{
    public class CartItemDto
    {
        public Guid ProductId { get; set; }
        public Guid VariantId { get; set; }

        public string Name { get; set; }
        public string Brand { get; set; }
        public string ImageUrl { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}
