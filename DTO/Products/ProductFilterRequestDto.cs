using EcommerceApi.Entities;

namespace EcommerceApi.DTO.Products
{
    public class ProductFilterRequestDto
    {
        public string? Keyword { get; set; }
        public string? Brand { get; set; }
        public ProductStatus? Status { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public int PageNumber { get; set; } = 1;   // default
        public int PageSize { get; set; } = 10;    // default (safe)
    }

}
