using System.ComponentModel.DataAnnotations;

namespace EcommerceApi.DTO.Cart
{
    public class AddToCartDto
    {
        [Required]
        public Guid VariantId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
}
