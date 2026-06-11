# SQLite + JavaScript Quick Reference

## ⚡ Quick Commands

### Start Using the API
```javascript
// Get all employees
const employees = await API.employees.getAll();

// Get specific employee
const emp = await API.employees.getById(1);

// Create employee
await API.employees.create({
    firstName: 'John',
    lastName: 'Doe',
    department: 'IT',
    designation: 'Developer',
    salary: 50000
});

// Update employee
await API.employees.update(1, {
    firstName: 'Jane',
    salary: 55000
});

// Delete employee
await API.employees.delete(1);

// Mark attendance
await API.attendance.mark(1, 'present');

// Get attendance for today
const att = await API.attendance.getByDate();

// Get attendance stats
const stats = await fetch('/api/attendance/stats').then(r => r.json());

// Generate payroll
await API.payroll.generate({
    month: 'January',
    year: 2025
});

// Get payroll
const payroll = await API.payroll.getByMonth('January', 2025);

// Get dashboard stats
const dash = await fetch('/api/dashboard/stats').then(r => r.json());
```

---

## 📍 API Endpoints Quick Reference

### Employees
```
GET    /api/employees              Get all
GET    /api/employees/1            Get by ID
POST   /api/employees              Create
PUT    /api/employees/1            Update
DELETE /api/employees/1            Delete
```

### Attendance
```
GET    /api/attendance             Get by date
GET    /api/attendance/stats       Get stats
POST   /api/attendance             Mark attendance
```

### Payroll
```
GET    /api/payroll                Get by month
GET    /api/payroll/1              Get by ID
GET    /api/payroll/stats          Get stats
POST   /api/payroll/generate       Generate
```

### Dashboard
```
GET    /api/dashboard/stats                    Stats
GET    /api/dashboard/monthly-attendance       Monthly data
GET    /api/dashboard/department-distribution  Departments
GET    /api/dashboard/recent-employees         Recent
```

---

## 🎯 Common Tasks

### Task 1: Display All Employees in Table
```javascript
// JavaScript
API.employees.getAll()
    .then(employees => {
        const tbody = document.querySelector('tbody');
        employees.forEach(emp => {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${emp.id}</td>
                <td>${emp.name}</td>
                <td>${emp.email}</td>
                <td>${emp.department}</td>
            `;
            tbody.appendChild(row);
        });
    });
```

### Task 2: Create Employee from Form
```javascript
// HTML
<form id="empForm">
    <input name="firstName" required>
    <input name="lastName" required>
    <input name="department">
    <input name="designation">
    <input name="salary" type="number">
    <button type="submit">Create</button>
</form>

// JavaScript
document.getElementById('empForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    const data = new FormData(e.target);
    await API.employees.create(Object.fromEntries(data));
    AppUtils.showNotification('Employee created!', 'success');
    e.target.reset();
});
```

### Task 3: Show Dashboard Stats
```javascript
// HTML
<div class="stat-card">
    <p id="totalEmployees">0</p>
</div>
<div class="stat-card">
    <p id="presentToday">0</p>
</div>

// JavaScript
fetch('/api/dashboard/stats')
    .then(r => r.json())
    .then(stats => {
        document.getElementById('totalEmployees').textContent = stats.totalEmployees;
        document.getElementById('presentToday').textContent = stats.presentToday;
    });
```

### Task 4: Mark Attendance
```javascript
// HTML
<button onclick="markPresent(123)">Present</button>

// JavaScript
async function markPresent(employeeId) {
    await API.attendance.mark(employeeId, 'present');
    AppUtils.showNotification('Marked present', 'success');
}
```

### Task 5: Searchable Employee Table
```html
<!-- HTML -->
<input type="text" id="search" placeholder="Search employees...">
<table id="empTable" data-sortable="true">
    <!-- Headers -->
</table>

<!-- JavaScript -->
<script>
document.getElementById('search').addEventListener('input', async (e) => {
    const employees = await API.employees.getAll();
    const filtered = employees.filter(emp => 
        emp.name.toLowerCase().includes(e.target.value.toLowerCase())
    );
    // Render filtered employees
});
</script>
```

---

## 🔧 API Response Examples

### Get Employees Response
```json
[
    {
        "id": 1,
        "name": "John Doe",
        "firstName": "John",
        "lastName": "Doe",
        "email": "john@example.com",
        "department": "IT",
        "designation": "Developer",
        "salary": 50000,
        "status": 1,
        "dateOfJoining": "2024-01-15T00:00:00"
    }
]
```

### Dashboard Stats Response
```json
{
    "totalEmployees": 25,
    "presentToday": 20,
    "pendingLeaves": 3,
    "monthlyPayroll": 1250000,
    "attendancePercentage": 80
}
```

### Mark Attendance Response
```json
{
    "message": "Attendance marked successfully"
}
```

---

## 📊 Database Tables

### Amanhr_employees
```
Id                 int PRIMARY KEY
UserId             int FOREIGN KEY
EmployeeCode       string(50)
FirstName          string(100)
LastName           string(100)
Phone              string(20)
Department         string(200)
Designation        string(100)
DateOfJoining      datetime
Salary             decimal
Status             int (1=Active, 0=Inactive)
CreatedAt          datetime
```

### Amanhr_attendance
```
Id                 int PRIMARY KEY
EmployeeId         int FOREIGN KEY
AttendanceDate     datetime
CheckIn            time
CheckOut           time
Status             string(20) - "present", "absent", "leave"
CreatedAt          datetime
```

### Amanhr_payroll
```
Id                 int PRIMARY KEY
EmployeeId         int FOREIGN KEY
SalaryMonth        string(20) - "January", "February", etc
BasicSalary        decimal
Allowances         decimal
Deductions         decimal
NetSalary          decimal
GeneratedAt        datetime
```

---

## ❌ Error Handling

```javascript
// Basic error handling
API.employees.getAll()
    .then(employees => {
        // Success
    })
    .catch(error => {
        console.error('Error:', error);
        AppUtils.showNotification('Failed to load employees', 'danger');
    });

// With async/await
try {
    const employees = await API.employees.getAll();
    console.log(employees);
} catch (error) {
    AppUtils.showNotification('Error: ' + error.message, 'danger');
}
```

---

## 🔍 Debugging Tips

### Check if API is Working
```javascript
// In browser console (F12)
await API.employees.getAll()  // Should return array
```

### Monitor Network Requests
1. Open DevTools (F12)
2. Go to Network tab
3. Perform action
4. Click on request to see details
5. Check Response tab for data

### Check Database
```csharp
// In Visual Studio Debug Console
db.Employees.Count()  // Should show count
```

---

## ✅ Checklist

Before deploying:
- [ ] API endpoints respond correctly
- [ ] Database has sample data
- [ ] JavaScript console shows no errors
- [ ] Authentication is working
- [ ] CORS is configured
- [ ] Hot reload updates API code
- [ ] Network requests show 200 OK
- [ ] Response data is in correct format

---

## 📞 Quick Support

**API not working?**
→ Check Network tab in DevTools for response details

**Database empty?**
→ Run application to auto-initialize (delete `.db` file if needed)

**JavaScript errors?**
→ Open F12 → Console tab → Look for red errors

**Slow performance?**
→ Check Network tab → Look for slow API calls

**404 errors?**
→ Verify endpoint URLs in `api-client.js`

---

## 🚀 Ready to Use!

Your SQLite database is connected to JavaScript through REST APIs.

Start building your features! 🎉
