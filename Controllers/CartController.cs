using System.Security.Claims;
using EcommerceApi.DTO.Cart;
using EcommerceApi.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/cart")]
public class CartController : ControllerBase
{
    private readonly CartService _cartService;

    public CartController(CartService cartService)
    {
        _cartService = cartService;
    }

    private string GetUserId()
    {
        // User login → email
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (!string.IsNullOrEmpty(email)) return email;

        // User not login → fallback guest cart id
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
    public async Task<IActionResult> AddToCart(AddToCartDto req)
    {
        var userId = GetUserId();
        var cart = await _cartService.AddItemAsync(userId, new CartItemDto
        {
            VariantId = req.VariantId,
            Quantity = req.Quantity
        });
        return Ok(cart);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateItem(UpdateCartItemDto req)
    {
        var userId = GetUserId();
        var cart = await _cartService.UpdateQuantityAsync(userId, req.VariantId, req.Quantity);
        return Ok(cart);
    }

    [HttpDelete("{variantId}")]
    public async Task<IActionResult> DeleteItem(Guid variantId)
    {
        var userId = GetUserId();
        var cart = await _cartService.RemoveItemAsync(userId, variantId);
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
        var cart = new CartDto();           

        await _cartService.SaveCartAsync(userId, cart);   

        return Ok(cart);
    }
}
