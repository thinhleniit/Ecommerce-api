using System.ComponentModel.DataAnnotations;
using EcommerceApi.Models;

namespace EcommerceApi.Entities
{
    public class Inventory
    {
        [Key]
        public Guid VariantId { get; set; }
        public ProductVariant Variant { get; set; } = default!;

        public int AvailableQuantity { get; set; }
        public int ReservedQuantity { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = default!;
    }

}
