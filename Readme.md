# Transaction Logs API

A RESTful API built with ASP.NET Core to track database transactions and changes automatically using Entity Framework Core interceptors.

## Features

- **Audit Logging**: Automatically logs all CRUD operations (Create, Read, Update, Delete) on entities.
- **Entity Tracking**: Tracks changes to `Product` and `User` entities.
- **Transaction Metadata**: Stores operation type, table name, primary key, entity data, and timestamps.
- **SQL Server Integration**: Uses SQL Server for data storage.
- **RESTful API**: Exposes endpoints for managing Products, Users, and viewing Transaction logs.

## Technologies Used

- **Backend**: ASP.NET Core 6+
- **Database**: SQL Server + Entity Framework Core
- **Audit Logging**: EF Core Interceptors
- **API Documentation**: Swagger/OpenAPI
- **Dependency Injection**: Built-in .NET Core DI

## Getting Started

### Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Installation

1. **Clone the Repository**
   ```bash
   git clone https://github.com/yourusername/transaction-logs-api.git
   cd transaction-logs-api
   ```

2. **Database Setup**
   - Update connection string in `appsettings.json`:
     ```json
     "ConnectionStrings": {
       "cs": "Server=your_server;Database=TransactionLogsDB;Integrated Security=True;TrustServerCertificate=True;"
     }
     ```
   - Run migrations:
     ```bash
     dotnet ef database update
     ```

3. **Run the Application**
   ```bash
   dotnet run
   ```
   The API will be available at `https://localhost:7111`.

4. **Access Swagger UI**
   Visit `https://localhost:7111/swagger` to explore the API endpoints.

## API Documentation

### Endpoints

#### Products
| Method | Endpoint         | Description                  |
|--------|------------------|------------------------------|
| GET    | /api/Product     | Get all products             |
| GET    | /api/Product/{id}| Get product by ID            |
| POST   | /api/Product     | Create new product           |
| PUT    | /api/Product/{id}| Update existing product      |
| DELETE | /api/Product/{id}| Delete product               |

#### Users
| Method | Endpoint       | Description                |
|--------|----------------|----------------------------|
| GET    | /api/User      | Get all users              |
| GET    | /api/User/{id} | Get user by ID             |
| POST   | /api/User      | Create new user            |
| PUT    | /api/User/{id} | Update existing user       |
| DELETE | /api/User/{id} | Delete user                |

#### Transactions
| Method | Endpoint          | Description                |
|--------|-------------------|----------------------------|
| GET    | /api/Transaction  | Get all audit logs         |

### Audit Logging Mechanism
- **Automatic Tracking**: All database operations are intercepted and logged to the `Transactions` table.
- **Data Captured**:
  - Operation type (Add/Update/Delete)
  - Affected table name
  - Primary key value
  - Full entity snapshot (JSON)
  - Timestamp

## Project Structure

```
TransactionLogs/
├── Domain/
│   ├── Entities/          # Database entities (Product, User, Transaction)
│   └── Interfaces/        # Repository interfaces
├── Infrastructure/
│   ├── Data/              # DbContext and migrations
│   ├── Interceptors/      # Audit logging implementation
│   └── Repositories/      # Repository implementations
├── web/                   # API Controllers
└── appsettings.json       # Configuration
```

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/your-feature`)
3. Commit your changes (`git commit -m 'Add some feature'`)
4. Push to the branch (`git push origin feature/your-feature`)
5. Open a Pull Request

