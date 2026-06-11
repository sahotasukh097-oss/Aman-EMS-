# JavaScript Implementation Guide - AMAN HR Management System

## Overview
This document describes the JavaScript files added to enhance the AMAN HR Management System with interactive features, form validation, data handling, and dashboard functionality.

## JavaScript Files Structure

### 1. **main.js** - Core Application Initialization
**Location:** `wwwroot/js/main.js`

**Purpose:** Initialize the application on page load and provide global utility functions.

**Features:**
- Auto-initialization of Bootstrap components (tooltips, popovers)
- Auto-dismissing alerts after 5 seconds
- Form submission button disabling with loading state
- Dashboard data loading
- Global utility functions exposed via `AppUtils`

**Global Utility Methods:**
```javascript
AppUtils.showNotification(message, type)  // Show dismissible alerts
AppUtils.formatCurrency(value)            // Format numbers as currency
AppUtils.formatDate(dateString)           // Format dates consistently
AppUtils.debounce(func, wait)             // Debounce function calls
```

### 2. **form-validation.js** - Form Handling and Validation
**Location:** `wwwroot/js/form-validation.js`

**Purpose:** Handle client-side form validation and user feedback.

**Features:**
- Bootstrap form validation
- Email validation with live feedback
- Password strength indicator with visual feedback
- Custom error message display
- Form-specific handlers (Login, Signup)
- Password confirmation matching

**Supported Form Classes:**
- `.login-form` - Handles login form submission
- `.signup-form` - Handles signup form submission
- `.needs-validation` - Bootstrap validation

**Password Strength Levels:**
- Weak (Red): 0-1 criteria met
- Fair (Yellow): 2 criteria met
- Good (Blue): 3 criteria met
- Strong (Green): All 4 criteria met

### 3. **dashboard.js** - Dashboard Interactions
**Location:** `wwwroot/js/dashboard.js`

**Purpose:** Provide dashboard-specific features and data visualization.

**Features:**
- Stat card hover animations
- Chart.js integration for data visualization
- Employee data loading and rendering
- Dashboard refresh functionality
- Real-time data updates

**Chart Types:**
- Line chart for monthly attendance trends
- Doughnut chart for department distribution

**Global Methods:**
```javascript
Dashboard.loadEmployeeData()  // Fetch and display employee data
Dashboard.setupChartData()    // Initialize charts
```

### 4. **table-utils.js** - Table Management
**Location:** `wwwroot/js/table-utils.js`

**Purpose:** Enhance tables with sorting, filtering, and pagination.

**Features:**
- Column sorting (text, number, date)
- Real-time search filtering with debouncing
- Pagination with page controls
- Row selection with "Select All" checkbox
- Dynamic pagination button generation

**Usage:**
```html
<!-- For sortable table -->
<table data-sortable="true">
    <th data-sortable="text">Name</th>
    <th data-sortable="number">ID</th>
    <th data-sortable="date">Date</th>
</table>

<!-- For searchable table -->
<input type="text" data-table-search="tableId" placeholder="Search...">
<table id="tableId"></table>

<!-- For paginated table -->
<table data-paginate="10"></table>
```

**Global Methods:**
```javascript
TableUtils.sortTable(table, columnIndex, dataType)
TableUtils.filterTable(table, searchValue)
TableUtils.paginateTable(table, rowsPerPage)
```

### 5. **api-client.js** - API Integration
**Location:** `wwwroot/js/api-client.js`

**Purpose:** Centralized API communication with the backend.

**Features:**
- Generic fetch wrapper with error handling
- RESTful API endpoint organization
- Automatic JSON serialization/deserialization
- Organized API endpoints by resource

**API Endpoints:**
```javascript
// Employees
API.employees.getAll()
API.employees.getById(id)
API.employees.create(data)
API.employees.update(id, data)
API.employees.delete(id)

// Attendance
API.attendance.getByDate(date)
API.attendance.mark(employeeId, status)

// Leaves
API.leaves.getAll()
API.leaves.request(data)
API.leaves.approve(leaveId)
API.leaves.reject(leaveId)

// Payroll
API.payroll.getByMonth(month, year)
API.payroll.generate(data)

// Dashboard
API.dashboard.getStats()
```

## Integration Points

### In _Layout.cshtml
All scripts are loaded in the correct order for proper dependency management:

```html
<script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/3.9.1/chart.min.js"></script>
<script src="~/js/main.js"></script>
<script src="~/js/api-client.js"></script>
<script src="~/js/form-validation.js"></script>
<script src="~/js/table-utils.js"></script>
<script src="~/js/dashboard.js"></script>
```

### In Razor Pages

**Login Page (Login.cshtml):**
- Automatic email validation
- Password strength indicator
- Form submission with loading state

**Signup Page (Signup.cshtml):**
- Email validation
- Password strength indicator
- Confirm password matching

**Dashboard Page (Dashboard.cshtml):**
- Stat card animations
- Chart.js data visualization
- Employee table with search/sort/pagination

## External Dependencies

- **Bootstrap 5** - CSS framework and JavaScript components
- **Chart.js 3.9.1** - Data visualization library
- **Font Awesome 6.4.0** - Icon library

## Usage Examples

### Show Notification
```javascript
AppUtils.showNotification('Operation completed successfully', 'success');
AppUtils.showNotification('An error occurred', 'danger');
```

### Fetch Employee Data
```javascript
API.employees.getAll()
    .then(data => {
        console.log('Employees:', data);
    })
    .catch(error => {
        AppUtils.showNotification('Failed to load employees', 'danger');
    });
```

### Format Data
```javascript
const currency = AppUtils.formatCurrency(50000);  // "$50,000.00"
const date = AppUtils.formatDate('2025-01-15');    // "January 15, 2025"
```

### Create New Employee
```javascript
API.employees.create({
    name: 'John Doe',
    email: 'john@example.com',
    department: 'IT'
})
.then(result => {
    AppUtils.showNotification('Employee created successfully', 'success');
})
.catch(error => {
    AppUtils.showNotification('Failed to create employee', 'danger');
});
```

## Browser Compatibility

- Chrome/Edge (latest)
- Firefox (latest)
- Safari 12+
- Requires ES6 support

## Performance Considerations

- Scripts are loaded asynchronously where possible
- Debouncing is applied to search and input events
- CSS transitions use GPU acceleration
- Event delegation is used for dynamic content
- Efficient DOM manipulation patterns

## Debugging

To debug JavaScript issues:

1. Open browser Developer Tools (F12)
2. Check Console tab for errors
3. Use Network tab to monitor API calls
4. Check Elements tab for DOM structure
5. Use Application tab to view local storage/session storage

## Future Enhancements

- Add more chart types (bar, radar, scatter)
- Implement real-time updates with WebSockets
- Add offline support with Service Workers
- Implement advanced filtering options
- Add export to PDF/Excel functionality
- Add drag-and-drop file upload
