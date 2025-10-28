using System.ComponentModel.DataAnnotations;

namespace App.FCG.WebApi.Models.Dtos;

public class GameInsertDto
{
    [Required]
    public string Name { get; set; }
    [Required]
    [MaxLength (500, ErrorMessage = "Campo pode ter no máximop 500 caracteres.")]
    public string Description { get; set; }
    [Required]
    [MaxLength (20, ErrorMessage = "Campo pode ter no máximo 20 caracteres.")]
    public string PublisherName { get; set; }
    [Required]
    public DateTime ReleaseDate { get; set; }
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero.")]
    public decimal Price { get; set; }
}
