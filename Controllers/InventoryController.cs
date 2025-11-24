using EcommerceApi.Data;
using EcommerceApi.DTO.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApi.Controllers
{
    [ApiController]
    [Route("api/inventory")]
    public class InventoryController : ControllerBase
    {
        private readonly AppDbContext _db;

        public InventoryController(AppDbContext db)
        {
            _db = db;
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPost("{variantId}/adjust")]
        public async Task<IActionResult> AdjustInventory(Guid variantId, [FromBody] AdjustInventoryDto dto)
        {
            var inventory = await _db.Inventories
                .FirstOrDefaultAsync(i => i.VariantId == variantId);

            if (inventory == null)
                return NotFound("Inventory not found for this variant.");

            // Check optimistic concurrency
            if (!inventory.RowVersion.SequenceEqual(dto.RowVersion))
            {
                return Conflict(new
                {
                    message = "Inventory was updated by another process.",
                    currentRowVersion = inventory.RowVersion
                });
            }

            // Validate stock cannot be negative
            if (inventory.AvailableQuantity + dto.Delta < 0)
            {
                return BadRequest("Insufficient stock.");
            }

            // Update inventory
            inventory.AvailableQuantity += dto.Delta;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("Concurrency conflict occurred.");
            }

            return Ok(new
            {
                newQuantity = inventory.AvailableQuantity,
                rowVersion = inventory.RowVersion
            });
        }
    }

}
