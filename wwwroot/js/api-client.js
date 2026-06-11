// API interaction and data fetching
(function() {
    'use strict';

    window.API = {
        // Base URL for API calls
        baseUrl: '/api',

        // Generic fetch wrapper
        async fetch(endpoint, options = {}) {
            const url = `${this.baseUrl}${endpoint}`;
            const defaultOptions = {
                headers: {
                    'Content-Type': 'application/json'
                }
            };

            const config = {
                ...defaultOptions,
                ...options
            };

            try {
                const response = await fetch(url, config);
                
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }

                return await response.json();
            } catch (error) {
                console.error('API Error:', error);
                throw error;
            }
        },

        // Employee endpoints
        employees: {
            getAll: function() {
                return API.fetch('/employees');
            },

            getById: function(id) {
                return API.fetch(`/employees/${id}`);
            },

            create: function(data) {
                return API.fetch('/employees', {
                    method: 'POST',
                    body: JSON.stringify(data)
                });
            },

            update: function(id, data) {
                return API.fetch(`/employees/${id}`, {
                    method: 'PUT',
                    body: JSON.stringify(data)
                });
            },

            delete: function(id) {
                return API.fetch(`/employees/${id}`, {
                    method: 'DELETE'
                });
            }
        },

        // Attendance endpoints
        attendance: {
            getByDate: function(date) {
                return API.fetch(`/attendance?date=${date}`);
            },

            mark: function(employeeId, status) {
                return API.fetch('/attendance', {
                    method: 'POST',
                    body: JSON.stringify({ employeeId, status, date: new Date().toISOString() })
                });
            }
        },

        // Leave endpoints
        leaves: {
            getAll: function() {
                return API.fetch('/leaves');
            },

            request: function(data) {
                return API.fetch('/leaves', {
                    method: 'POST',
                    body: JSON.stringify(data)
                });
            },

            approve: function(leaveId) {
                return API.fetch(`/leaves/${leaveId}/approve`, {
                    method: 'POST'
                });
            },

            reject: function(leaveId) {
                return API.fetch(`/leaves/${leaveId}/reject`, {
                    method: 'POST'
                });
            }
        },

        // Payroll endpoints
        payroll: {
            getByMonth: function(month, year) {
                return API.fetch(`/payroll?month=${month}&year=${year}`);
            },

            generate: function(data) {
                return API.fetch('/payroll/generate', {
                    method: 'POST',
                    body: JSON.stringify(data)
                });
            }
        },

        // Dashboard endpoints
        dashboard: {
            getStats: function() {
                return API.fetch('/dashboard/stats');
            }
        }
    };
})();
