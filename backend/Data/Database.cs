using Microsoft.Data.Sqlite;

namespace backend.Data
{
    public class Database
    {
        private readonly string _connectionString;

        public Database(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? "Data Source=inventory.db";
        }

        public SqliteConnection GetConnection()
        {
            return new SqliteConnection(_connectionString);
        }

        public void Initialize()
        {
            using var conn = GetConnection();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS products (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT NOT NULL,
                    description TEXT DEFAULT '',
                    price REAL NOT NULL,
                    stock INTEGER NOT NULL DEFAULT 0,
                    category TEXT DEFAULT '',
                    created_at TEXT DEFAULT (datetime('now'))
                );

                CREATE TABLE IF NOT EXISTS movements (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    product_id INTEGER NOT NULL,
                    type TEXT NOT NULL,
                    quantity INTEGER NOT NULL,
                    note TEXT DEFAULT '',
                    created_at TEXT DEFAULT (datetime('now')),
                    FOREIGN KEY (product_id) REFERENCES products(id)
                );
            ";
            cmd.ExecuteNonQuery();

            // Seed data
            var countCmd = conn.CreateCommand();
            countCmd.CommandText = "SELECT COUNT(*) FROM products";
            var count = (long)(countCmd.ExecuteScalar() ?? 0);

            if (count == 0)
            {
                var seedCmd = conn.CreateCommand();
                seedCmd.CommandText = @"
                    INSERT INTO products (name, description, price, stock, category) VALUES
                    ('Laptop Pro 15', 'Laptop de alto rendimiento', 1200.00, 15, 'Electrónica'),
                    ('Mouse Inalámbrico', 'Mouse ergonómico bluetooth', 35.00, 50, 'Electrónica'),
                    ('Teclado Mecánico', 'Teclado RGB switches azules', 95.00, 30, 'Electrónica'),
                    ('Monitor 4K 27', 'Monitor UHD con HDR', 380.00, 10, 'Electrónica'),
                    ('Silla Ergonómica', 'Silla de oficina con soporte lumbar', 320.00, 8, 'Muebles'),
                    ('Escritorio Ajustable', 'Escritorio de pie regulable', 450.00, 5, 'Muebles'),
                    ('Audífonos BT', 'Auriculares con cancelación de ruido', 89.00, 25, 'Electrónica'),
                    ('Webcam HD', 'Cámara 1080p para videollamadas', 65.00, 20, 'Electrónica');
                ";
                seedCmd.ExecuteNonQuery();
            }

            Console.WriteLine("Base de datos inicializada");
        }
    }
}