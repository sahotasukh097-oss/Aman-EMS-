// Dashboard functionality and interactions
(function() {
    'use strict';

    document.addEventListener('DOMContentLoaded', function() {
        if (document.querySelector('.dashboard-container')) {
            initializeDashboard();
        }
    });

    function initializeDashboard() {
        setupStatCardInteractions();
        setupChartData();
        loadEmployeeData();
        setupRefreshButton();
    }

    function setupStatCardInteractions() {
        const statCards = document.querySelectorAll('.stat-card');
        statCards.forEach(card => {
            card.addEventListener('mouseenter', function() {
                this.style.transform = 'translateY(-5px)';
                this.style.boxShadow = '0 8px 20px rgba(0,0,0,0.15)';
            });

            card.addEventListener('mouseleave', function() {
                this.style.transform = 'translateY(0)';
                this.style.boxShadow = '';
            });

            card.style.transition = 'all 0.3s ease';
            card.style.cursor = 'pointer';
        });
    }

    function setupChartData() {
        // Monthly attendance chart
        const attendanceCtx = document.getElementById('attendanceChart');
        if (attendanceCtx) {
            new Chart(attendanceCtx, {
                type: 'line',
                data: {
                    labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
                    datasets: [{
                        label: 'Attendance %',
                        data: [85, 88, 90, 87, 92, 89],
                        borderColor: '#007bff',
                        backgroundColor: 'rgba(0, 123, 255, 0.1)',
                        tension: 0.4
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: true,
                    plugins: {
                        legend: {
                            display: true,
                            position: 'top'
                        }
                    },
                    scales: {
                        y: {
                            beginAtZero: true,
                            max: 100
                        }
                    }
                }
            });
        }

        // Department distribution pie chart
        const deptCtx = document.getElementById('departmentChart');
        if (deptCtx) {
            new Chart(deptCtx, {
                type: 'doughnut',
                data: {
                    labels: ['HR', 'IT', 'Finance', 'Operations', 'Sales'],
                    datasets: [{
                        data: [25, 40, 15, 12, 8],
                        backgroundColor: [
                            '#007bff',
                            '#28a745',
                            '#ffc107',
                            '#dc3545',
                            '#17a2b8'
                        ]
                    }]
                },
                options: {
                    responsive: true,
                    plugins: {
                        legend: {
                            position: 'bottom'
                        }
                    }
                }
            });
        }
    }

    function loadEmployeeData() {
        fetch('/api/employees')
            .then(response => response.json())
            .then(data => {
                renderEmployeeTable(data);
            })
            .catch(error => {
                console.error('Error loading employees:', error);
                AppUtils.showNotification('Failed to load employee data', 'danger');
            });
    }

    function renderEmployeeTable(employees) {
        const tableBody = document.querySelector('.employees-table tbody');
        if (!tableBody) return;

        tableBody.innerHTML = '';
        employees.forEach(employee => {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${employee.id}</td>
                <td>${employee.name}</td>
                <td>${employee.email}</td>
                <td>${employee.department}</td>
                <td>
                    <span class="badge bg-success">${employee.status}</span>
                </td>
                <td>
                    <button class="btn btn-sm btn-info" onclick="viewEmployee(${employee.id})">View</button>
                    <button class="btn btn-sm btn-warning" onclick="editEmployee(${employee.id})">Edit</button>
                </td>
            `;
            tableBody.appendChild(row);
        });
    }

    function setupRefreshButton() {
        const refreshBtn = document.querySelector('[data-action="refresh-dashboard"]');
        if (refreshBtn) {
            refreshBtn.addEventListener('click', function() {
                this.innerHTML = '<span class="spinner-border spinner-border-sm"></span>';
                setTimeout(() => {
                    location.reload();
                }, 500);
            });
        }
    }

    // Expose functions globally
    window.Dashboard = {
        loadEmployeeData: loadEmployeeData,
        setupChartData: setupChartData
    };
})();
