# Audit Logging System with RabbitMQ Integration

## Overview
This system implements a robust audit logging solution that captures database changes via Entity Framework Core interceptors and sends them to RabbitMQ for asynchronous processing. The solution is designed to minimize impact on main application performance while ensuring reliable audit log persistence.

## Key Components
1. **EF Core Interceptor**: Captures entity changes during `SaveChangesAsync`
2. **RabbitMQ Integration**: Sends audit logs to message queue
3. **Background Worker**: Processes messages and saves to database
4. **SQL Server**: Stores audit logs persistently

## System Architecture
```
[API Endpoint]
    │
    │ (1) Entity Operation (Create/Update/Delete)
    ▼
[EF Core Interceptor]
    │
    │ (2) Send audit logs to RabbitMQ
    ▼
[RabbitMQ Queue]
    │
    │ (3) Deliver messages
    ▼
[Background Worker]
    │
    │ (4) Save to database
    ▼
[SQL Server Database]
```

## Prerequisites
- .NET 8 SDK
- SQL Server 2019+
- RabbitMQ 3.11+
- Windows OS (or adjust for Linux/Mac)

## Setup Instructions

### 1. Clone Repository
```bash
git clone https://github.com/your-repo/audit-logging-system.git
cd audit-logging-system
```

### 2. Configure Database
Update connection string in `appsettings.json`:
```json
"ConnectionStrings": {
  "cs": "Server=localhost\\MSSQLSERVER01;Database=TransactionsLogs;User Id=your-user;Password=your-password;TrustServerCertificate=True;"
}
```

### 3. Configure RabbitMQ
Update RabbitMQ settings in `appsettings.json`:
```json
"RabbitMQ": {
  "Host": "localhost",
  "Username": "guest",
  "Password": "guest"
}
```

### 4. Apply Database Migrations
```bash
dotnet ef database update --project TransactionLogs
```

### 5. Start RabbitMQ (Windows)
```powershell
rabbitmq-service.bat start
rabbitmq-plugins enable rabbitmq_management
```

## Running the Application

### Start the API
```bash
dotnet run --project TransactionLogs
```

### Start the Background Worker
```bash
dotnet run --project TransactionLogs
```

## Testing the System

### 1. Create a Product
```bash
POST /api/product
Content-Type: application/json

{
  "name": "Test Product",
  "price": 29.99
}
```

### 2. Verify Logs
Check application logs for:
```
Sent 1 audit logs to RabbitMQ
Received 1 audit logs
Saved 1 audit logs to database
```

### 3. View Audit Logs
```bash
GET /api/transaction
```

## Monitoring

### RabbitMQ Dashboard
Access the RabbitMQ management interface:
```
http://localhost:15672
Credentials: guest/guest
```

### Database Verification
Query audit logs table:
```sql
SELECT * FROM Transactions ORDER BY TimeStamp DESC
```

## Key Features

- **Asynchronous Processing**: Audit logs are processed in background
- **Entity Change Tracking**: Automatic capture of create/update/delete operations
- **RabbitMQ Integration**: Reliable message queuing
- **SQL Server Storage**: Persistent audit log storage
- **Error Resilience**: Automatic retries for failed operations

## Troubleshooting

### Common Issues
1. **RabbitMQ Connection Failed**:
   - Verify RabbitMQ service is running
   - Check firewall settings for port 5672

2. **Database Connection Issues**:
   - Validate connection string in appsettings.json
   - Ensure SQL Server is accessible

3. **No Audit Logs Created**:
   - Check interceptor registration in Program.cs
   - Verify entities are not excluded in interceptor

### Log Locations
- Application logs: Console output
- RabbitMQ logs: `C:\Program Files\RabbitMQ Server\log\rabbit@localhost.log`
- SQL Server logs: SQL Server Management Studio

## Deployment

### Windows Service
```powershell
sc create "AuditLogService" binPath="C:\path\to\TransactionLogs.exe"
sc start AuditLogService
```

### Docker Container
```Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "TransactionLogs.csproj"
RUN dotnet build "TransactionLogs.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TransactionLogs.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TransactionLogs.dll"]
```