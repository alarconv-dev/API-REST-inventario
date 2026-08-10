# API REST Inventario

API REST para gestión de inventario construida con .NET 10 y SQLite.

## Estructura del proyecto

- `backend/`
  - `Program.cs` - configuración de servicios y arranque de la API.
  - `Controllers/ProductsController.cs` - endpoints CRUD para productos.
  - `Controllers/MovementsController.cs` - endpoints para movimientos de stock.
  - `Data/Database.cs` - inicialización de SQLite y seeding de datos.
  - `DTOs/` - clases para transferencia de datos.
  - `Models/` - modelos de datos para productos y movimientos.

## Funcionalidades

- Lista productos: `GET /api/products`
- Obtiene producto por id: `GET /api/products/{id}`
- Crea producto: `POST /api/products`
- Actualiza producto: `PUT /api/products/{id}`
- Elimina producto: `DELETE /api/products/{id}`
- Lista movimientos: `GET /api/movements`
- Registra movimiento de stock: `POST /api/movements`

## Base de datos

- Usa SQLite local con archivo `inventory.db` por defecto.
- Crea automáticamente las tablas `products` y `movements` si no existen.
- Inserta datos iniciales de productos la primera vez que arranca.

## Ejecución

1. Abre la carpeta `backend` en tu terminal.
2. Ejecuta `dotnet restore`.
3. Ejecuta `dotnet run`.
4. La API queda disponible en el puerto configurado por .NET (por defecto `http://localhost:5000`).

## Ejemplo de uso

### Crear un producto

POST `http://localhost:5000/api/products`

Body JSON:

```json
{
  "name": "Producto de prueba",
  "description": "Descripción del producto",
  "price": 100.0,
  "stock": 10,
  "category": "Categoría"
}
```

### Registrar movimiento

POST `http://localhost:5000/api/movements`

Body JSON:

```json
{
  "productId": 1,
  "type": "entrada",
  "quantity": 5,
  "note": "Ingreso de stock"
}
```

## Notas

- CORS está habilitado para permitir el acceso desde un frontend externo.
- El backend está listo para integrarse con cualquier cliente que consuma la API.
