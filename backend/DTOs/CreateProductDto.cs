using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public class CreateProductDto
{
    public string Name { get; set; }

    public string Description { get; set; }

    public double Price { get; set; }

    [Range(0, 10000, ErrorMessage = "La cantidad en stock debe de ser un número positivo")]
    public int Stock { get; set; }

    [Required(ErrorMessage = "La categoría es obligatoria")]
    public string Category { get; set; }

    public string? CreatedAt { get; set; }
}
