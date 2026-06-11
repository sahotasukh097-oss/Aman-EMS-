# SQLite + JavaScript Integration Guide

## 🗄️ SQLite Database Configuration

Your project is now fully configured to work with SQLite and your JavaScript implementation:

### Database Setup (Already Configured)
- **Database File:** `aman_hrm.db` (created automatically)
- **Location:** Project root directory
- **Connection String:** `Data Source=aman_hrm.db`
- **Configuration:** `appsettings.json`

### Tables Created
- `Amanhr_users` - User accounts
- `Amanhr_employees` - Employee information
- `Amanhr_attendance` - Attendance records
- `Amanhr_leaves` - Leave requests
- `Amanhr_payroll` - Payroll records

### Database Initialization
The database is automatically created and initialized with sample data on application startup:
```csharp
// In Program.cs
dbContext.Database.EnsureCreated();
await SampleDataInitializer.InitializeAsync(dbContext);
```

---

## 🔌 API Endpoints for JavaScript

### Employees API
```
GET    /api/employees              - Get all employees
GET    /api/employees/{id}         - Get specific employee
POST   /api/employees              - Create new employee
PUT    /api/employees/{id}         - Update employee
DELETE /api/employees/{id}         - Delete employee
```

**Example Usage (JavaScript):**
```javascript
// Get all employees
API.employees.getAll()
    .then(employees => {
        console.log('Employees:', employees);
        // Render in table
    });

// Create employee
API.employees.create({
    firstName: 'John',
    lastName: 'Doe',
    department: 'IT',
    designation: 'Developer',
    salary: 50000
})
.then(() => AppUtils.showNotification('Employee created!', 'success'));

// Update employee
API.employees.update(1, {
    firstName: 'Jane',
    salary: 55000
})
.then(() => AppUtils.showNotification('Employee updated!', 'success'));

// Delete employee
API.employees.delete(1)
.then(() => AppUtils.showNotification('Employee deleted!', 'success'));
```

---

### Attendance API
```
GET    /api/attendance             - Get attendance by date
GET    /api/attendance/stats       - Get attendance statistics
POST   /api/attendance             - Mark attendance
```

**Example Usage (JavaScript):**
```javascript
// Get attendance for today
API.attendance.getByDate()
    .then(records => console.log('Today attendance:', records));

// Get attendance for specific date
API.attendance.getByDate(new Date('2025-01-15'))
    .then(records => console.log('Attendance:', records));

// Mark employee as present
API.attendance.mark(123, 'present')
    .then(() => AppUtils.showNotification('Present marked!', 'success'));

// Get attendance statistics
fetch('/api/attendance/stats')
    .then(r => r.json())
    .then(stats => {
        console.log('Total:', stats.totalEmployees);
        console.log('Present:', stats.presentToday);
        console.log('Percentage:', stats.attendancePercentage + '%');
    });
```

---

### Payroll API
```
GET    /api/payroll                - Get payroll by month/year
GET    /api/payroll/{id}           - Get specific payroll
GET    /api/payroll/stats          - Get payroll statistics
POST   /api/payroll/generate       - Generate payroll
```

**Example Usage (JavaScript):**
```javascript
// Get payroll for current month
API.payroll.getByMonth()
    .then(payrolls => console.log('Payroll:', payrolls));

// Get payroll for specific month
API.payroll.getByMonth('January', 2025)
    .then(payrolls => console.log('Payroll:', payrolls));

// Generate payroll
API.payroll.generate({
    month: 'January',
    year: 2025
})
.then(() => AppUtils.showNotification('Payroll generated!', 'success'));

// Get payroll statistics
fetch('/api/payroll/stats')
    .then(r => r.json())
    .then(stats => {
        console.log('Total Payroll:', AppUtils.formatCurrency(stats.totalPayroll));
        console.log('Average Salary:', AppUtils.formatCurrency(stats.averageSalary));
        console.log('Employees Processed:', stats.employeesProcessed);
    });
```

---

### Dashboard API
```
GET    /api/dashboard/stats                 - Get dashboard statistics
GET    /api/dashboard/monthly-attendance    - Get monthly attendance data
GET    /api/dashboard/department-distribution - Get department distribution
GET    /api/dashboard/recent-employees      - Get recently added employees
```

**Example Usage (JavaScript):**
```javascript
// Get all dashboard statistics
fetch('/api/dashboard/stats')
    .then(r => r.json())
    .then(stats => {
        document.getElementById('totalEmployees').textContent = stats.totalEmployees;
        document.getElementById('presentToday').textContent = stats.presentToday;
        document.getElementById('pendingLeaves').textContent = stats.pendingLeaves;
        document.getElementById('monthlyPayroll').textContent = 
            AppUtils.formatCurrency(stats.monthlyPayroll);
    });

// Get monthly attendance for charts
fetch('/api/dashboard/monthly-attendance')
    .then(r => r.json())
    .then(data => {
        // Use data to populate chart
        Dashboard.setupChartData();
    });

// Get department distribution
fetch('/api/dashboard/department-distribution')
    .then(r => r.json())
    .then(data => {
        console.log('Departments:', data.departments);
        console.log('Counts:', data.counts);
    });
```

---

## 🔄 Data Flow: SQLite → API → JavaScript

```
┌─────────────────────────────────────────────────────────┐
│             SQLite Database (aman_hrm.db)               │
│  ┌──────────────┬─────────────┬──────────┬────────────┐ │
│  │ Amanhr_      │ Amanhr_     │ Amanhr_  │ Amanhr_    │ │
│  │ employees    │ attendance  │ leaves   │ payroll    │ │
│  └──────────────┴─────────────┴──────────┴────────────┘ │
└────────────────────────┬────────────────────────────────┘
                         │
                         ↓
┌─────────────────────────────────────────────────────────┐
│        .NET API Controllers (C#)                        │
│  ┌──────────────┬──────────┬───────────┬────────────┐   │
│  │ Employees    │ Attendance│ Payroll  │ Dashboard  │   │
│  │ Controller   │ Controller│ Controller│ Controller │   │
│  └──────────────┴──────────┴───────────┴────────────┘   │
└────────────────────────┬────────────────────────────────┘
                         │
                         ↓ (HTTP REST API)
┌─────────────────────────────────────────────────────────┐
│        JavaScript (Browser)                             │
│  ┌──────────────┬──────────┬───────────┬────────────┐   │
│  │ API.employees│ API.     │ API.payroll
│  │              │ attendance│           │   API.     │   │
│  │              │          │           │ dashboard  │   │
│  └──────────────┴──────────┴───────────┴────────────┘   │
│                          ↓                              │
│              ┌─────────────────────────┐               │
│              │  AppUtils, TableUtils   │               │
│              │  Dashboard, Forms       │               │
│              └─────────────────────────┘               │
│                          ↓                              │
│              ┌─────────────────────────┐               │
│              │   User Interface (DOM)  │               │
│              └─────────────────────────┘               │
└─────────────────────────────────────────────────────────┘
```

---

## 📋 Complete Request/Response Examples

### Create Employee Example

**Request (JavaScript):**
```javascript
API.employees.create({
    firstName: 'John',
    lastName: 'Doe',
    phone: '1234567890',
    department: 'IT',
    designation: 'Software Developer',
    salary: 50000
})
```

**HTTP Request:**
```
POST /api/employees
Content-Type: application/json

{
    "firstName": "John",
    "lastName": "Doe",
    "phone": "1234567890",
    "department": "IT",
    "designation": "Software Developer",
    "salary": 50000
}
```

**Response (SQLite):**
- Data inserted into `Amanhr_employees` table
- Returns HTTP 201 Created

**Response (JSON):**
```json
{
    "id": 1,
    "name": "John Doe",
    "message": "Employee created successfully"
}
```

---

### Get Attendance Example

**Request (JavaScript):**
```javascript
API.attendance.getByDate(new Date('2025-01-15'))
```

**HTTP Request:**
```
GET /api/attendance?date=2025-01-15
```

**Database Query:**
```sql
SELECT * FROM Amanhr_attendance 
WHERE DATE(AttendanceDate) = '2025-01-15'
```

**Response (JSON):**
```json
[
    {
        "id": 1,
        "employeeId": 1,
        "employeeName": "John Doe",
        "date": "2025-01-15T00:00:00",
        "status": "present",
        "checkIn": "09:00:00",
        "checkOut": "18:00:00"
    }
]
```

---

### Get Dashboard Stats Example

**Request (JavaScript):**
```javascript
fetch('/api/dashboard/stats')
    .then(r => r.json())
    .then(stats => console.log(stats))
```

**Database Queries:**
```sql
SELECT COUNT(*) FROM Amanhr_employees;
SELECT COUNT(*) FROM Amanhr_attendance WHERE DATE(AttendanceDate) = TODAY();
SELECT COUNT(*) FROM Amanhr_leaves WHERE Status = 'pending';
SELECT SUM(NetSalary) FROM Amanhr_payroll WHERE MONTH(GeneratedAt) = CURRENT_MONTH();
```

**Response (JSON):**
```json
{
    "totalEmployees": 25,
    "presentToday": 20,
    "pendingLeaves": 3,
    "monthlyPayroll": 1250000,
    "attendancePercentage": 80
}
```

---

## 🔐 API Authentication

All API endpoints require authentication (Bearer token from login cookie):

```javascript
// Automatic with Fetch API (includes cookies)
fetch('/api/employees', {
    method: 'GET',
    credentials: 'include', // Automatically sends auth cookie
    headers: {
        'Content-Type': 'application/json'
    }
})
```

The `API` object in your JavaScript already handles this automatically!

---

## 🛠️ Development Workflow

### 1. Database Schema Changes
If you modify models, update the database:
```csharp
// In Package Manager Console
Add-Migration YourMigrationName
Update-Database
```

### 2. Add New API Endpoint
1. Create controller method
2. Return JSON response
3. Use in JavaScript with `API.xxx.yyy()`

### 3. Test API Endpoints
Open browser DevTools (F12) → Network tab → Perform action → Check requests

---

## 📊 Sample Data

The application automatically creates sample data on first run:

**Sample Employees:**
- Various employees in different departments (HR, IT, Finance, Operations)
- Each has name, email, salary, and joining date

**Sample Attendance:**
- Recent attendance records for all employees

**Sample Leaves:**
- Various leave requests in different statuses

**Sample Payroll:**
- Monthly payroll records with salary calculations

---

## 🐛 Troubleshooting

### API Returns 404
- ✅ Verify endpoint URL in JavaScript
- ✅ Check controller is in `Controllers` folder
- ✅ Verify route attribute `[Route("api/[controller]")]`
- ✅ Ensure controller inherits from `ControllerBase`

### API Returns 500 (Server Error)
- ✅ Check browser console for error details
- ✅ Check server logs in Visual Studio Output
- ✅ Verify database query syntax
- ✅ Check employee/data exists before querying

### Data Not Saving
- ✅ Verify `await _dbContext.SaveChangesAsync()` is called
- ✅ Check model relationships are correct
- ✅ Verify foreign keys exist

### Slow Performance
- ✅ Add `.Include()` for navigation properties
- ✅ Use `.Select()` to project specific columns
- ✅ Check database indexes
- ✅ Monitor with browser DevTools Network tab

---

## 📈 Performance Tips

1. **Use Pagination** for large result sets:
   ```javascript
   fetch('/api/employees?page=1&pageSize=10')
   ```

2. **Lazy Load** related data:
   ```csharp
   var employees = await _dbContext.Employees
       .Include(e => e.User)  // Load related user
       .ToListAsync();
   ```

3. **Cache** frequently accessed data:
   ```javascript
   let employeesCache = null;
   async function getEmployees() {
       if (!employeesCache) {
           employeesCache = await API.employees.getAll();
       }
       return employeesCache;
   }
   ```

4. **Debounce** search queries:
   ```javascript
   const search = AppUtils.debounce(function(term) {
       fetch(`/api/employees?search=${term}`)
   }, 300);
   ```

---

## 🔗 Related Documentation

- **JavaScript Setup:** See `JAVASCRIPT_QUICKSTART.md`
- **API Details:** See `api-client.js` in `wwwroot/js/`
- **Database Schema:** See models in `Models/` folder
- **Complete Examples:** See `JAVASCRIPT_EXAMPLES.md`

---

## ✅ Verification Checklist

- [x] SQLite database configured
- [x] API controllers created
- [x] Endpoints match JavaScript API calls
- [x] Sample data initialization working
- [x] CORS enabled for API calls
- [x] Authentication configured
- [x] Database migrations ready
- [x] Example data available

---

## 🚀 Next Steps

1. **Test the APIs** using browser DevTools Network tab
2. **Implement** frontend pages with your new tables
3. **Add** data attributes to HTML for interactivity
4. **Monitor** performance in production
5. **Optimize** queries based on usage patterns

**Your SQLite + JavaScript integration is complete and ready to use!** 🎉
