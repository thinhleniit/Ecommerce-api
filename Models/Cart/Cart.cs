namespace EcommerceApi.Models.Cart
{
    public class Cart
    {
        public string UserId { get; set; } = "";
        public List<CartItem> Items { get; set; } = new();
    }
}
