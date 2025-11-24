using System.ComponentModel.DataAnnotations;
using EcommerceApi.Validators;

namespace EcommerceApi.DTO.Variants
{
    public class CreateVariantDto
    {
        [Required]
        [MinLength(1)]
        public string Name { get; set; } = default!;

        [Required]
        [ValidSku]
        public string Sku { get; set; } = default!;

        [Range(0.01, 10000)]
        public decimal Price { get; set; }

        // Add currency back
        [Required]
        [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "Currency must be a valid ISO code (e.g., USD, VND)")]
        public string Currency { get; set; } = "USD";

        public bool IsDefault { get; set; }
    }
}
