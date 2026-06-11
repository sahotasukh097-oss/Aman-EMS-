// Main JavaScript - Global utilities and initialization
(function() {
    'use strict';

    // Initialize on DOM ready
    document.addEventListener('DOMContentLoaded', function() {
        initializeApp();
    });

    function initializeApp() {
        // Initialize tooltips
        initializeTooltips();
        
        // Initialize popovers
        initializePopovers();
        
        // Setup common event listeners
        setupEventListeners();
        
        // Load dashboard data if on dashboard page
        if (document.querySelector('.dashboard-container')) {
            loadDashboardData();
        }
    }

    // Tooltip initialization (Bootstrap)
    function initializeTooltips() {
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }

    // Popover initialization (Bootstrap)
    function initializePopovers() {
        const popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
        popoverTriggerList.map(function (popoverTriggerEl) {
            return new bootstrap.Popover(popoverTriggerEl);
        });
    }

    // Setup common event listeners
    function setupEventListeners() {
        // Auto-hide alerts after 5 seconds
        const alerts = document.querySelectorAll('.alert:not(.alert-permanent)');
        alerts.forEach(function(alert) {
            setTimeout(function() {
                const bsAlert = new bootstrap.Alert(alert);
                bsAlert.close();
            }, 5000);
        });

        // Form submission loader
        const forms = document.querySelectorAll('form');
        forms.forEach(function(form) {
            form.addEventListener('submit', function() {
                const submitBtn = this.querySelector('[type="submit"]');
                if (submitBtn) {
                    submitBtn.disabled = true;
                    submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Loading...';
                }
            });
        });
    }

    // Load dashboard data via AJAX
    function loadDashboardData() {
        fetch('/api/dashboard/stats')
            .then(response => response.json())
            .then(data => {
                updateDashboardStats(data);
            })
            .catch(error => {
                console.error('Error loading dashboard data:', error);
            });
    }

    // Update dashboard statistics
    function updateDashboardStats(data) {
        const elements = {
            totalEmployees: document.getElementById('totalEmployees'),
            presentToday: document.getElementById('presentToday'),
            pendingLeaves: document.getElementById('pendingLeaves'),
            monthlyPayroll: document.getElementById('monthlyPayroll')
        };

        if (elements.totalEmployees) {
            elements.totalEmployees.textContent = data.totalEmployees || '0';
        }
        if (elements.presentToday) {
            elements.presentToday.textContent = data.presentToday || '0';
        }
        if (elements.pendingLeaves) {
            elements.pendingLeaves.textContent = data.pendingLeaves || '0';
        }
        if (elements.monthlyPayroll) {
            elements.monthlyPayroll.textContent = '$' + (data.monthlyPayroll || '0').toLocaleString();
        }
    }

    // Expose utility functions globally
    window.AppUtils = {
        showNotification: function(message, type = 'info') {
            const alertDiv = document.createElement('div');
            alertDiv.className = `alert alert-${type} alert-dismissible fade show`;
            alertDiv.role = 'alert';
            alertDiv.innerHTML = `
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
            `;
            
            const container = document.querySelector('body');
            if (container) {
                container.insertAdjacentElement('afterbegin', alertDiv);
                setTimeout(() => {
                    const bsAlert = new bootstrap.Alert(alertDiv);
                    bsAlert.close();
                }, 5000);
            }
        },

        formatCurrency: function(value) {
            return new Intl.NumberFormat('en-US', {
                style: 'currency',
                currency: 'USD'
            }).format(value);
        },

        formatDate: function(dateString) {
            const options = { year: 'numeric', month: 'long', day: 'numeric' };
            return new Date(dateString).toLocaleDateString('en-US', options);
        },

        debounce: function(func, wait) {
            let timeout;
            return function executedFunction(...args) {
                const later = () => {
                    clearTimeout(timeout);
                    func(...args);
                };
                clearTimeout(timeout);
                timeout = setTimeout(later, wait);
            };
        }
    };
})();
