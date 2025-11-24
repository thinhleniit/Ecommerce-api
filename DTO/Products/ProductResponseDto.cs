using EcommerceApi.DTO.Variants;
using EcommerceApi.Entities;
using EcommerceApi.Models;

namespace EcommerceApi.DTO.Products
{
    public class ProductResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public string? Description { get; set; }
        public string? Brand { get; set; }
        public ProductStatus Status { get; set; }
        public List<VariantResponseDto> Variants { get; set; } = new();
        public string? ImageUrl { get; set; }
    }

}
