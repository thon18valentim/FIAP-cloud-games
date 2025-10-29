using System.ComponentModel.DataAnnotations;

namespace App.FCG.WebApi.Models.Dtos
{
    public class GameInsertDto
    {
        [Required(ErrorMessage = "Name field cannot be empty")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Field cannot be empty")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Max Length is 500")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Publisher name cannot be empty")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Maximum length is 100 and minimum is 5")]
        public string PublisherName { get; set; }

        [Required(ErrorMessage = "Field cannot be empty")]
        public DateOnly ReleaseDate { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greaten than 0")]
        public decimal Price { get; set; }

        public GameInsertDto() { }

        public GameInsertDto(string name, string description, string publisherName, DateOnly releaseDate, decimal price)
        {
            Name = name;
            Description = description;
            PublisherName = publisherName;
            ReleaseDate = releaseDate;
            Price = price;
        }
    }
}