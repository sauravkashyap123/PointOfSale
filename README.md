# Sri Sai Sweets - Point of Sale (POS) System

A multi-project **ASP.NET Core MVC** and **Entity Framework Core** application built for a sweet shop business. The system manages shops, warehouses, inventory, billing terminal (POS), purchase entries, staff, and sales reporting.

---

## 🏛️ Project Architecture

The solution (`PointOfSale.sln`) is divided into three distinct layers:

1. **`PointOfSale` (Web / UI Layer)**:
   - ASP.NET Core MVC application.
   - Contains controllers, views (using custom CSS layouts and sidebar navigation), static assets (`wwwroot`), and dependency configurations.
   - Entry point: [Program.cs](file:///c:/Users/hp/Desktop/PointOfSale/PointOfSale/Program.cs).

2. **`POSModels` (Service & Business Logic Layer)**:
   - Houses service interfaces (in `Services/`) and implementation view models (in `ViewModels/`).
   - Defines all request, response, and print layout models.

3. **`POSDb` (Data Access Layer)**:
   - Contains the EF Core DB Context [ApplicationDbContext.cs](file:///c:/Users/hp/Desktop/PointOfSale/POSDb/Data/ApplicationDbContext.cs).
   - Houses entity definitions (`EntityModels/`) and migrations (`Migrations/`).

---

## 🚀 Key Features

* **Master Key Configuration**: Setup and administration of Shops, Warehouses, Categories, and Units.
* **Product & Production Management**: Create products (with purchase/sale rates, unit of measure, GST %, and shelf-life expiry) and track daily sweet/food production.
* **Purchase Entry Master**: Record vendor purchases of items and manage raw materials incoming to the warehouse/shops.
* **Stock & Inventory Control**: Check real-time stock levels, transfer stock from Warehouses to individual Shops, log adjustments, and track stock history.
* **Responsive POS & Billing Terminal**: Interactive cart interface for counter billing, calculating totals/GST, generating sales invoices, and printing bills.
* **Comprehensive Reports**: Analyze sales, purchases, and stock reports.
* **Authentication & Role Authorization**: Identity-based login for administrators and shop staff.

---

## 🗃️ Database Entity Relationships

The database context maps entities to tables with the `tbl` prefix:
* `EShop` (`tblShop`) ↔ `EWarehouseModel` (`tblwarehouse`): Multi-shop to warehouse association.
* `EProduct` (`tblProduct`) ↔ Inherits from `EBaseModel` (`tblProductBase`), holding codes, rates, GST%, and active statuses.
* `ECart` (`tblCart`) / `ESalesInvoice` (`tblSaleInvoice`): Used for sales transaction records.
* `EStockTransfer` (`tblStockTransfer`) & `EStockTransferDetail` (`tblStockTransferDetails`): Tracks inventory movement between locations.

---

## 🛠️ Getting Started

### Prerequisites
* **.NET 8.0 SDK** or higher.
* **SQL Server** / LocalDB or SQL Server Express.

### Setup Instructions

1. **Configure Connection String**:
   Open [appsettings.json](file:///c:/Users/hp/Desktop/PointOfSale/PointOfSale/appsettings.json) and configure your database settings under `DefaultConnection`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Data Source=YOUR_SERVER;Initial Catalog=POSDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True"
   }
   ```

2. **Run Migrations**:
   Open your terminal in the repository root folder and execute the database migration to create your local schema:
   ```powershell
   dotnet ef database update --project POSDb --startup-project PointOfSale
   ```

3. **Build & Run**:
   Run the project using the CLI:
   ```powershell
   dotnet run --project PointOfSale
   ```
   Or open `PointOfSale.sln` directly in Visual Studio and click **Start/Run**.

4. **Access the System**:
   Open your browser and navigate to the local port (e.g., `https://localhost:7198` or `http://localhost:5198`). It will redirect you to the Login screen.