using System.ComponentModel.DataAnnotations;

namespace EcommerceApi.DTO.Cart
{
    public class UpdateCartItemDto
    {
        [Required]
        public Guid VariantId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
        public int Quantity { get; set; } // = 0 is remove
    }
}
