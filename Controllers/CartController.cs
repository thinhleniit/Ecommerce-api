using System.Security.Claims;
using EcommerceApi.Data;
using EcommerceApi.DTO.Cart;
using EcommerceApi.Models.Cart;
using EcommerceApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/cart")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly AppDbContext _db;

    public CartController(ICartService cartService, AppDbContext db)
    {
        _cartService = cartService;
        _db = db;
    }

    private string GetUserId()
    {
        // user login → email từ JWT
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (!string.IsNullOrEmpty(email))
            return email;

        // Guest user → use cart_id cookie
        if (!Request.Cookies.TryGetValue("cart_id", out var cartId))
        {
            cartId = Guid.NewGuid().ToString();
            Response.Cookies.Append("cart_id", cartId, new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(30),
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.None
            });
        }

        return cartId;
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add(AddToCartDto dto)
    {
        var userId = GetUserId();

        var variant = await _db.ProductVariants
            .Include(v => v.Product)
            .FirstOrDefaultAsync(v => v.Id == dto.VariantId);

        if (variant == null)
            return NotFound("Variant not found");

        var item = new CartItem
        {
            ProductId = variant.ProductId,
            VariantId = variant.Id,
            Name = variant.Product.Name,
            Brand = variant.Product.Brand,
            ImageUrl = variant.Product.ImageUrl,
            Price = variant.Price,
            Quantity = dto.Quantity
        };

        await _cartService.AddItemAsync(userId, item);

        return Ok();
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateItem(UpdateCartItemDto req)
    {
        var userId = GetUserId();

        var cart = await _cartService.GetCartAsync(userId);
        var item = cart.Items.FirstOrDefault(x => x.VariantId == req.VariantId);

        if (item == null)
            return NotFound("Item not found");

        item.Quantity = req.Quantity;

        await _cartService.SaveCartAsync(userId, cart);

        return Ok(cart);
    }

    [HttpDelete("{variantId}")]
    public async Task<IActionResult> DeleteItem(Guid variantId)
    {
        var userId = GetUserId();

        await _cartService.RemoveItemAsync(userId, variantId);

        var cart = await _cartService.GetCartAsync(userId);
        return Ok(cart);
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = GetUserId();
        var cart = await _cartService.GetCartAsync(userId);
        return Ok(cart);
    }

    [HttpPost("clear")]
    public async Task<IActionResult> ClearCart()
    {
        var userId = GetUserId();
        await _cartService.ClearCartAsync(userId);

        return Ok(new { message = "Cart cleared" });
    }
}
