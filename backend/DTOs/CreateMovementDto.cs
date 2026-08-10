using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public class CreateMovementDto
{
    public int ProductId { get; set; }
    public string Type { get; set; }
    [Range(0.01, 10000, ErrorMessage = "El precio debe de ser Mayor a 0")]
    public int Quantity { get; set; }
    public string Note { get; set; }
    public string CreatedAt { get; set; }
    public string ProductName { get; set; }
}
