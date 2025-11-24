using EcommerceApi.Data;
using EcommerceApi.DTO.Products;
using EcommerceApi.Entities;
using EcommerceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApi.Services
{
    public class ProductService
    {
        private readonly AppDbContext _db;

        public ProductService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Product?> GetById(Guid id)
        {
            return await _db.Products
                .Include(x => x.Variants)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Guid> Create(CreateProductDto dto)
        {
            if (await _db.Products.AnyAsync(x => x.Slug == dto.Slug))
                throw new Exception("Slug already exists");

            var p = new Product
            {
                Name = dto.Name,
                Slug = dto.Slug,
                Description = dto.Description,
                Brand = dto.Brand,
                Status = dto.Status!.Value
            };

            foreach (var v in dto.Variants)
            {
                p.Variants.Add(new ProductVariant
                {
                    Sku = v.Sku,
                    Name = v.Name,
                    Price = v.Price,
                    Currency = v.Currency,
                    IsDefault = v.IsDefault
                });
            }

            _db.Products.Add(p);
            await _db.SaveChangesAsync();

            return p.Id;
        }

        public async Task Update(Guid id, UpdateProductDto dto)
        {
            var p = await _db.Products.FindAsync(id);
            if (p == null) throw new Exception("Product not found");

            p.Name = dto.Name;
            p.Slug = dto.Slug;
            p.Description = dto.Description;
            p.Brand = dto.Brand;
            p.Status = dto.Status;
            p.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var p = await _db.Products.FindAsync(id);
            if (p == null) throw new Exception("Product not found");

            p.Status = ProductStatus.Archived;
            await _db.SaveChangesAsync();
        }
    }
}
