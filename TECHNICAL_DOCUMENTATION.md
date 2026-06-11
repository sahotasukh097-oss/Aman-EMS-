# AMAN HR Management System - Technical Documentation

## Overview

This document provides technical details about the C# ASP.NET Core conversion of the AMAN HR Management System.

## Architecture

### Layered Architecture

```
┌─────────────────────────────────────┐
│     Views (Razor Pages/HTML)        │
├─────────────────────────────────────┤
│     Controllers (Business Logic)    │
├─────────────────────────────────────┤
│     Models (Data Entities)          │
├─────────────────────────────────────┤
│     Data Context (EF Core)          │
├─────────────────────────────────────┤
│     SQLite Database                 │
└─────────────────────────────────────┘
```

## Component Details

### 1. Models Layer (`Models/`)

#### User.cs
```csharp
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string FullName { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public virtual ICollection<Employee> Employees { get; set; }
}
```

**Purpose:** Represents a user account in the system
**Database Table:** `Amanhr_users`
**Relations:** One-to-Many with Employee

#### Employee.cs
```csharp
public class Employee
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string EmployeeCode { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Phone { get; set; }
    public string Department { get; set; }
    public string Designation { get; set; }
    public DateTime? DateOfJoining { get; set; }
    public decimal? Salary { get; set; }
    public int Status { get; set; }
    public virtual User User { get; set; }
    public virtual ICollection<Attendance> Attendances { get; set; }
    public virtual ICollection<Leave> Leaves { get; set; }
    public virtual ICollection<Payroll> Payrolls { get; set; }
}
```

**Purpose:** Represents an employee record
**Database Table:** `Amanhr_employees`
**Relations:** Many-to-One with User, One-to-Many with Attendance/Leave/Payroll

#### Attendance.cs
```csharp
public class Attendance
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime AttendanceDate { get; set; }
    public TimeOnly? CheckIn { get; set; }
    public TimeOnly? CheckOut { get; set; }
    public string Status { get; set; } // present, absent, leave
    public DateTime CreatedAt { get; set; }
    public virtual Employee Employee { get; set; }
}
```

**Purpose:** Tracks employee attendance
**Database Table:** `Amanhr_attendance`
**Relations:** Many-to-One with Employee

#### Leave.cs
```csharp
public class Leave
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string LeaveType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; }
    public string Status { get; set; } // pending, approved, rejected
    public DateTime AppliedAt { get; set; }
    public virtual Employee Employee { get; set; }
}
```

**Purpose:** Manages leave requests
**Database Table:** `Amanhr_leaves`
**Relations:** Many-to-One with Employee

#### Payroll.cs
```csharp
public class Payroll
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string SalaryMonth { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal Allowances { get; set; }
    public decimal Deductions { get; set; }
    public decimal NetSalary { get; set; }
    public DateTime GeneratedAt { get; set; }
    public virtual Employee Employee { get; set; }
}
```

**Purpose:** Records payroll calculations
**Database Table:** `Amanhr_payroll`
**Relations:** Many-to-One with Employee

### 2. Data Access Layer (`Data/`)

#### ApplicationDbContext.cs

```csharp
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Attendance> Attendances { get; set; }
    public DbSet<Leave> Leaves { get; set; }
    public DbSet<Payroll> Payrolls { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Table configurations
        modelBuilder.Entity<User>().ToTable("Amanhr_users");
        modelBuilder.Entity<Employee>().ToTable("Amanhr_employees");
        // ... more configurations
    }
}
```

**Purpose:** Entity Framework Core database context
**Responsibilities:**
- Define DbSets for all entities
- Configure table names and relationships
- Manage database connections
- Execute queries

**Key Methods:**
- `SaveChangesAsync()` - Persist changes to database
- `Database.Migrate()` - Apply migrations
- `Users.Add()` - Add entity to context

#### SampleDataInitializer.cs

```csharp
public static class SampleDataInitializer
{
    public static async Task InitializeAsync(ApplicationDbContext context)
    {
        // Seed data logic
    }
}
```

**Purpose:** Seeds initial sample data
**Called From:** `Program.cs` on application startup
**Data Created:**
- 1 admin user (admin@aman.com)
- 10 sample employees with various departments

### 3. Controllers Layer (`Controllers/`)

#### AuthController.cs

**Routes & Actions:**

| Route | Method | Action | Purpose |
|-------|--------|--------|---------|
| `/Auth/Login` | GET | Login | Display login form |
| `/Auth/Login` | POST | Login | Process login request |
| `/Auth/Signup` | GET | Signup | Display signup form |
| `/Auth/Signup` | POST | Signup | Process registration |
| `/Auth/Logout` | POST | Logout | Logout user |
| `/Auth/Dashboard` | GET | Dashboard | Display dashboard |

**Login Flow:**
```
1. User submits credentials
2. Find user by email/username (case-insensitive)
3. Verify password using BCrypt
4. Create claims identity
5. Sign in with cookies
6. Redirect to dashboard
```

**Signup Flow:**
```
1. Validate all fields
2. Check password match & strength
3. Check email uniqueness
4. Create user with BCrypt hash
5. Save to database
6. Auto-login user
7. Redirect to dashboard
```

**Key Methods:**
- `VerifyPassword()` - BCrypt verification
- `VerifyPbkdf2Password()` - Legacy hash support
- `Dashboard()` - Load statistics

### 4. View Layer (`Views/`)

#### Login.cshtml
- Modern gradient background
- Email and password input fields
- Remember me checkbox
- Demo credentials display
- Error message display
- Link to signup page
- Responsive design with Bootstrap

#### Signup.cshtml
- Full name, email, password fields
- Password confirmation
- Form validation display
- Password strength requirements
- Link to login page
- Same styling as login

#### Dashboard.cshtml
- Statistics cards (total employees, present today, pending leaves, payroll)
- Quick action buttons
- Gradient icons for each statistic
- Responsive grid layout
- Font Awesome icons

## Authentication & Security

### Password Hashing

**Implementation:**
```csharp
// Create hash
user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

// Verify hash
bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
```

**Features:**
- BCrypt with automatic salt generation
- Work factor: 10
- Cost parameter: 12
- Resistant to rainbow table attacks

### Authentication Flow

```
1. User submits login form
2. Server validates input
3. Query database for user
4. Verify password hash
5. Create ClaimsIdentity
6. Sign in with CookieAuthenticationDefaults
7. Issue secure cookie to client
8. Client includes cookie with each request
9. Server validates cookie on protected routes
```

### Cookie Configuration

```csharp
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/Auth/Login";
    options.LogoutPath = "/Auth/Logout";
    options.AccessDeniedPath = "/Auth/Login";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
});
```

**Settings:**
- **LoginPath:** Where to redirect if not authenticated
- **ExpireTimeSpan:** Cookie expires after 7 days
- **SlidingExpiration:** Refreshes cookie on each request
- **AccessDeniedPath:** Where to redirect if unauthorized

### Claims

```csharp
var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    new Claim(ClaimTypes.Email, user.Email),
    new Claim(ClaimTypes.Name, user.FullName),
    new Claim("UserId", user.Id.ToString())
};
```

**Used For:**
- Identify user in views: `@User.Identity?.Name`
- Check authentication: `@User.Identity?.IsAuthenticated`
- Access user ID in controllers

## Database Schema

### Table Structure

**phphr_users**
```sql
CREATE TABLE phphr_users (
    Id INTEGER PRIMARY KEY,
    Username TEXT NOT NULL UNIQUE,
    Email TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    FullName TEXT NOT NULL,
    Status INTEGER DEFAULT 1,
    CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP
);
```

**phphr_employees**
```sql
CREATE TABLE phphr_employees (
    Id INTEGER PRIMARY KEY,
    UserId INTEGER NOT NULL,
    EmployeeCode TEXT UNIQUE,
    FirstName TEXT,
    LastName TEXT,
    Phone TEXT,
    Department TEXT,
    Designation TEXT,
    DateOfJoining TEXT,
    Salary REAL,
    Status INTEGER DEFAULT 1,
    CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES phphr_users(Id)
);
```

**phphr_attendance**
```sql
CREATE TABLE phphr_attendance (
    Id INTEGER PRIMARY KEY,
    EmployeeId INTEGER NOT NULL,
    AttendanceDate TEXT NOT NULL,
    CheckIn TEXT,
    CheckOut TEXT,
    Status TEXT DEFAULT 'present',
    CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (EmployeeId) REFERENCES phphr_employees(Id)
);
```

**phphr_leaves**
```sql
CREATE TABLE phphr_leaves (
    Id INTEGER PRIMARY KEY,
    EmployeeId INTEGER NOT NULL,
    LeaveType TEXT NOT NULL,
    StartDate TEXT NOT NULL,
    EndDate TEXT NOT NULL,
    Reason TEXT,
    Status TEXT DEFAULT 'pending',
    AppliedAt TEXT DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (EmployeeId) REFERENCES phphr_employees(Id)
);
```

**phphr_payroll**
```sql
CREATE TABLE phphr_payroll (
    Id INTEGER PRIMARY KEY,
    EmployeeId INTEGER NOT NULL,
    SalaryMonth TEXT,
    BasicSalary REAL NOT NULL,
    Allowances REAL DEFAULT 0,
    Deductions REAL DEFAULT 0,
    NetSalary REAL NOT NULL,
    GeneratedAt TEXT DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (EmployeeId) REFERENCES phphr_employees(Id)
);
```

## Configuration

### Program.cs Startup

```csharp
// 1. Create builder
var builder = WebApplication.CreateBuilder(args);

// 2. Add services
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddAuthentication(...);
builder.Services.AddAuthorization();

// 3. Build app
var app = builder.Build();

// 4. Initialize database
using (var scope = app.Services.CreateScope()) {
    var dbContext = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
    await SampleDataInitializer.InitializeAsync(dbContext);
}

// 5. Configure middleware
app.UseAuthentication();
app.UseAuthorization();

// 6. Map routes
app.MapControllerRoute(...)

app.Run();
```

### appsettings.json

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

## NuGet Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| Microsoft.EntityFrameworkCore.Sqlite | 10.0.0 | SQLite provider for EF Core |
| Microsoft.EntityFrameworkCore.Tools | 10.0.0 | Migration tools |
| BCrypt.Net-Next | 4.0.3 | Password hashing |

## Development Workflow

### Adding a New Feature

1. **Create Model** in `Models/`
2. **Add DbSet** to `ApplicationDbContext`
3. **Create Controller** in `Controllers/`
4. **Create Views** in `Views/`
5. **Build and Test**

### Database Changes

1. **Modify Model** in `Models/`
2. **Update Context** in `ApplicationDbContext`
3. **Run Migration:**
   ```bash
   dotnet ef migrations add DescriptionOfChange
   dotnet ef database update
   ```

### Testing Authentication

1. Use browser DevTools to inspect cookies
2. Check `Application` → `Cookies` for auth cookie
3. Name: `.AspNetCore.Cookies`
4. Verify it's HttpOnly and Secure

## Performance Considerations

### Database Optimization
- SQLite suitable for small deployments
- For production: SQL Server or PostgreSQL
- Consider indexing on frequently searched fields
- Use `.AsNoTracking()` for read-only queries

### Caching
- Implement caching for employee lists
- Cache dashboard statistics
- Use `IDistributedCache` for distributed scenarios

### Async Operations
- All database operations are async
- Use `await` in controllers
- Improve scalability and responsiveness

## Error Handling

### Try-Catch Pattern
```csharp
try {
    _context.Users.Add(user);
    await _context.SaveChangesAsync();
} catch (Exception ex) {
    ModelState.AddModelError("", $"Error: {ex.Message}");
    return View(model);
}
```

### Validation
- Model-level validation via data annotations
- Business logic validation in controllers
- Client-side validation via HTML5 attributes

## Testing

### Manual Testing Checklist
- [ ] Can login with admin credentials
- [ ] Can register new account
- [ ] Can logout
- [ ] Dashboard loads correctly
- [ ] Statistics display properly
- [ ] Remember me functionality works
- [ ] Session expires correctly
- [ ] Unauthorized access redirects to login

### Unit Testing (Future)
```csharp
[TestClass]
public class AuthControllerTests
{
    [TestMethod]
    public async Task Login_WithValidCredentials_ReturnsRedirect() { }
    
    [TestMethod]
    public async Task Signup_WithDuplicateEmail_ReturnsError() { }
}
```

## Troubleshooting

### Issue: Database not created
**Solution:** Check `appsettings.json` connection string and delete old db file

### Issue: Login fails
**Solution:** Verify BCrypt package is installed, check password hash format

### Issue: Migrations failed
**Solution:** Run `dotnet ef migrations add [name] --no-build` and check errors

## Deployment

### Publish to Production
```bash
dotnet publish -c Release -o ./publish
```

### Docker Deployment
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY publish/ .
ENTRYPOINT ["dotnet", "Aman(EMS).dll"]
```

### Database Backup
- Copy `aman_hrm.db` file to backup location
- Implement automated backups for production
- Consider database replication

## References

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [BCrypt.Net-Next](https://github.com/BcryptNet/bcrypt.net-next)
- [Cookie Authentication](https://docs.microsoft.com/aspnet/core/security/authentication/cookie)

---

**Last Updated:** 2024
**Version:** 1.0
**Status:** Complete - Ready for Production
