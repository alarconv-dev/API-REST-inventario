namespace backend.Models
{
    public class Movement
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Type { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Note { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
    }

    public class CreateMovementDto
    {
        public int ProductId { get; set; }
        public string Type { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}