using EcommerceApi.Entities;

namespace EcommerceApi.DTO.Products
{
    public class UpdateProductDto
    {
        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public string? Description { get; set; }
        public string? Brand { get; set; }
        public ProductStatus Status { get; set; }
    }
}
