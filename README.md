# 🧾 DEX Processing App (.NET MAUI + ASP.NET Core + SQL Server)

This project is a complete solution for processing DEX (Data Exchange) files using a .NET MAUI frontend and an ASP.NET Core Minimal API backend, with data stored in SQL Server via stored procedures.



https://github.com/user-attachments/assets/7db6aee6-c942-4ded-894d-dbbac6c04f44



---

## 📦 Project Structure

```
DEXProject/
├── DexApi/           → ASP.NET Core Minimal API (C#)
├── DexMauiApp/       → .NET MAUI App for sending DEX files
├── DexDatabase/      → SQL scripts (tables, stored procedures, indexes)
```

---

## ⚙️ Technologies Used

- ✅ .NET 8
- ✅ ASP.NET Core Web API (Minimal)
- ✅ .NET MAUI (Android/iOS/Windows)
- ✅ SQL Server LocalDB / Express
- ✅ Entity Framework Core
- ✅ Stored Procedures
- ✅ Swagger (with HTTP Basic Auth)
- ✅ Newtonsoft.Json

---

## 🔐 Authentication

The API requires HTTP Basic Authentication.

- **Username:** `vendsys`  
- **Password:** `NFsZGmHAGWJSZ#RuvdiV`

These credentials are stored in `appsettings.json` under `DexApiAuth`.

---

## 🧪 How It Works

1. The MAUI app has two buttons:
   - **Send Machine A**: Sends a fixed DEX string to the API
   - **Send Machine B**: Sends another fixed DEX string
2. The API:
   - Parses the DEX content
   - Saves header info (`DEXMeter`) and line details (`DEXLaneMeter`) via stored procedures
   - Ensures no duplicate `(Machine, DEXDateTime)` is inserted
3. Data is stored in a SQL Server database (`DEXDb`)

---

## 🗃️ SQL Database Schema

### Tables

- `DEXMeter`
  - `Id` (PK)
  - `Machine` (`A` or `B`)
  - `DEXDateTime` (**unique per machine**)
  - `MachineSerialNumber`
  - `ValueOfPaidVends`

- `DEXLaneMeter`
  - `Id` (PK)
  - `DEXMeterId` (FK → `DEXMeter`)
  - `ProductIdentifier`
  - `Price`
  - `NumberOfVends`
  - `ValueOfPaidSales`

### Stored Procedures

- `SaveDEXMeter`: Inserts the header and returns the inserted ID
- `SaveDEXLaneMeter`: Inserts the product-level data linked to `DEXMeterId`

### Indexes and Constraints

- `UNIQUE (Machine, DEXDateTime)` to prevent duplicates
- Foreign key from `DEXLaneMeter.DEXMeterId` → `DEXMeter.Id`

---

## ▶️ Running the App

### 1. Setup Database

- Open Visual Studio → Server Explorer
- Create a new LocalDb database: `DEXDb`
- Run all SQL scripts (tables, procedures, indexes)

### 2. Run API

- Open `DexApi`
- Press `F5` or use `dotnet run`
- Swagger UI will be available at: `https://localhost:PORT/swagger`
- Test POST `/Dex/vdi-dex` with Basic Auth

### 3. Run MAUI App

- Open `DexMauiApp`
- Select platform (Android emulator, Windows, iOS)
- Run and click "Send Machine A" or "Send Machine B"

---

## ⚠️ Error Handling

- If a duplicate `(Machine, DEXDateTime)` is detected, the API will return:
  - `409 Conflict`: "A DEX record for this machine and datetime already exists."
- SQL datetime values are validated before insertion (`>= 1753-01-01`)

---

## ✅ Requirements Fulfilled

- [x] .NET MAUI App with 2 buttons
- [x] Fixed DEX file sending
- [x] Minimal API with Basic Auth
- [x] Stored Procedures and SQL Server
- [x] Duplicate protection using `Machine + DEXDateTime`
- [x] Swagger enabled with Basic Auth support

---

## 📄 License

This project is for educational and assessment purposes only.
