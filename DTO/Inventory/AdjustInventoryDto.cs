namespace EcommerceApi.DTO.Inventory
{
    public class AdjustInventoryDto
    {
        public int Delta { get; set; }  // + adds stock, - reduces stock
        public byte[] RowVersion { get; set; } = default!;
    }

}
