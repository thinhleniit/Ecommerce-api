using System.ComponentModel.DataAnnotations;
using EcommerceApi.Entities;
using EcommerceApi.DTO.Variants;

namespace EcommerceApi.DTO.Products
{
    public class CreateProductDto
    {
        [Required]
        [MinLength(3)]
        public string Name { get; set; } = default!;

        [Required]
        [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Slug only allows lowercase letters, numbers, and hyphens.")]
        public string Slug { get; set; } = default!;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [EnumDataType(typeof(ProductStatus), ErrorMessage = "Invalid status value.")]
        public ProductStatus? Status { get; set; }  // ← FIXED

        [Required]
        [MinLength(2)]
        public string Brand { get; set; } = default!;

        public List<CreateVariantDto> Variants { get; set; } = new();

        [Url]
        public string? ImageUrl { get; set; }
    }
}
