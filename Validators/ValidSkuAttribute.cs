using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace EcommerceApi.Validators
{
    public class ValidSkuAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var sku = value?.ToString();

            if (string.IsNullOrWhiteSpace(sku))
                return new ValidationResult("SKU is required.");

            if (!Regex.IsMatch(sku, @"^[A-Za-z0-9-]+$"))
                return new ValidationResult("SKU only allows letters, numbers and hyphens.");

            return ValidationResult.Success!;
        }
    }

}
