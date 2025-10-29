using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace App.FCG.WebApi.Models.Dtos
{
    public class GameUpdateDto
    {
        [Key]
        public Guid Id { get; set; }
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

        public GameUpdateDto() { }

        public GameUpdateDto(string name, string description, string publisherName, DateOnly releaseDate, decimal price)
        {
            Name = name;
            Description = description;
            PublisherName = publisherName;
            ReleaseDate = releaseDate;
            Price = price;  
        }

        public bool IsValid()
        {
            return new GameUpdateDtoValidation().Validate(this).IsValid;
        }

        public class GameUpdateDtoValidation : AbstractValidator<GameUpdateDto>
        {
            public GameUpdateDtoValidation()
            {
                RuleFor(c => c.Description)
                    .MaximumLength(500)
                    .NotEmpty()
                    .NotNull()
                    .WithMessage("Field cannot be empty")
                    .MinimumLength(10)
                    .WithMessage("Max Length is 500");

                RuleFor(c => c.PublisherName)
                    .MaximumLength(100)
                    .MinimumLength(5)
                    .WithMessage("Maximum length is 100 and minimum is 5");

                RuleFor(c => c.ReleaseDate)
                    .NotEmpty()
                    .WithMessage("Field cannot be empty");

                RuleFor(c => c.Price)
                    .GreaterThan(0)
                    .WithMessage("Price must be greaten than 0");
            }
        }
    }
}
