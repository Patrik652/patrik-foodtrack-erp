# AGENTS.md — FoodTrack ERP

## Project Context
This is a **portfolio demo project** for a C#/.NET developer position at Starbug s.r.o.
Starbug builds ERP systems for the food industry using C#, Xamarin/.NET MAUI, REST APIs, and SQL.

## Tech Stack (MANDATORY)
- **Backend:** ASP.NET Core 8 Web API
- **ORM:** Entity Framework Core 8 with SQLite
- **Mobile:** .NET MAUI (Android-first, iOS structure present)
- **Architecture:** Clean Architecture (Domain → Application → Infrastructure → API/Mobile)
- **Testing:** xUnit + FluentAssertions
- **API docs:** Swagger/Swashbuckle

## Architecture Rules
1. Domain layer has ZERO dependencies — only pure C# entities and value objects
2. Application layer depends only on Domain — contains services, interfaces, DTOs
3. Infrastructure implements interfaces from Application (repositories, DbContext)
4. API and Mobile are presentation layers — thin controllers, call Application services
5. Use dependency injection throughout

## Domain Model
```
Product
├── Id (Guid)
├── Name (string) — e.g. "Mlieko Tatranské 1L"
├── SKU (string)
├── Category (enum: Dairy, Meat, Bakery, Frozen, Beverages, Other)
├── Unit (enum: Piece, Kilogram, Liter, Box)
└── MinStockLevel (int)

Batch
├── Id (Guid)
├── ProductId (FK → Product)
├── BatchNumber (string) — e.g. "LOT-2026-04-001"
├── ManufactureDate (DateTime)
├── ExpirationDate (DateTime)
├── Quantity (decimal)
├── Location (string) — warehouse location
└── Status (enum: Active, Expired, Recalled, Depleted)

StockMovement
├── Id (Guid)
├── BatchId (FK → Batch)
├── Type (enum: Receive, Dispatch, Adjust, Return)
├── Quantity (decimal)
├── Timestamp (DateTime)
├── Note (string)
└── PerformedBy (string)
```

## Code Style
- Use C# 12 features (primary constructors, collection expressions)
- Async/await throughout
- XML documentation on public APIs
- Nullable reference types enabled
- SK locale for demo data (Slovak food product names)

## Important
- This must look like a REAL developer built it, not AI-generated boilerplate
- Add meaningful business logic: expiration warnings (7/14/30 days), FIFO stock rotation
- The mobile app should feel like a real warehouse worker's tool
- README must explain the architecture with a Mermaid diagram
