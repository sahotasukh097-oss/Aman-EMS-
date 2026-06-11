# 🎯 SQLite + JavaScript - Getting Started in 5 Minutes

## ✨ What You Now Have

Your AMAN HR Management System is now fully integrated with:
- ✅ SQLite Database (auto-created)
- ✅ 5 REST API Controllers  
- ✅ 15+ API Endpoints
- ✅ Complete JavaScript Integration
- ✅ Sample Data & Initialization

---

## 🚀 Quick Start

### Step 1: Run Your Application
```bash
# In Visual Studio
Press F5  # or Ctrl+F5

# Or in Terminal
dotnet run
```

### Step 2: Wait for Database
- The app creates `aman_hrm.db` automatically
- Sample data is loaded
- API is ready to use

### Step 3: Test in Browser
Open DevTools (F12) and paste:
```javascript
await API.employees.getAll()
```
You should see an array of employees!

---

## 💻 Common Code Snippets

### Display Employee List
```html
<!-- Add this to your page -->
<table id="employees" data-sortable="true" data-paginate="10">
    <thead>
        <tr>
            <th data-sortable="number">ID</th>
            <th data-sortable="text">Name</th>
            <th data-sortable="text">Department</th>
            <th data-sortable="number">Salary</th>
        </tr>
    </thead>
    <tbody id="empBody"></tbody>
</table>

<script>
API.employees.getAll().then(emps => {
    const html = emps.map(e => `
        <tr>
            <td>${e.id}</td>
            <td>${e.name}</td>
            <td>${e.department}</td>
            <td>${AppUtils.formatCurrency(e.salary)}</td>
        </tr>
    `).join('');
    document.getElementById('empBody').innerHTML = html;
});
</script>
```

### Add Employee Form
```html
<form id="addForm">
    <input name="firstName" placeholder="First Name" required>
    <input name="lastName" placeholder="Last Name" required>
    <input name="department" placeholder="Department" required>
    <input name="designation" placeholder="Designation" required>
    <input name="salary" type="number" placeholder="Salary" required>
    <button type="submit">Add Employee</button>
</form>

<script>
document.getElementById('addForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    const data = Object.fromEntries(new FormData(e.target));
    await API.employees.create(data);
    AppUtils.showNotification('Employee added!', 'success');
    e.target.reset();
});
</script>
```

### Show Dashboard Stats
```html
<div class="dashboard">
    <div class="stat">Total Employees: <span id="total">0</span></div>
    <div class="stat">Present Today: <span id="present">0</span></div>
    <div class="stat">Pending Leaves: <span id="leaves">0</span></div>
    <div class="stat">Payroll: <span id="payroll">$0</span></div>
</div>

<script>
fetch('/api/dashboard/stats')
    .then(r => r.json())
    .then(stats => {
        document.getElementById('total').textContent = stats.totalEmployees;
        document.getElementById('present').textContent = stats.presentToday;
        document.getElementById('leaves').textContent = stats.pendingLeaves;
        document.getElementById('payroll').textContent = 
            AppUtils.formatCurrency(stats.monthlyPayroll);
    });
</script>
```

### Mark Attendance
```html
<button onclick="markPresent(123)">Mark Present</button>

<script>
async function markPresent(empId) {
    try {
        await API.attendance.mark(empId, 'present');
        AppUtils.showNotification('Marked present!', 'success');
    } catch (error) {
        AppUtils.showNotification('Error marking attendance', 'danger');
    }
}
</script>
```

---

## 🔗 All Available APIs

```javascript
// ===== EMPLOYEES =====
API.employees.getAll()              // Get all
API.employees.getById(1)            // Get one
API.employees.create({...})         // Create
API.employees.update(1, {...})      // Update
API.employees.delete(1)             // Delete

// ===== ATTENDANCE =====
API.attendance.getByDate(date)      // Get by date
API.attendance.mark(empId, status)  // Mark attendance

// ===== PAYROLL =====
API.payroll.getByMonth(month, year)     // Get payroll
API.payroll.generate({month, year})     // Generate

// ===== DASHBOARD =====
fetch('/api/dashboard/stats')
fetch('/api/dashboard/monthly-attendance')
fetch('/api/dashboard/department-distribution')
fetch('/api/dashboard/recent-employees')

// ===== UTILITIES =====
AppUtils.showNotification(msg, type)    // Show alert
AppUtils.formatCurrency(amount)         // Format money
AppUtils.formatDate(date)               // Format date
AppUtils.debounce(func, ms)             // Throttle calls
```

---

## 📊 Database Tables (What's Available)

```
Employees Table:
├─ id, userId, firstName, lastName, phone
├─ department, designation, salary
├─ dateOfJoining, status, createdAt
└─ Navigation: User, Attendances, Leaves, Payrolls

Attendance Table:
├─ id, employeeId, attendanceDate
├─ checkIn, checkOut, status, createdAt
└─ Navigation: Employee

Payroll Table:
├─ id, employeeId, salaryMonth
├─ basicSalary, allowances, deductions, netSalary
├─ generatedAt
└─ Navigation: Employee

Leaves Table:
├─ id, employeeId, leaveType
├─ startDate, endDate, reason, status
├─ appliedAt
└─ Navigation: Employee
```

---

## ⚡ One-Liners

```javascript
// Get employee count
(await API.employees.getAll()).length

// Get total salary
(await API.employees.getAll())
  .reduce((sum, e) => sum + e.salary, 0)

// Mark multiple employees present
[1,2,3].forEach(id => API.attendance.mark(id, 'present'))

// Get high-salary employees
(await API.employees.getAll())
  .filter(e => e.salary > 50000)

// Format a response
AppUtils.formatCurrency(
  (await fetch('/api/dashboard/stats').then(r => r.json()))
  .monthlyPayroll
)
```

---

## 🐛 If Something Doesn't Work

### API Returns 404
```javascript
// Check URL in api-client.js
// Make sure endpoint exists in controllers
// Verify spelling and case
```

### API Returns 500
```javascript
// Check browser console (F12) for details
// Look at server output in Visual Studio
// Verify table exists in database
```

### JavaScript Shows No Data
```javascript
// Open F12 → Network tab
// Look for failed requests
// Check response body for errors
// Try calling API directly in console
```

### Database Not Creating
```csharp
// In Visual Studio Debug Console
// Manually initialize:
db.Database.EnsureCreated()
```

---

## 📋 Checklist - Verify Everything Works

- [ ] Application runs without errors
- [ ] `aman_hrm.db` file exists in project root
- [ ] `await API.employees.getAll()` returns data in console
- [ ] No red errors in browser console (F12)
- [ ] Network tab shows requests succeeding (200 OK)
- [ ] Visual Studio build shows "Build successful"
- [ ] Can add new employee and see it appear
- [ ] Dashboard stats load correctly

---

## 🎯 Your First Task

**Display a searchable employee table on a page:**

```html
<!-- In your Razor Page -->
<input type="text" id="search" placeholder="Search...">
<table id="empTable" data-sortable="true" data-paginate="10">
    <thead>
        <tr>
            <th data-sortable="number">ID</th>
            <th data-sortable="text">Name</th>
            <th data-sortable="text">Department</th>
        </tr>
    </thead>
    <tbody></tbody>
</table>

<script>
// Load data
API.employees.getAll().then(employees => {
    const tbody = document.querySelector('#empTable tbody');
    employees.forEach(emp => {
        const row = tbody.insertRow();
        row.innerHTML = `<td>${emp.id}</td><td>${emp.name}</td><td>${emp.department}</td>`;
    });
});

// Enable search
document.getElementById('search').addEventListener('keyup', function(e) {
    const search = e.target.value.toLowerCase();
    document.querySelectorAll('#empTable tbody tr').forEach(row => {
        const text = row.textContent.toLowerCase();
        row.style.display = text.includes(search) ? '' : 'none';
    });
});
</script>
```

**Result:** Fully functional searchable table! 🎉

---

## 📚 Deep Dive Resources

- `SQLITE_JAVASCRIPT_INTEGRATION.md` - Complete guide
- `SQLITE_QUICK_REFERENCE.md` - Quick commands
- `JAVASCRIPT_QUICKSTART.md` - JavaScript setup
- `JAVASCRIPT_EXAMPLES.md` - More code samples

---

## 🚀 Next Features to Add

Priority order:
1. Employee management page (CRUD)
2. Attendance marking
3. Dashboard with statistics
4. Leave request system
5. Payroll generation
6. Search and filtering
7. Data export
8. Reports

---

## 💡 Pro Tips

1. **Use DevTools Console** - Test API calls directly
2. **Check Network Tab** - See all requests and responses
3. **Use Postman** - Test endpoints outside browser
4. **Enable Hot Reload** - Changes apply without rebuilding
5. **Check Browser Cache** - Clear if data seems stale

---

## 🎉 You're Ready!

Your system is fully set up and ready to use.

- SQLite database is configured
- REST APIs are running
- JavaScript integration is complete
- Sample data is available
- Documentation is comprehensive

**Start building awesome features!** 🚀

---

**Questions?** Check the documentation files or review the code comments in:
- `Controllers/` - See how APIs work
- `wwwroot/js/api-client.js` - See available endpoints
- `Models/` - See database structure

Good luck! 💪
