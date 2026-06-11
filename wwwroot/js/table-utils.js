// Table interactions and data management
(function() {
    'use strict';

    document.addEventListener('DOMContentLoaded', function() {
        initializeTableFeatures();
    });

    function initializeTableFeatures() {
        setupTableSorting();
        setupTableSearch();
        setupTablePagination();
        setupRowSelection();
    }

    function setupTableSorting() {
        const tables = document.querySelectorAll('table[data-sortable="true"]');
        tables.forEach(table => {
            const headers = table.querySelectorAll('th[data-sortable]');
            headers.forEach(header => {
                header.style.cursor = 'pointer';
                header.addEventListener('click', function() {
                    sortTable(table, this.cellIndex, this.dataset.sortable);
                });
            });
        });
    }

    function sortTable(table, columnIndex, dataType) {
        const tbody = table.querySelector('tbody');
        const rows = Array.from(tbody.querySelectorAll('tr'));

        rows.sort((a, b) => {
            const aValue = a.cells[columnIndex].textContent.trim();
            const bValue = b.cells[columnIndex].textContent.trim();

            let comparison = 0;

            if (dataType === 'number') {
                comparison = parseFloat(aValue) - parseFloat(bValue);
            } else if (dataType === 'date') {
                comparison = new Date(aValue) - new Date(bValue);
            } else {
                comparison = aValue.localeCompare(bValue);
            }

            return comparison;
        });

        rows.forEach(row => tbody.appendChild(row));
    }

    function setupTableSearch() {
        const searchInputs = document.querySelectorAll('[data-table-search]');
        searchInputs.forEach(input => {
            const tableId = input.dataset.tableSearch;
            const table = document.getElementById(tableId);
            if (table) {
                input.addEventListener('keyup', AppUtils.debounce(function() {
                    filterTable(table, this.value);
                }, 300));
            }
        });
    }

    function filterTable(table, searchValue) {
        const rows = table.querySelectorAll('tbody tr');
        const searchLower = searchValue.toLowerCase();

        rows.forEach(row => {
            const text = row.textContent.toLowerCase();
            row.style.display = text.includes(searchLower) ? '' : 'none';
        });
    }

    function setupTablePagination() {
        const tables = document.querySelectorAll('[data-paginate]');
        tables.forEach(table => {
            const rowsPerPage = parseInt(table.dataset.paginate) || 10;
            paginateTable(table, rowsPerPage);
        });
    }

    function paginateTable(table, rowsPerPage) {
        const tbody = table.querySelector('tbody');
        const rows = tbody.querySelectorAll('tr');
        const pageCount = Math.ceil(rows.length / rowsPerPage);

        // Hide all rows initially
        rows.forEach(row => row.style.display = 'none');

        // Show first page
        for (let i = 0; i < rowsPerPage; i++) {
            if (rows[i]) rows[i].style.display = '';
        }

        // Create pagination controls
        if (pageCount > 1) {
            createPaginationControls(table, pageCount, rowsPerPage, rows);
        }
    }

    function createPaginationControls(table, pageCount, rowsPerPage, rows) {
        let paginationContainer = table.nextElementSibling;
        if (!paginationContainer || !paginationContainer.classList.contains('pagination-controls')) {
            paginationContainer = document.createElement('div');
            paginationContainer.className = 'pagination-controls mt-3 d-flex justify-content-center';
            table.parentNode.insertBefore(paginationContainer, table.nextSibling);
        }

        paginationContainer.innerHTML = '';

        for (let i = 1; i <= pageCount; i++) {
            const btn = document.createElement('button');
            btn.className = 'btn btn-sm btn-outline-primary' + (i === 1 ? ' active' : '');
            btn.textContent = i;
            btn.addEventListener('click', function() {
                showPage(i, rowsPerPage, rows);
                updatePaginationButtons(paginationContainer, i);
            });
            paginationContainer.appendChild(btn);
        }
    }

    function showPage(pageNumber, rowsPerPage, rows) {
        const startIndex = (pageNumber - 1) * rowsPerPage;
        const endIndex = startIndex + rowsPerPage;

        rows.forEach((row, index) => {
            row.style.display = (index >= startIndex && index < endIndex) ? '' : 'none';
        });
    }

    function updatePaginationButtons(container, activePage) {
        const buttons = container.querySelectorAll('button');
        buttons.forEach((btn, index) => {
            btn.classList.toggle('active', index + 1 === activePage);
        });
    }

    function setupRowSelection() {
        const selectAllCheckbox = document.getElementById('selectAll');
        if (selectAllCheckbox) {
            selectAllCheckbox.addEventListener('change', function() {
                const rowCheckboxes = document.querySelectorAll('tbody input[type="checkbox"]');
                rowCheckboxes.forEach(cb => cb.checked = this.checked);
            });
        }
    }

    // Expose functions globally
    window.TableUtils = {
        sortTable: sortTable,
        filterTable: filterTable,
        paginateTable: paginateTable
    };
})();
