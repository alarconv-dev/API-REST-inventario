using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using backend.Data;
using backend.Models;
using backend.DTOs;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly Database _db;

        public ProductsController(Database db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            using var conn = _db.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM products ORDER BY created_at DESC";

            var products = new List<Product>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                products.Add(new Product
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    Price = reader.GetDouble(3),
                    Stock = reader.GetInt32(4),
                    Category = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    CreatedAt = reader.IsDBNull(6) ? "" : reader.GetString(6)
                });
            }
            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM products WHERE id = $id";
            cmd.Parameters.AddWithValue("$id", id);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return NotFound(new { message = "Producto no encontrado" });

            return Ok(new Product
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                Price = reader.GetDouble(3),
                Stock = reader.GetInt32(4),
                Category = reader.IsDBNull(5) ? "" : reader.GetString(5),
                CreatedAt = reader.IsDBNull(6) ? "" : reader.GetString(6)
            });
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateProductDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new { message = "El nombre es obligatorio" });

            using var conn = _db.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO products (name, description, price, stock, category)
                VALUES ($name, $desc, $price, $stock, $cat);
                SELECT last_insert_rowid();
            ";
            cmd.Parameters.AddWithValue("$name", dto.Name);
            cmd.Parameters.AddWithValue("$desc", dto.Description);
            cmd.Parameters.AddWithValue("$price", dto.Price);
            cmd.Parameters.AddWithValue("$stock", dto.Stock);
            cmd.Parameters.AddWithValue("$cat", dto.Category);

            var id = (long)(cmd.ExecuteScalar() ?? 0);

            var getCmd = conn.CreateCommand();
            getCmd.CommandText = "SELECT * FROM products WHERE id = $id";
            getCmd.Parameters.AddWithValue("$id", id);

            using var reader = getCmd.ExecuteReader();
            reader.Read();
            return Created($"/api/products/{id}", new Product
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                Price = reader.GetDouble(3),
                Stock = reader.GetInt32(4),
                Category = reader.IsDBNull(5) ? "" : reader.GetString(5),
                CreatedAt = reader.IsDBNull(6) ? "" : reader.GetString(6)
            });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] CreateProductDto dto)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            var check = conn.CreateCommand();
            check.CommandText = "SELECT id FROM products WHERE id = $id";
            check.Parameters.AddWithValue("$id", id);
            if (check.ExecuteScalar() == null)
                return NotFound(new { message = "Producto no encontrado" });

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE products 
                SET name=$name, description=$desc, price=$price, stock=$stock, category=$cat
                WHERE id=$id
            ";
            cmd.Parameters.AddWithValue("$name", dto.Name);
            cmd.Parameters.AddWithValue("$desc", dto.Description);
            cmd.Parameters.AddWithValue("$price", dto.Price);
            cmd.Parameters.AddWithValue("$stock", dto.Stock);
            cmd.Parameters.AddWithValue("$cat", dto.Category);
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();

            var getCmd = conn.CreateCommand();
            getCmd.CommandText = "SELECT * FROM products WHERE id = $id";
            getCmd.Parameters.AddWithValue("$id", id);
            using var reader = getCmd.ExecuteReader();
            reader.Read();
            return Ok(new Product
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                Price = reader.GetDouble(3),
                Stock = reader.GetInt32(4),
                Category = reader.IsDBNull(5) ? "" : reader.GetString(5),
                CreatedAt = reader.IsDBNull(6) ? "" : reader.GetString(6)
            });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            var check = conn.CreateCommand();
            check.CommandText = "SELECT id FROM products WHERE id = $id";
            check.Parameters.AddWithValue("$id", id);
            if (check.ExecuteScalar() == null)
                return NotFound(new { message = "Producto no encontrado" });

            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM products WHERE id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();

            return Ok(new { message = "Producto eliminado" });
        }
    }
}