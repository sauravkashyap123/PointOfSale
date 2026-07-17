# Project Guidelines & AI Developer Guide

Welcome to the **Sri Sai Sweets - Point of Sale (POS) System** workspace. Below are instructions and guidelines to follow when adding new features, modifying code, or performing refactoring in this repository.

---

## 🏛️ Architectural Design

The project is structured into three main layers (projects) within the Visual Studio Solution:

1. **`PointOfSale` (Presentation Layer)**:
   - An ASP.NET Core MVC application.
   - Houses the Controllers, Views, routing, application configuration (`appsettings.json`, `Program.cs`), and static assets (`wwwroot`).
   - UI views use a customized Sidebar template (`_SideLayout.cshtml`) and custom CSS variables.
   
2. **`POSModels` (Service & Data Transfer Layer)**:
   - Houses DTOs (Data Transfer Objects), request models, response models, and VM results.
   - Defines standard interfaces for services under `/Services`.
   - Implements those services inside the `/ViewModels` folder (e.g., `HomeViewModel.cs`, `BillingService.cs`).
   
3. **`POSDb` (Data Access Layer)**:
   - Houses the Entity Framework Core database context (`ApplicationDbContext.cs`).
   - Contains all Entity models under `/EntityModels` (categorized by subfolders like `MithaiShop` or `PurchaseEntry`).
   - Manages database migration history under the `/Migrations` folder.

---

## 🛠️ Design Patterns & Best Practices

To maintain code cleanliness and readability, strictly follow these practices:

### 1. Separation of Concerns (SoC)
* **Do NOT write database queries or core business logic directly in the Controllers.**
* Always define an interface in `POSModels/Services/` (e.g., `INewFeatureService.cs`).
* Implement the service in `POSModels/ViewModels/` (e.g., `NewFeatureViewModel.cs`).
* Inject the service via Constructor Dependency Injection into the controller.
* Register the service in `PointOfSale/Program.cs` under the builder services section:
  ```csharp
  builder.Services.AddScoped<INewFeatureService, NewFeatureViewModel>();
  ```

### 2. Naming Conventions
* **Entities**: Database entity classes must be prefixed with `E` (e.g., `EProduct`, `EShop`, `EStockTransfer`) and located in the `POSDb.EntityModels` namespace.
* **DbSets**: Registered DbSet tables in `ApplicationDbContext` should prefix with `tbl` (e.g., `tblProduct`, `tblShop`, `tblStock`).
* **ViewModels/Services**: Services under `POSModels/ViewModels/` can end with `ViewModel` or `Service` depending on context, but their interfaces must always start with `I` (e.g., `IHomeService`).
* **Controllers**: Controllers must reside in the `Controllers` folder of `PointOfSale` and end with `Controller.cs` (e.g., `BillingController.cs`).

### 3. Database Migrations
Since migrations are hosted inside the `POSDb` class library, always specify the correct project and startup project when executing migration commands:
* **Add a Migration**:
  ```powershell
  dotnet ef migrations add <MigrationName> --project POSDb --startup-project PointOfSale
  ```
* **Apply Migrations**:
  ```powershell
  dotnet ef database update --project POSDb --startup-project PointOfSale
  ```

### 4. UI/Styling System
* UI styling relies on custom vanilla CSS.
* The main sidebar uses Tabler Icons. Check `_SideLayout.cshtml` to see existing icons and style colors.
* Always define responsive UI structures using modern grid or flex layouts. Ensure compatibility with mobile screen viewports (breakpoint at `640px` and below).
