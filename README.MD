# Task Management System

## 📋 Deskripsi
Task Management System adalah aplikasi RESTful API berbasis ASP.NET Core dengan arsitektur Clean Architecture yang mengelola users, projects, dan tasks dengan sistem role-based authentication.

## 🛠 Tech Stack
- **Backend**: ASP.NET Core 9.0
- **Database**: SQL Server
- **ORM**: Entity Framework Core
- **Authentication**: JWT Bearer Token
- **Documentation**: Swagger/OpenAPI
- **Logging**: Serilog
- **Architecture**: Clean Architecture (4 layers)

## 📁 Struktur Project
```
TaskManagement/
├── src/
│   ├── TaskManagement.API/          # Web API Layer
│   ├── TaskManagement.Application/  # Business Logic Layer
│   ├── TaskManagement.Domain/       # Domain Entities
│   └── TaskManagement.Infrastructure/ # Data & External Services
└── README.md
```

## 🗄 Database Schema (ERD)
```
Users (1) -----> (N) Projects
  |                     |
  |                     |
  v                     v
Tasks (N) <----------- (1) Projects

Users:
├── Id (GUID, PK)
├── Username (nvarchar, unique)
├── Email (nvarchar, unique)
├── PasswordHash (nvarchar)
├── Role (enum: Admin=0, Manager=1, User=2)
└── CreatedAt (datetime)

Projects:
├── Id (GUID, PK)
├── Name (nvarchar)
├── Description (nvarchar)
├── CreatedByUserId (FK -> Users.Id)
└── CreatedAt (datetime)

Tasks:
├── Id (GUID, PK)
├── Title (nvarchar)
├── Description (nvarchar)
├── Status (enum: Pending=0, InProgress=1, Done=2)
├── AssignedToUserId (FK -> Users.Id)
├── ProjectId (FK -> Projects.Id)
├── CreatedAt (datetime)
└── DueDate (datetime, nullable)
```

## 🔐 User Roles & Permissions

### Admin
- ✅ Full CRUD Users, Projects, Tasks
- ✅ View all data across system

### Manager  
- ✅ CRUD Projects (own projects only)
- ✅ CRUD Tasks in own projects
- ✅ Assign tasks to users

### User
- ✅ CRUD own assigned tasks
- ❌ Cannot create projects
- ❌ Cannot manage other users

## 🚀 Installation & Setup

### Prerequisites
- .NET 9.0 SDK
- SQL Server (LocalDB atau SQL Server instance)
- Visual Studio 2022 atau VS Code

### Step-by-Step Installation

1. **Clone & Setup Project**
```bash
git clone <repository-url>
cd TaskManagement
```

2. **Restore Packages**
```bash
dotnet restore
```

3. **Update Connection String**
Update `appsettings.json` di `TaskManagement.API`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TaskManagementDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

4. **Create Database**
```bash
cd TaskManagement.API
dotnet ef migrations add InitialCreate
dotnet ef database update
```

5. **Run Application**
```bash
dotnet run
```

6. **Access Application**
- API: `http://localhost:5215`
- Swagger: `http://localhost:5215/swagger`

## 👤 Default Login Credentials

### Admin Account (Auto-created)
- **Username**: `admin`
- **Password**: `admin123`
- **Role**: Admin

## 📚 API Endpoints

### Authentication
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/register` | Register new user | ❌ |
| POST | `/api/auth/login` | Login user | ❌ |

### Users (Admin Only)
| Method | Endpoint | Description | Roles |
|--------|----------|-------------|-------|
| GET | `/api/users` | Get all users | Admin |
| GET | `/api/users/{id}` | Get user by ID | Admin |
| DELETE | `/api/users/{id}` | Delete user | Admin |

### Projects
| Method | Endpoint | Description | Roles |
|--------|----------|-------------|-------|
| GET | `/api/projects` | Get projects | All |
| POST | `/api/projects` | Create project | Admin, Manager |
| PUT | `/api/projects/{id}` | Update project | Admin, Owner |
| DELETE | `/api/projects/{id}` | Delete project | Admin, Owner |

### Tasks
| Method | Endpoint | Description | Roles |
|--------|----------|-------------|-------|
| GET | `/api/tasks` | Get tasks | All |
| POST | `/api/tasks` | Create task | Admin, Manager |
| PUT | `/api/tasks/{id}` | Update task | Admin, Manager, Owner |
| DELETE | `/api/tasks/{id}` | Delete task | Admin, Owner |

## 🔧 Testing dengan Postman

### 1. Import Collection
Import file JSON collection Postman

### 2. Set Base URL
Set collection variable `baseUrl` = `http://localhost:5215`

### 3. Testing Flow
```
1. Login Admin → Auto set token
2. Create Manager user via Register
3. Login as Manager → Auto set token  
4. Create Project
5. Create Task
6. Test different role permissions
```

### 4. Environment Variables
| Variable | Description |
|----------|-------------|
| `baseUrl` | API base URL |
| `token` | JWT token (auto-set after login) |

## ⭐ Key Features

### 🔒 Security Features
- JWT Authentication with role-based authorization
- Password hashing with SHA256 + salt
- Protected endpoints based on user roles

### 🌐 External API Integration
- Email validation during registration
- Auto-generated quotes for empty task descriptions
- Fallback mechanisms for API failures

### 🔄 Background Services
- Task reminder service (checks hourly)
- Due date notifications logging

### 📝 Logging & Monitoring  
- Serilog integration with file and console outputs
- Request/response logging
- Error tracking with stack traces

### 🗃 Database Features
- Code-First Entity Framework approach
- Transactional operations
- Proper foreign key relationships
- Audit trails with created timestamps

### 📖 Documentation
- Auto-generated Swagger documentation
- Comprehensive API descriptions
- Request/response examples

## 🎯 Testing Scenarios

### Authentication Flow
```bash
# 1. Register User
POST /api/auth/register
{
  "username": "john_doe",
  "email": "john@example.com", 
  "password": "password123",
  "role": 2
}

# 2. Login
POST /api/auth/login
{
  "username": "john_doe",
  "password": "password123"  
}
# Response: { "token": "eyJ...", "username": "john_doe", "role": "User" }
```

### Project Management
```bash
# 1. Create Project (Manager/Admin)
POST /api/projects
Authorization: Bearer {token}
{
  "name": "Mobile App Project",
  "description": "iOS and Android app development"
}

# 2. Get Projects  
GET /api/projects
Authorization: Bearer {token}
```

### Task Management
```bash
# 1. Create Task
POST /api/tasks  
Authorization: Bearer {token}
{
  "title": "Design UI mockups",
  "description": "", // Will auto-fill with random quote
  "assignedToUserId": "{user-guid}",
  "projectId": "{project-guid}",
  "dueDate": "2025-12-31T23:59:59Z"
}

# 2. Update Task Status
PUT /api/tasks/{taskId}
Authorization: Bearer {token}
{
  "title": "Design UI mockups", 
  "description": "UI mockups completed",
  "status": 2, // Done
  "dueDate": "2025-12-31T23:59:59Z"
}
```

## ⚠️ Common Issues & Solutions

### 1. JWT Token Error
**Error**: `IDX10653: The encryption algorithm requires a key size of at least '128' bits`
**Solution**: Update JWT key in `appsettings.json` to minimum 32 characters

### 2. Database Connection Error  
**Error**: Cannot connect to SQL Server
**Solution**: Update connection string or ensure SQL Server is running

### 3. Migration Error
**Error**: `No migrations configuration type found`
**Solution**: Run commands from `TaskManagement.API` directory

---
**© 2025 Task Management System - Built with ASP.NET Core 9.0**