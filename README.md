# FoodTrack ERP

FoodTrack ERP je portfolio demo pre C#/.NET pozíciu v Starbug s.r.o. Repo teraz obsahuje dokončený ASP.NET Core 8 backend, reálnu .NET MAUI mobilnú appku pod `src/FoodTrack.Mobile`, JWT autentifikáciu pre skladové write flow, Docker support a rozšírené automatizované testy.

## Scope

- ASP.NET Core 8 Web API nad EF Core 8 a SQLite
- Clean Architecture vrstvy `Domain -> Application -> Infrastructure -> API/Mobile`
- FIFO vyskladnenie a expirácie `30 / 14 / 7` dní
- Operator login cez badge code + PIN s bearer tokenmi
- CRUD/read-management surface pre produkty, batch detail a stock movement históriu
- Android-first .NET MAUI app pod `src/FoodTrack.Mobile/` s `Shell`, login, inventory listom, batch detailom, receive/dispatch/adjust flow a dashboardom
- Dockerfile pre API a root `docker-compose.yml`
- xUnit + FluentAssertions coverage cez domain, application a API integration

## Architecture

```mermaid
flowchart TB
    Mobile["FoodTrack.Mobile\nShell + XAML Views + ViewModels"] -->|HttpClient + JWT| API["FoodTrack.API\nControllers + Swagger + Auth"]
    API --> Application["FoodTrack.Application\nQueries + Commands + Management Services"]
    Application --> Domain["FoodTrack.Domain\nEntities + Enums + FIFO Policies"]
    Infrastructure["FoodTrack.Infrastructure\nEF Core + SQLite + Seed Data"] --> Application
    Infrastructure --> Domain
    API --> Infrastructure
    Mobile --> Application
    Tests["Tests\nDomain + Application + API"] --> Domain
    Tests --> Application
    Tests --> API
```

## Solution Layout

- `src/FoodTrack.Domain` obsahuje čisté entity, enumy a business pravidlá bez externých závislostí.
- `src/FoodTrack.Application` drží DTO, query/command služby, auth flow a management surface.
- `src/FoodTrack.Infrastructure` implementuje EF Core repozitáre, `DbContext` a seeded demo dáta.
- `src/FoodTrack.API` vystavuje Swagger API, JWT autentifikáciu a warehouse endpoints.
- `src/FoodTrack.Mobile` obsahuje reálny .NET MAUI app projekt s `UseMaui`, `Platforms`, `Resources`, XAML views, viewmodelmi a `HttpClient` service layerom.
- `tests/FoodTrack.Domain.Tests` pokrýva entity a FIFO pravidlá.
- `tests/FoodTrack.Application.Tests` pokrýva query/command/auth služby.
- `tests/FoodTrack.API.Tests` spúšťa reálny HTTP pipeline cez `WebApplicationFactory`.

## Demo Credentials

Seeded operátori:

- `OP-1001 / 1234` -> `Roman Skladnik`
- `OP-1002 / 2345` -> `Jana Expedicia`
- `OP-1003 / 3456` -> `Marek Pekar`

Seeded produkty:

- `Mlieko Tatranske 1L`
- `Kuracie Prsia Chladene 1kg`
- `Rozok Bily 60g`
- `Mineralna Voda Jemne Sycena 1.5L`

## API Surface

Anonymous read endpoints:

- `POST /api/auth/login`
- `GET /api/dashboard/expiration-overview`
- `GET /api/products`
- `GET /api/products/{productId}`
- `GET /api/products/{productId}/fifo-batches`
- `GET /api/batches/{batchId}`
- `GET /api/stock-movements`
- `GET /api/stock-movements/{movementId}`

Authenticated warehouse/admin endpoints:

- `POST /api/products`
- `PUT /api/products/{productId}`
- `DELETE /api/products/{productId}`
- `POST /api/inventory/receive`
- `POST /api/inventory/dispatch`
- `POST /api/inventory/adjustments`
- `PUT /api/batches/{batchId}`
- `DELETE /api/batches/{batchId}`

Swagger používa bearer security scheme. Najprv zavolaj `POST /api/auth/login`, potom vlož vrátený token do `Authorize`.

## Local Setup

```bash
export PATH="$HOME/.dotnet:$PATH"
dotnet build FoodTrack.sln
dotnet test FoodTrack.sln --filter "FullyQualifiedName!~FoodTrack.Mobile"
dotnet run --project src/FoodTrack.API
```

Default Swagger URL:

- `http://localhost:5262/swagger`

Mobile shell je pod `src/FoodTrack.Mobile/`. V tomto repo je udržiavaný v MAUI štruktúre a zároveň zostáva buildovateľný na tomto Linux hoste bez nainštalovaných MAUI workloadov. `MauiProgram.cs` je pripravený na pripojenie proti API cez `HttpClient` a JWT session store.
Mobilná appka je pod `src/FoodTrack.Mobile/` ako skutočný .NET MAUI projekt so `Shell` navigáciou, `Platforms/*`, `Resources/*`, XAML stránkami a JWT `HttpClient` vrstvou. Default solution verification na tomto Linux hoste stavia backend a test projekty; samotný MAUI head sa v solution konfigurácii nebuildí automaticky, aby verify nevyžadoval plný Android SDK/JDK toolchain.

## Docker

Build a run API v containery:

```bash
docker compose up --build
```

API bude dostupné na:

- `http://localhost:8080/swagger`

SQLite databáza sa ukladá do named volume `foodtrack-data`.

## Mobile Notes

`src/FoodTrack.Mobile/` obsahuje:

- `FoodTrack.Mobile.csproj` s `UseMaui=true` a Android-first targetom
- `AppShell.xaml` s TabBar navigáciou `Scan`, `Inventory`, `Dashboard`, `Settings`
- `Platforms/Android`, `Platforms/iOS`, `Platforms/MacCatalyst`, `Platforms/Windows`
- `Views/LoginPage.xaml` pre operator login
- `Views/ProductListPage.xaml` so search/filter a refresh UI
- `Views/BatchDetailPage.xaml` s movement históriou
- `Views/ReceiveStockPage.xaml`, `Views/DispatchStockPage.xaml`, `Views/AdjustStockPage.xaml`
- `Views/DashboardPage.xaml` s expiracnym summary
- `Services/ApiService.cs` s bearer token auth na backend
- viewmodely s loading stavmi a offline-friendly error hláškami

Automatizované testy v solution:

- `FoodTrack.Domain.Tests`: `6`
- `FoodTrack.Application.Tests`: `9`
- `FoodTrack.API.Tests`: `11`
- spolu: `26` zelených testov

## Verification

Hlavné verifikačné príkazy:

```bash
export PATH="$HOME/.dotnet:$PATH" && dotnet build FoodTrack.sln
export PATH="$HOME/.dotnet:$PATH" && dotnet test FoodTrack.sln --filter "FullyQualifiedName!~FoodTrack.Mobile"
test $(export PATH="$HOME/.dotnet:$PATH" && dotnet test FoodTrack.sln --filter "FullyQualifiedName!~FoodTrack.Mobile" --list-tests 2>/dev/null | grep -c 'test host') -ge 1
```
