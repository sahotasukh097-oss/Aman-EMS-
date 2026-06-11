// Dashboard Charts Initialization
let charts = {};

document.addEventListener('DOMContentLoaded', function() {
    initializeDashboard();
});

async function initializeDashboard() {
    // Set current date
    updateCurrentDate();

    // Load all dashboard data
    await loadDashboardStats();

    // Initialize charts
    initializeDashboardCharts();
}

function updateCurrentDate() {
    const dateElement = document.getElementById('currentDate');
    if (dateElement) {
        const today = new Date();
        const options = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
        dateElement.textContent = today.toLocaleDateString('en-IN', options);
    }
}

async function loadDashboardStats() {
    try {
        const response = await fetch('/api/dashboard/stats');
        if (response.ok) {
            const stats = await response.json();
            updateStatCards(stats);
            loadActivityData();
        } else {
            console.error('Failed to load stats:', response.status);
        }
    } catch (error) {
        console.error('Error loading dashboard stats:', error);
    }
}

function updateStatCards(stats) {
    // Update Total Employees
    const totalEmpsElement = document.getElementById('totalEmployees');
    if (totalEmpsElement) {
        totalEmpsElement.innerHTML = `<span class="stat-value">${stats.totalEmployees || 0}</span>`;
    }

    // Update Present Today with percentage
    const presentElement = document.getElementById('presentToday');
    const attendancePercLabel = document.getElementById('attendancePercLabel');
    if (presentElement && stats.totalEmployees > 0) {
        const percentage = stats.attendancePercentage || 0;
        presentElement.innerHTML = `<span class="stat-value">${stats.presentToday || 0}</span>`;
        if (attendancePercLabel) {
            attendancePercLabel.textContent = `${Math.round(percentage)}% attendance`;
        }
    } else if (presentElement) {
        presentElement.innerHTML = `<span class="stat-value">0</span>`;
    }

    // Update Pending Leaves
    const leavesElement = document.getElementById('pendingLeaves');
    if (leavesElement) {
        leavesElement.innerHTML = `<span class="stat-value">${stats.pendingLeaves || 0}</span>`;
    }

    // Update Monthly Payroll
    const payrollElement = document.getElementById('monthlyPayroll');
    if (payrollElement) {
        const payrollAmount = stats.monthlyPayroll || 0;
        const formattedAmount = formatCurrency(payrollAmount);
        payrollElement.innerHTML = `<span class="stat-value">${formattedAmount}</span>`;
    }
}

function formatCurrency(amount) {
    if (amount === 0) return '₹0';
    if (amount >= 10000000) {
        return '₹' + (amount / 10000000).toFixed(1) + 'Cr';
    } else if (amount >= 100000) {
        return '₹' + (amount / 100000).toFixed(1) + 'L';
    }
    return '₹' + amount.toLocaleString('en-IN');
}

async function loadActivityData() {
    try {
        const response = await fetch('/api/dashboard/recent-employees?limit=3');
        if (response.ok) {
            const employees = await response.json();
            updateActivityList(employees);
        }
    } catch (error) {
        console.error('Error loading activity data:', error);
    }
}

function updateActivityList(employees) {
    const activityList = document.getElementById('activityList');
    if (!activityList) return;

    if (employees.length === 0) {
        activityList.innerHTML = '<div class="activity-empty"><p>No recent activity</p></div>';
        return;
    }

    const activityIcons = [
        { icon: 'fa-user-tie', class: 'new-hires', text: 'New hire' },
        { icon: 'fa-briefcase', class: 'attendance-update', text: 'Employee' },
        { icon: 'fa-star', class: 'payroll-gen', text: 'Team member' }
    ];

    let html = '';
    employees.forEach((emp, index) => {
        const iconData = activityIcons[index % activityIcons.length];
        html += `
            <div class="activity-item">
                <div class="activity-icon ${iconData.class}">
                    <i class="fas ${iconData.icon}"></i>
                </div>
                <div class="activity-content">
                    <h4>${emp.name}</h4>
                    <p>${emp.department} • ${emp.designation}</p>
                </div>
            </div>
        `;
    });

    activityList.innerHTML = html;
}

function initializeDashboardCharts() {
    initializeDepartmentChart();
    initializeAttendanceChart();
    initializeSalaryChart();
}

async function initializeDepartmentChart() {
    const ctx = document.getElementById('departmentChart');
    if (!ctx) return;

    try {
        const response = await fetch('/api/dashboard/department-distribution');
        if (response.ok) {
            const data = await response.json();

            const colors = [
                '#FF6B6B', '#4ECDC4', '#FFE66D', '#95E1D3', 
                '#C7CEEA', '#FF9999', '#66C2A5', '#FFB3BA',
                '#FFDFBA', '#FFFFBA', '#BAFFC9', '#BAE1FF'
            ];

            charts.department = new Chart(ctx, {
                type: 'doughnut',
                data: {
                    labels: data.departments || [],
                    datasets: [{
                        data: data.counts || [],
                        backgroundColor: colors.slice(0, (data.departments || []).length),
                        borderColor: '#0f172a',
                        borderWidth: 2
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: true,
                    plugins: {
                        legend: {
                            display: true,
                            position: 'bottom',
                            labels: {
                                color: '#cbd5e1',
                                padding: 15,
                                font: { size: 12 },
                                usePointStyle: true
                            }
                        },
                        tooltip: {
                            backgroundColor: 'rgba(0, 0, 0, 0.8)',
                            padding: 12,
                            titleColor: '#f8fafc',
                            bodyColor: '#cbd5e1',
                            callbacks: {
                                label: function(context) {
                                    return context.label + ': ' + context.parsed + ' employees';
                                }
                            }
                        }
                    },
                    cutout: '60%'
                }
            });
        }
    } catch (error) {
        console.error('Error initializing department chart:', error);
    }
}

async function initializeAttendanceChart() {
    const ctx = document.getElementById('attendanceChart');
    if (!ctx) return;

    try {
        const response = await fetch('/api/dashboard/monthly-attendance');
        if (response.ok) {
            const data = await response.json();
            const months = data.data.map(d => d.month) || [];
            const percentages = data.data.map(d => d.percentage) || [];

            charts.attendance = new Chart(ctx, {
                type: 'line',
                data: {
                    labels: months,
                    datasets: [{
                        label: 'Attendance %',
                        data: percentages,
                        borderColor: '#10b981',
                        backgroundColor: 'rgba(16, 185, 129, 0.1)',
                        borderWidth: 3,
                        fill: true,
                        tension: 0.4,
                        pointBackgroundColor: '#10b981',
                        pointBorderColor: '#fff',
                        pointBorderWidth: 2,
                        pointRadius: 6,
                        pointHoverRadius: 8
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            display: true,
                            labels: {
                                color: '#cbd5e1',
                                padding: 20,
                                usePointStyle: true,
                                font: { size: 12 }
                            }
                        },
                        tooltip: {
                            backgroundColor: 'rgba(0, 0, 0, 0.8)',
                            padding: 12,
                            titleColor: '#f8fafc',
                            bodyColor: '#cbd5e1',
                            callbacks: {
                                label: function(context) {
                                    return context.dataset.label + ': ' + context.parsed.y.toFixed(1) + '%';
                                }
                            }
                        }
                    },
                    scales: {
                        y: {
                            beginAtZero: true,
                            max: 100,
                            grid: {
                                color: 'rgba(71, 85, 105, 0.1)',
                                drawBorder: false
                            },
                            ticks: {
                                color: '#94a3b8',
                                font: { size: 11 },
                                callback: function(value) {
                                    return value + '%';
                                }
                            }
                        },
                        x: {
                            grid: {
                                display: false,
                                drawBorder: false
                            },
                            ticks: {
                                color: '#94a3b8',
                                font: { size: 11 }
                            }
                        }
                    }
                }
            });
        }
    } catch (error) {
        console.error('Error initializing attendance chart:', error);
    }
}

function initializeSalaryChart() {
    const ctx = document.getElementById('salaryChart');
    if (!ctx) return;

    charts.salary = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: ['₹3-4L', '₹4-5L', '₹5-6L', '₹6-7L', '₹7-8L', '₹8L+'],
            datasets: [{
                label: 'Number of Employees',
                data: [8, 12, 18, 15, 10, 2],
                backgroundColor: [
                    '#FF6B6B',
                    '#FFB66B',
                    '#FFE66D',
                    '#B7D791',
                    '#7FD791',
                    '#5DB7D7'
                ],
                borderRadius: 8,
                borderSkipped: false,
                borderColor: 'transparent'
            }]
        },
        options: {
            indexAxis: 'y',
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: false
                },
                tooltip: {
                    backgroundColor: 'rgba(0, 0, 0, 0.8)',
                    padding: 12,
                    titleColor: '#f8fafc',
                    bodyColor: '#cbd5e1',
                    callbacks: {
                        label: function(context) {
                            return 'Employees: ' + context.parsed.x;
                        }
                    }
                }
            },
            scales: {
                x: {
                    beginAtZero: true,
                    grid: {
                        color: 'rgba(71, 85, 105, 0.1)',
                        drawBorder: false
                    },
                    ticks: {
                        color: '#94a3b8',
                        font: { size: 11 }
                    }
                },
                y: {
                    grid: {
                        display: false,
                        drawBorder: false
                    },
                    ticks: {
                        color: '#94a3b8',
                        font: { size: 11 }
                    }
                }
            }
        }
    });
}
