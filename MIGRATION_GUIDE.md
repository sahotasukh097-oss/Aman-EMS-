# AMAN HR Management System - C# Migration

## Overview

This document describes the conversion of the Python Flask-based AMAN HR Management System to a C# ASP.NET Core Razor Pages application.

## What Was Converted

### Python Flask Application → C# ASP.NET Core

The following components from the Python application have been converted to C#:

#### 1. **Database Models**
- **User** - User authentication and profile management
- **Employee** - Employee information and records
- **Attendance** - Attendance tracking
- **Leave** - Leave request management
- **Payroll** - Payroll calculations and records

**Files:**
- `Models/User.cs`
- `Models/Employee.cs`
- `Models/Attendance.cs`
- `Models/Leave.cs`
- `Models/Payroll.cs`

#### 2. **Database Context**
- Converted from Flask-SQLAlchemy to Entity Framework Core (EF Core)
- Configured for SQLite database (same as Python app)
- Location: `Data/ApplicationDbContext.cs`

#### 3. **Authentication & Authorization**
- Implemented Cookie-based authentication using ASP.NET Core Identity
- Password hashing using BCrypt.Net-Next
- Login/Signup functionality with secure session management
- Location: `Controllers/AuthController.cs`

#### 4. **Sample Data Initialization**
- Converted `add_sample_data.py` to `Data/SampleDataInitializer.cs`
- Automatically seeds database with 10 sample employees on first run
- Default admin credentials:
  - Email: `admin@aman.com`
  - Password: `123456`

#### 5. **Views/Pages**
- **Login Page** - User authentication (`Views/Auth/Login.cshtml`)
- **Signup Page** - New user registration (`Views/Auth/Signup.cshtml`)
- **Dashboard** - Main dashboard with statistics (`Views/Auth/Dashboard.cshtml`)

## Project Structure

```
Aman(EMS)/
├── Controllers/
│   ├── HomeController.cs       (Existing - public pages)
│   └── AuthController.cs       (NEW - authentication)
├── Models/
│   ├── ErrorViewModel.cs       (Existing)
│   ├── User.cs                 (NEW)
│   ├── Employee.cs             (NEW)
│   ├── Attendance.cs           (NEW)
│   ├── Leave.cs                (NEW)
│   └── Payroll.cs              (NEW)
├── Data/
│   ├── ApplicationDbContext.cs (NEW - EF Core context)
│   └── SampleDataInitializer.cs (NEW - seed data)
├── Views/
│   ├── Auth/
│   │   ├── Login.cshtml        (NEW)
│   │   ├── Signup.cshtml       (NEW)
│   │   └── Dashboard.cshtml    (NEW)
│   ├── Home/                   (Existing - public pages)
│   └── Shared/
│       └── _Layout.cshtml      (Updated - added auth links)
├── Program.cs                  (Updated - added EF Core, Auth)
├── appsettings.json            (Updated - connection string)
└── Aman(EMS).csproj            (Updated - added NuGet packages)
```

## Key Features Implemented

### 1. Authentication System
- User login with email/username
- User registration/signup
- Cookie-based session management
- Password hashing with BCrypt
- "Remember Me" functionality
- Automatic logout on inactivity

### 2. Database Features
- SQLite database (portable, no server needed)
- Automatic database creation on first run
- Automatic table creation via Entity Framework Core migrations
- Sample data seeding with 10 employees

### 3. User Interface
- Modern gradient-based design
- Responsive login/signup forms
- Beautiful dashboard with statistics cards
- Quick action buttons for main features
- Navigation bar with authentication status

### 4. Security
- Password hashing using BCrypt
- Secure cookie authentication
- CSRF protection via ASP.NET Core
- Form validation on client and server
- SQL injection protection via EF Core

## Getting Started

### Prerequisites
- .NET 10 SDK or higher
- Visual Studio 2026 or Visual Studio Code with C# extension

### Installation

1. **Build the Project**
   ```bash
   dotnet build
   ```

2. **Run the Application**
   ```bash
   dotnet run
   ```

3. **Access the Application**
   - Open browser to `https://localhost:7000` (or the URL shown in console)
   - The public pages (Home, About, Features, FAQ, Contact) are available without login
   - Click "Demo" or "Login" in the navigation bar to access the authentication system

### Demo Credentials

**Admin Account:**
- Email: `admin@aman.com`
- Password: `123456`

**Sample Employees:**
The system automatically creates 10 sample employees during first run with names like:
- John Smith (Engineering)
- Sarah Johnson (Marketing)
- Michael Brown (Sales)
- And 7 more...

All sample employees have password: `password123`

## Database

### Location
The SQLite database file `aman_hrm.db` is created automatically in the project root directory on first run.

### Connection String
Configured in `appsettings.json`:
```json
"ConnectionStrings": {
    "DefaultConnection": "Data Source=aman_hrm.db"
}
```

### Tables Created
- `Amanhr_users` - User accounts
- `Amanhr_employees` - Employee records
- `Amanhr_attendance` - Attendance logs
- `Amanhr_leaves` - Leave requests
- `Amanhr_payroll` - Payroll records

## API Endpoints

### Authentication Routes
- `GET /Auth/Login` - Display login form
- `POST /Auth/Login` - Process login
- `GET /Auth/Signup` - Display signup form
- `POST /Auth/Signup` - Process signup
- `POST /Auth/Logout` - Logout user
- `GET /Auth/Dashboard` - View dashboard (requires authentication)

### Public Routes (Existing)
- `GET /` - Home page
- `GET /Home/About` - About page
- `GET /Home/Features` - Features page
- `GET /Home/Faq` - FAQ page
- `GET /Home/Contact` - Contact page

## Configuration

### appsettings.json
Key configurations:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=aman_hrm.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Authentication Settings (Program.cs)
- Default login redirect: `/Auth/Login`
- Default logout redirect: `/Auth/Logout`
- Cookie expiration: 7 days
- Sliding expiration: Enabled

## NuGet Packages Added

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="10.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="10.0.0" />
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
```

## Migration from Python to C#

### What Was Changed

1. **Database Layer**
   - Flask-SQLAlchemy → Entity Framework Core
   - SQLite connection remains the same
   - Table structure preserved exactly

2. **Authentication**
   - Flask-Login → ASP.NET Core Identity cookies
   - Werkzeug hashing → BCrypt hashing
   - Password verification still supports old hashes

3. **Routing**
   - Flask routes → ASP.NET Core controller actions
   - URL structure simplified

4. **Templates**
   - Jinja2 templates → Razor views
   - Bootstrap styling maintained
   - JavaScript functionality enhanced

### Backward Compatibility

- Old password hashes from Python app are still supported
- Database tables maintain same names (`Amanhr_*`)
- Data structure compatible with original Python schema

## Next Steps

To continue development:

1. **Implement Employee Management**
   - Create `Controllers/EmployeeController.cs`
   - Create views for list, add, edit, delete employees

2. **Implement Attendance Management**
   - Create `Controllers/AttendanceController.cs`
   - Track daily attendance

3. **Implement Leave Management**
   - Create `Controllers/LeaveController.cs`
   - Handle leave requests and approvals

4. **Implement Payroll Management**
   - Create `Controllers/PayrollController.cs`
   - Generate payroll reports

5. **Add Reporting Features**
   - Employee reports
   - Attendance reports
   - Payroll reports

## Troubleshooting

### Database Not Created
- The database is created automatically on first run
- Check that `appsettings.json` has the correct connection string
- Ensure the project directory has write permissions

### Login Issues
- Verify admin user was created (check `aman_hrm.db` exists)
- Clear browser cookies and try again
- Check that BCrypt package is properly installed

### Build Errors
- Run `dotnet restore` to restore all NuGet packages
- Ensure .NET 10 SDK is installed: `dotnet --version`
- Delete `bin` and `obj` folders and rebuild

## Technical Notes

### Password Storage
- New passwords are hashed using BCrypt
- Old passwords from Python app using PBKDF2 are still supported
- Passwords are salted and hashed before storage

### Session Management
- Sessions stored in secure cookies
- Cookie expires after 7 days of inactivity
- User must re-login if cookie expires

### Database Concurrency
- Entity Framework Core handles concurrency
- Optimistic concurrency checking available if needed

## Support

For issues or questions about the migration, refer to:
- `Data/SampleDataInitializer.cs` - See how sample data is created
- `Controllers/AuthController.cs` - See authentication implementation
- `Models/` - Review model definitions

## License

Same as the original Python AMAN project.
