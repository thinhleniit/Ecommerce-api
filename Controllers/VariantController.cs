using EcommerceApi.Data;
using EcommerceApi.DTO.Inventory;
using EcommerceApi.DTO.Variants;
using EcommerceApi.Entities;
using EcommerceApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApi.Controllers
{
    [ApiController]
    [Route("api/variants")]
    public class VariantController : ControllerBase
    {
        private readonly AppDbContext _db;

        public VariantController(AppDbContext db)
        {
            _db = db;
        }

        // ADD VARIANT TO PRODUCT
        [HttpPost("{productId}")]
        public async Task<IActionResult> AddVariant(Guid productId, [FromBody] CreateVariantDto dto)
        {
            if (!await _db.Products.AnyAsync(p => p.Id == productId))
                return NotFound("Product not found.");

            if (await _db.ProductVariants.AnyAsync(x => x.Sku == dto.Sku))
                return BadRequest("SKU already exists.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var variant = new ProductVariant
            {
                ProductId = productId,
                Sku = dto.Sku,
                Name = dto.Name,
                Price = dto.Price,
                Currency = dto.Currency,
                IsDefault = dto.IsDefault,
                Inventory = new Inventory
                {
                    AvailableQuantity = 0,
                    ReservedQuantity = 0
                }
            };

            _db.ProductVariants.Add(variant);
            await _db.SaveChangesAsync();

            return Ok(variant.Id);
        }

        [HttpGet("{variantId}/detail")]
        public async Task<IActionResult> GetDetail(Guid variantId)
        {
            var variant = await _db.ProductVariants
                .Include(v => v.Inventory)
                .FirstOrDefaultAsync(v => v.Id == variantId);

            if (variant == null)
                return NotFound();
            
            var response = new VariantResponseDto
            {
                Id = variant.Id,
                ProductId = variant.ProductId,
                Sku = variant.Sku,
                Name = variant.Name,
                Price = variant.Price,
                Currency = variant.Currency,
                IsDefault = variant.IsDefault,

                Inventory = variant.Inventory == null
                    ? null
                    : new InventoryResponseDto
                    {
                        AvailableQuantity = variant.Inventory.AvailableQuantity,
                        ReservedQuantity = variant.Inventory.ReservedQuantity,
                        RowVersion = Convert.ToBase64String(variant.Inventory.RowVersion) // 👈 IMPORTANT
                    }
            };

            return Ok(response);
        }

        [HttpPut("{variantId}")]
        public async Task<IActionResult> UpdateVariant(Guid variantId, [FromBody] CreateVariantDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var variant = await _db.ProductVariants.FindAsync(variantId);
            if (variant == null) return NotFound();

            variant.Name = dto.Name;
            variant.Price = dto.Price;
            variant.Currency = dto.Currency;
            variant.IsDefault = dto.IsDefault;
            variant.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return Ok(variant);
        }

        [HttpDelete("{variantId}")]
        public async Task<IActionResult> DeleteVariant(Guid variantId)
        {
            var variant = await _db.ProductVariants.FindAsync(variantId);
            if (variant == null) return NotFound();

            _db.ProductVariants.Remove(variant);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }

}
