using Microsoft.AspNetCore.Mvc;
using backend.Data;
using backend.Models;
using backend.DTOs;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovementsController : ControllerBase
    {
        private readonly Database _db;

        public MovementsController(Database db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            using var conn = _db.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT m.id, m.product_id, m.type, m.quantity, m.note, m.created_at, p.name
                FROM movements m
                JOIN products p ON p.id = m.product_id
                ORDER BY m.created_at DESC
                LIMIT 50
            ";

            var movements = new List<Movement>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                movements.Add(new Movement
                {
                    Id = reader.GetInt32(0),
                    ProductId = reader.GetInt32(1),
                    Type = reader.GetString(2),
                    Quantity = reader.GetInt32(3),
                    Note = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    CreatedAt = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    ProductName = reader.GetString(6)
                });
            }
            return Ok(movements);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateMovementDto dto)
        {
            if (dto.Quantity <= 0)
                return BadRequest(new { message = "La cantidad debe ser mayor a 0" });

            if (dto.Type != "entrada" && dto.Type != "salida")
                return BadRequest(new { message = "El tipo debe ser 'entrada' o 'salida'" });

            using var conn = _db.GetConnection();
            conn.Open();

            var checkProduct = conn.CreateCommand();
            checkProduct.CommandText = "SELECT stock FROM products WHERE id = $id";
            checkProduct.Parameters.AddWithValue("$id", dto.ProductId);
            var stockObj = checkProduct.ExecuteScalar();
            if (stockObj == null)
                return NotFound(new { message = "Producto no encontrado" });

            var currentStock = (long)stockObj;
            if (dto.Type == "salida" && currentStock < dto.Quantity)
                return BadRequest(new { message = "Stock insuficiente" });

            var insertCmd = conn.CreateCommand();
            insertCmd.CommandText = @"
                INSERT INTO movements (product_id, type, quantity, note)
                VALUES ($pid, $type, $qty, $note);
                SELECT last_insert_rowid();
            ";
            insertCmd.Parameters.AddWithValue("$pid", dto.ProductId);
            insertCmd.Parameters.AddWithValue("$type", dto.Type);
            insertCmd.Parameters.AddWithValue("$qty", dto.Quantity);
            insertCmd.Parameters.AddWithValue("$note", dto.Note);
            var id = (long)(insertCmd.ExecuteScalar() ?? 0);

            var updateStock = conn.CreateCommand();
            updateStock.CommandText = dto.Type == "entrada"
                ? "UPDATE products SET stock = stock + $qty WHERE id = $id"
                : "UPDATE products SET stock = stock - $qty WHERE id = $id";
            updateStock.Parameters.AddWithValue("$qty", dto.Quantity);
            updateStock.Parameters.AddWithValue("$id", dto.ProductId);
            updateStock.ExecuteNonQuery();

            return Created($"/api/movements/{id}", new { id, message = "Movimiento registrado" });
        }
    }
}