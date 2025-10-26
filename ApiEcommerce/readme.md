# ApiEcommerce

Descripción corta:
Proyecto API para un ecommerce con endpoints para productos, categorías, usuarios y órdenes. Diseñado para servir como backend de una tienda en línea.

Estado:
- Desarrollo activo

Tecnologías:
- .NET (versión del proyecto)
- Entity Framework Core
- SQL Server (o la base de datos configurada)
- Swagger / OpenAPI

Cómo ejecutar (ejemplo rápido):
1. Restaurar paquetes:
   ```powershell
   dotnet restore
   ```
2. Aplicar migraciones (si aplica):
    Intalar ef si no se tiene
    ```powershell
   dotnet tool install --global dotnet-ef
   ```
   Crear migracion en caso de no tener
    ```powershell
   dotnet ef migrations add {Nombre Migracion}
   ```
   ```powershell
   dotnet ef database update
   ```
3. Ejecutar la API:
   ```powershell
   dotnet run --project ApiEcommerce
   ```