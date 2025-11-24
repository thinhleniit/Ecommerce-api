namespace EcommerceApi.DTO.Products
{
    public class ProductDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Brand { get; set; }
        public string Status { get; set; }

        public List<ProductVariantDto> Variants { get; set; }
    }

    public class ProductVariantDto
    {
        public Guid Id { get; set; }
        public string Sku { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public bool IsDefault { get; set; }
        public string ImageUrl { get; set; }
    }
}
