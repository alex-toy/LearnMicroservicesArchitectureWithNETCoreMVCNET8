using Mango.Services.ProductAPI.Models.Dto;
using System.ComponentModel.DataAnnotations;

namespace Mango.Services.ProductAPI.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        public string Name { get; set; }
        [Range(1, 1000)]
        public double Price { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageLocalPath { get; set; }

        public void SetImage(ProductDto ProductDto, string baseUrl)
        {
            if (ProductDto.Image != null)
            {
                string fileName = ProductId + Path.GetExtension(ProductDto.Image.FileName);
                string filePath = @"wwwroot\ProductImages\" + fileName;

                var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                FileInfo file = new FileInfo(directoryLocation);
                if (file.Exists)
                {
                    file.Delete();
                }

                var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                using (var fileStream = new FileStream(filePathDirectory, FileMode.Create))
                {
                    ProductDto.Image.CopyTo(fileStream);
                }
                ImageUrl = baseUrl + "/ProductImages/" + fileName;
                ImageLocalPath = filePath;
            }
            else
            {
                ImageUrl = "https://placehold.co/600x400";
            }
        }

        public void UpdateImage(ProductDto ProductDto, string baseUrl)
        {
            if (ProductDto.Image != null)
            {
                if (!string.IsNullOrEmpty(ImageLocalPath))
                {
                    var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), ImageLocalPath);
                    FileInfo file = new FileInfo(oldFilePathDirectory);
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }

                string fileName = ProductId + Path.GetExtension(ProductDto.Image.FileName);
                string filePath = @"wwwroot\ProductImages\" + fileName;
                var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                using (var fileStream = new FileStream(filePathDirectory, FileMode.Create))
                {
                    ProductDto.Image.CopyTo(fileStream);
                }
                ImageUrl = baseUrl + "/ProductImages/" + fileName;
                ImageLocalPath = filePath;
            }
        }
    }
}
