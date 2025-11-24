namespace EcommerceApi.DTO.Inventory
{
    public class InventoryResponseDto
    {
        public int AvailableQuantity { get; set; }
        public int ReservedQuantity { get; set; }
        public string RowVersion { get; set; } = default!; // base64 string
    }

}
