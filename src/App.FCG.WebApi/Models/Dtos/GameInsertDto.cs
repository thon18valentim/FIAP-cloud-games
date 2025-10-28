using FCG.Core.Entities;
using FluentValidation;
using FluentValidation.Results;

namespace App.FCG.WebApi.Models.Dtos
{
    public class GameInsertDto
    {
        // Validations
        public ValidationResult ValidationResult { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string PublisherName { get; set; } 
        public DateOnly ReleaseDate { get; set; }
        public decimal Price { get; set; }

        public GameInsertDto(string name, string description, string publisherName, DateOnly releaseDate, decimal price)
        {
            Name = name;
            Description = description;
            PublisherName = publisherName;
            ReleaseDate = releaseDate;
            Price = price;

            if (!IsValid())
                throw new DomainException();
        }

        public bool IsValid()
        {
            return new GameInsertDtoValidation().Validate(this).IsValid;
        }

        public class GameInsertDtoValidation : AbstractValidator<GameInsertDto>
        {
            public GameInsertDtoValidation()
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
