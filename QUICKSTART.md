# AMAN HR Management - Quick Start Guide

## What's New

Your Python HR Management System has been successfully converted to C# ASP.NET Core!

## How to Run

### Step 1: Build the Project
```bash
dotnet build
```

### Step 2: Run the Application
```bash
dotnet run
```

### Step 3: Access the Application
- Open your browser
- Navigate to: `https://localhost:7000` (or the URL shown in terminal)
- You'll see the home page with public information

## Demo Account

**Admin Login:**
- Email: `admin@aman.com`
- Password: `123456`

**How to Login:**
1. Click "Demo" or "Login" in the navigation menu
2. Enter admin credentials
3. You'll be redirected to the dashboard

## What Can You Do

### Public Features (No Login Required)
- View Home page
- Read About us
- Explore Features
- Browse FAQ
- Contact us
- Read Presentation

### Demo Features (With Login)
- View Dashboard with statistics
- Manage employees (structure in place)
- Record attendance (structure in place)
- Manage leaves (structure in place)
- Generate payroll (structure in place)

## Database

- **Type:** SQLite (portable, no server needed)
- **Location:** `aman_hrm.db` (created automatically)
- **Sample Data:** 10 employees auto-created on first run

### Sample Employees Created
1. John Smith - Engineering
2. Sarah Johnson - Marketing
3. Michael Brown - Sales
4. Emily Davis - HR
5. David Wilson - Finance
6. Lisa Garcia - Engineering
7. Robert Martinez - Operations
8. Jennifer Anderson - Customer Service
9. James Taylor - Engineering
10. Maria Rodriguez - Marketing

All have password: `password123`

## Project Structure

```
📁 Aman(EMS)/
├── 📁 Controllers/
│   ├── HomeController.cs (Public pages)
│   └── AuthController.cs (Login/Signup/Dashboard)
├── 📁 Models/ (Database entities)
│   ├── User.cs
│   ├── Employee.cs
│   ├── Attendance.cs
│   ├── Leave.cs
│   └── Payroll.cs
├── 📁 Data/
│   ├── ApplicationDbContext.cs (Database context)
│   └── SampleDataInitializer.cs (Sample data)
├── 📁 Views/
│   ├── 📁 Auth/ (NEW - Login, Signup, Dashboard)
│   ├── 📁 Home/ (Public pages)
│   └── 📁 Shared/ (Layout & partials)
├── Program.cs (Application configuration)
├── appsettings.json (Settings including database)
└── Aman(EMS).csproj (Project file)
```

## Pages Available

### Public Pages (No Login)
- `/` - Home
- `/Home/About` - About us
- `/Home/Features` - Features
- `/Home/Faq` - FAQ
- `/Home/Contact` - Contact

### Authentication Pages
- `/Auth/Login` - Login form
- `/Auth/Signup` - Register new account
- `/Auth/Dashboard` - Dashboard (login required)
- `/Auth/Logout` - Logout (logout required)

## Key Features Implemented

✅ **Authentication**
- User login and registration
- Secure password hashing with BCrypt
- Session management
- Remember me functionality

✅ **Database**
- SQLite database (fully local)
- 5 main entities (User, Employee, Attendance, Leave, Payroll)
- Automatic database creation
- Sample data seeding

✅ **User Interface**
- Modern, responsive design
- Beautiful login/signup forms
- Dashboard with statistics
- Navigation with auth status

✅ **Security**
- Password hashing
- CSRF protection
- SQL injection prevention
- Secure cookies

## Next Steps

### To Test the System
1. Run the application
2. Go through public pages first (Home, About, etc.)
3. Click "Demo" or "Login"
4. Use `admin@aman.com` / `123456`
5. Explore the Dashboard

### To Extend the System
See `MIGRATION_GUIDE.md` for:
- How to add employee management
- How to implement attendance tracking
- How to create leave management
- How to generate payroll reports

## Troubleshooting

### Database Issues
- Delete `aman_hrm.db` to reset the database
- On next run, a fresh database with sample data will be created

### Login Issues
- Try clearing browser cookies
- Restart the application
- Verify you have the correct email: `admin@aman.com`

### Build Issues
- Run: `dotnet restore`
- Delete `bin` and `obj` folders
- Run: `dotnet build` again

## Technologies Used

- **Framework:** ASP.NET Core 10
- **Database:** Entity Framework Core with SQLite
- **Authentication:** ASP.NET Core Identity (Cookies)
- **Password Hashing:** BCrypt.Net-Next
- **UI Framework:** Bootstrap 5
- **Icons:** Font Awesome 6

## Files Modified/Created

**New Files:**
- `Controllers/AuthController.cs`
- `Models/User.cs`, `Employee.cs`, `Attendance.cs`, `Leave.cs`, `Payroll.cs`
- `Data/ApplicationDbContext.cs`
- `Data/SampleDataInitializer.cs`
- `Views/Auth/Login.cshtml`
- `Views/Auth/Signup.cshtml`
- `Views/Auth/Dashboard.cshtml`
- `MIGRATION_GUIDE.md`
- `QUICKSTART.md` (this file)

**Modified Files:**
- `Program.cs` - Added EF Core and authentication
- `appsettings.json` - Added database connection string
- `Aman(EMS).csproj` - Added NuGet packages
- `Views/Shared/_Layout.cshtml` - Added authentication links

## Support & Documentation

For detailed information about:
- Migration details → See `MIGRATION_GUIDE.md`
- Code structure → See comments in source files
- API endpoints → See `AuthController.cs`

## Demo Walk-through

1. **Start Application**
   ```bash
   dotnet run
   ```

2. **View Public Pages**
   - Navigate to home page
   - Check About, Features, FAQ, Contact pages

3. **Login to Demo**
   - Click "Demo" in navigation
   - Email: `admin@aman.com`
   - Password: `123456`
   - Click "Login"

4. **Explore Dashboard**
   - See total employees (10)
   - See present employees, pending leaves, payroll
   - View quick action buttons

5. **Logout**
   - Click "Logout" in navigation
   - You'll be redirected to login page

## Configuration

### Database Connection
Edit `appsettings.json` to change database location:
```json
"ConnectionStrings": {
    "DefaultConnection": "Data Source=aman_hrm.db"
}
```

### Authentication Cookie Settings
Edit `Program.cs` to modify:
- Cookie expiration time
- Login redirect URL
- Logout redirect URL

## Performance Tips

- SQLite is suitable for small to medium deployments
- For production, consider upgrading to SQL Server or PostgreSQL
- The application automatically handles database creation and seeding

---

**Enjoy your new C# AMAN HR Management System!** 🚀
