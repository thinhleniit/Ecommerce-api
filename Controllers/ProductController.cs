using EcommerceApi.Data;
using EcommerceApi.DTO.Common;
using EcommerceApi.DTO.Products;
using EcommerceApi.DTO.Variants;
using EcommerceApi.Entities;
using EcommerceApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApi.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            if (await _db.Products.AnyAsync(x => x.Slug == dto.Slug))
                return BadRequest("Slug already exists.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (dto.Variants == null || !dto.Variants.Any())
                return BadRequest("Product must include at least 1 variant.");
            if (dto.Variants.Count(v => v.IsDefault) > 1)
                return BadRequest("Only one default variant is allowed.");

            var product = new Product
            {
                Name = dto.Name,
                Slug = dto.Slug,
                Description = dto.Description,
                Brand = dto.Brand,
                Status = dto.Status!.Value
            };

            foreach (var v in dto.Variants)
            {
                product.Variants.Add(new ProductVariant
                {
                    Sku = v.Sku,
                    Name = v.Name,
                    Price = v.Price,
                    Currency = v.Currency,
                    IsDefault = v.IsDefault
                });
            }

            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            return Created($"/api/products/{product.Id}", product.Id);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var product = await _db.Products
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            var result = new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Slug = product.Slug,
                Description = product.Description,
                Brand = product.Brand,
                Status = product.Status,
                ImageUrl = product.ImageUrl,
                Variants = product.Variants.Select(v => new VariantResponseDto
                {
                    Id = v.Id,
                    Sku = v.Sku,
                    Name = v.Name,
                    Price = v.Price,
                    Currency = v.Currency,
                    IsDefault = v.IsDefault
                }).ToList()
            };

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.Name = dto.Name;
            product.Slug = dto.Slug;
            product.Description = dto.Description;
            product.Brand = dto.Brand;
            product.Status = dto.Status!.Value;
            product.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return Ok(product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.Status = ProductStatus.Archived;
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] ProductFilterRequestDto filter)
        {
            var query = _db.Products
                .AsNoTracking()
                .Include(p => p.Variants)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var keyword = filter.Keyword.Trim().ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(keyword) ||
                    p.Slug.Contains(keyword));
            }

            if (!string.IsNullOrWhiteSpace(filter.Brand))
                query = query.Where(p => p.Brand == filter.Brand);

            if (filter.Status != null)
                query = query.Where(p => p.Status == filter.Status);

            if (filter.MinPrice != null)
                query = query.Where(p => p.Variants.Any(v => v.Price >= filter.MinPrice));

            if (filter.MaxPrice != null)
                query = query.Where(p => p.Variants.Any(v => v.Price <= filter.MaxPrice));

            var totalItems = await query.CountAsync();

            var products = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var result = new PagedResult<ProductResponseDto>
            {
                TotalItems = totalItems,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Items = products.Select(p => new ProductResponseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Slug = p.Slug,
                    Description = p.Description,
                    Brand = p.Brand,
                    Status = p.Status,
                    ImageUrl = p.ImageUrl,
                    Variants = p.Variants.Select(v => new VariantResponseDto
                    {
                        Id = v.Id,
                        Sku = v.Sku,
                        Name = v.Name,
                        Price = v.Price,
                        Currency = v.Currency,
                        IsDefault = v.IsDefault
                    }).ToList()
                }).ToList()
            };

            return Ok(result);
        }

        [HttpPost("{id}/upload-image")]
        public async Task<IActionResult> UploadImage(Guid id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var product = await _db.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // create file random
            var filename = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, filename);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            product.ImageUrl = $"/uploads/{filename}";
            await _db.SaveChangesAsync();

            return Ok(new { imageUrl = product.ImageUrl });
        }

    }
}
