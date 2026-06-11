# JavaScript Integration Examples

## Form Validation Examples

### Login Form
```html
<form method="post" class="login-form needs-validation">
    <div class="form-group">
        <label for="Email">Email Address</label>
        <input type="email" id="Email" name="Email" class="form-control" 
               placeholder="Enter your email" required>
    </div>

    <div class="form-group">
        <label for="Password">Password</label>
        <input type="password" id="Password" name="Password" class="form-control" 
               placeholder="Enter your password" required>
    </div>

    <button type="submit" class="btn btn-primary">Login</button>
</form>
```
**Features:** Email validation, password strength indicator, auto-submit loading state

### Signup Form
```html
<form method="post" class="signup-form needs-validation">
    <div class="form-group">
        <label for="Name">Full Name</label>
        <input type="text" id="Name" name="Name" class="form-control" required>
    </div>

    <div class="form-group">
        <label for="Email">Email Address</label>
        <input type="email" id="Email" name="Email" class="form-control" required>
    </div>

    <div class="form-group">
        <label for="Password">Password</label>
        <input type="password" id="Password" name="Password" class="form-control" required>
    </div>

    <div class="form-group">
        <label for="ConfirmPassword">Confirm Password</label>
        <input type="password" id="ConfirmPassword" name="ConfirmPassword" 
               class="form-control" required>
    </div>

    <button type="submit" class="btn btn-primary">Sign Up</button>
</form>
```
**Features:** All login features plus password confirmation matching

## Dashboard Examples

### Stat Cards with Animations
```html
<div class="dashboard-stats">
    <div class="stat-card">
        <div class="stat-icon employees">
            <i class="fas fa-users"></i>
        </div>
        <div class="stat-content">
            <h3>Total Employees</h3>
            <p class="stat-number" id="totalEmployees">0</p>
        </div>
    </div>

    <div class="stat-card">
        <div class="stat-icon present">
            <i class="fas fa-check-circle"></i>
        </div>
        <div class="stat-content">
            <h3>Present Today</h3>
            <p class="stat-number" id="presentToday">0</p>
        </div>
    </div>

    <div class="stat-card">
        <div class="stat-icon leaves">
            <i class="fas fa-calendar-times"></i>
        </div>
        <div class="stat-content">
            <h3>Pending Leaves</h3>
            <p class="stat-number" id="pendingLeaves">0</p>
        </div>
    </div>

    <div class="stat-card">
        <div class="stat-icon payroll">
            <i class="fas fa-dollar-sign"></i>
        </div>
        <div class="stat-content">
            <h3>Monthly Payroll</h3>
            <p class="stat-number" id="monthlyPayroll">$0</p>
        </div>
    </div>
</div>
```
**Features:** Hover animations, dynamic data updates

### Charts with Chart.js
```html
<div class="row">
    <div class="col-md-6">
        <div class="card">
            <div class="card-header">
                <h5>Monthly Attendance</h5>
            </div>
            <div class="card-body">
                <canvas id="attendanceChart"></canvas>
            </div>
        </div>
    </div>

    <div class="col-md-6">
        <div class="card">
            <div class="card-header">
                <h5>Department Distribution</h5>
            </div>
            <div class="card-body">
                <canvas id="departmentChart"></canvas>
            </div>
        </div>
    </div>
</div>
```
**Features:** Automatically initialized line and doughnut charts

## Table Features Examples

### Searchable, Sortable, Paginated Table
```html
<div class="mb-3">
    <input type="text" class="form-control" 
           data-table-search="employeeTable" 
           placeholder="Search employees...">
</div>

<table id="employeeTable" class="table table-striped" 
       data-sortable="true" data-paginate="10">
    <thead>
        <tr>
            <th data-sortable="number">ID</th>
            <th data-sortable="text">Name</th>
            <th data-sortable="text">Email</th>
            <th data-sortable="text">Department</th>
            <th data-sortable="text">Status</th>
            <th>Actions</th>
        </tr>
    </thead>
    <tbody>
        <!-- Rows populated by JavaScript -->
    </tbody>
</table>
```
**Features:**
- Click column headers to sort
- Type in search box to filter
- Automatic pagination controls

### Table with Select All
```html
<table class="table">
    <thead>
        <tr>
            <th>
                <input type="checkbox" id="selectAll">
            </th>
            <th>Name</th>
            <th>Email</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td><input type="checkbox"></td>
            <td>John Doe</td>
            <td>john@example.com</td>
        </tr>
    </tbody>
</table>
```
**Features:** "Select All" checkbox syncs with row checkboxes

## API Usage Examples

### In JavaScript
```javascript
// Get all employees
API.employees.getAll()
    .then(employees => {
        console.log('Employees loaded:', employees);
    })
    .catch(error => {
        AppUtils.showNotification('Failed to load employees', 'danger');
    });

// Mark attendance
API.attendance.mark(employeeId, 'present')
    .then(result => {
        AppUtils.showNotification('Attendance marked successfully', 'success');
    });

// Request leave
API.leaves.request({
    employeeId: 123,
    startDate: '2025-02-01',
    endDate: '2025-02-05',
    reason: 'Vacation'
})
    .then(result => {
        AppUtils.showNotification('Leave request submitted', 'success');
    });

// Get payroll data
API.payroll.getByMonth(1, 2025)
    .then(payroll => {
        console.log('Payroll data:', payroll);
    });
```

### In HTML with onclick handlers
```html
<button onclick="loadEmployees()">Load Employees</button>
<button onclick="markAttendance(123)">Mark Present</button>

<script>
function loadEmployees() {
    API.employees.getAll()
        .then(employees => {
            // Handle data
        });
}

function markAttendance(employeeId) {
    API.attendance.mark(employeeId, 'present')
        .then(() => {
            AppUtils.showNotification('Attendance marked', 'success');
        });
}
</script>
```

## Notification/Alert Examples

### Success Notification
```javascript
AppUtils.showNotification('Operation completed successfully', 'success');
```

### Error Notification
```javascript
AppUtils.showNotification('An error occurred while saving', 'danger');
```

### Warning Notification
```javascript
AppUtils.showNotification('Please review the data before submitting', 'warning');
```

### Info Notification
```javascript
AppUtils.showNotification('New employees added to the system', 'info');
```

## Utility Function Examples

### Format Currency
```javascript
const salary = 50000;
console.log(AppUtils.formatCurrency(salary));  // Output: "$50,000.00"
```

### Format Date
```javascript
const dateStr = '2025-01-15T10:30:00';
console.log(AppUtils.formatDate(dateStr));  // Output: "January 15, 2025"
```

### Debounce Function
```javascript
const searchInput = document.getElementById('search');
const debouncedSearch = AppUtils.debounce(function(value) {
    API.employees.getAll()  // Make API call
        .then(data => {
            console.log('Search results:', data);
        });
}, 300);

searchInput.addEventListener('input', function(e) {
    debouncedSearch(e.target.value);
});
```

## Bootstrap Integration Examples

### Tooltips
```html
<button type="button" class="btn btn-secondary" data-bs-toggle="tooltip" 
        title="Click to delete this record">
    Delete
</button>
```

### Popovers
```html
<button type="button" class="btn btn-secondary" data-bs-toggle="popover" 
        title="Employee Info" data-bs-content="Click to view full details">
    View Details
</button>
```

### Alerts (Auto-dismissing)
```html
<div class="alert alert-success" role="alert">
    Record saved successfully!
</div>
```
Auto-dismisses after 5 seconds

## CSS Classes for Enhanced Styling

### Form Validation
```html
<!-- Valid input -->
<input class="form-control is-valid">

<!-- Invalid input -->
<input class="form-control is-invalid">

<!-- Feedback text -->
<span class="invalid-feedback">This field is required</span>
```

### Responsive Design
```html
<div class="row">
    <div class="col-lg-8 col-md-10 col-sm-12">
        <!-- Content -->
    </div>
</div>
```

## Debugging Tips

### Check if Scripts Loaded
```javascript
console.log(typeof AppUtils);  // Should be "object"
console.log(typeof API);       // Should be "object"
console.log(typeof Dashboard); // Should be "object"
console.log(typeof TableUtils); // Should be "object"
```

### Monitor API Calls
```javascript
// Check Network tab in DevTools to see all API requests
// Add console logging in API responses
API.employees.getAll()
    .then(data => {
        console.log('API Response:', data);
    });
```

### Debug Table Filtering
```javascript
// Manually test filter function
const table = document.getElementById('myTable');
TableUtils.filterTable(table, 'searchterm');
```
