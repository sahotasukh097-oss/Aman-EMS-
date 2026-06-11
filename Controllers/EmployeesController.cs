using Aman_EMS_.Data;
using Aman_EMS_.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aman_EMS_.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(ApplicationDbContext dbContext, ILogger<EmployeesController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // GET: api/employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAllEmployees()
        {
            try
            {
                var employees = await _dbContext.Employees
                    .Select(e => new
                    {
                        id = e.Id,
                        name = e.FullName,
                        firstName = e.FirstName,
                        lastName = e.LastName,
                        email = e.User.Email,
                        department = e.Department,
                        designation = e.Designation,
                        salary = e.Salary,
                        status = e.Status,
                        dateOfJoining = e.DateOfJoining
                    })
                    .ToListAsync();

                return Ok(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching employees: {ex.Message}");
                return StatusCode(500, new { message = "Error fetching employees", error = ex.Message });
            }
        }

        // GET: api/employees/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetEmployeeById(int id)
        {
            try
            {
                var employee = await _dbContext.Employees
                    .Include(e => e.User)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (employee == null)
                    return NotFound(new { message = "Employee not found" });

                return Ok(new
                {
                    id = employee.Id,
                    name = employee.FullName,
                    firstName = employee.FirstName,
                    lastName = employee.LastName,
                    phone = employee.Phone,
                    email = employee.User?.Email,
                    department = employee.Department,
                    designation = employee.Designation,
                    salary = employee.Salary,
                    status = employee.Status,
                    dateOfJoining = employee.DateOfJoining
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching employee: {ex.Message}");
                return StatusCode(500, new { message = "Error fetching employee", error = ex.Message });
            }
        }

        // POST: api/employees
        [HttpPost]
        public async Task<ActionResult<object>> CreateEmployee([FromBody] CreateEmployeeDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var employee = new Employee
                {
                    FirstName = createDto.FirstName,
                    LastName = createDto.LastName,
                    Phone = createDto.Phone,
                    Department = createDto.Department,
                    Designation = createDto.Designation,
                    Salary = createDto.Salary,
                    DateOfJoining = DateTime.Now,
                    Status = 1,
                    UserId = 1 // This should be set appropriately from context
                };

                _dbContext.Employees.Add(employee);
                await _dbContext.SaveChangesAsync();

                return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.Id }, new
                {
                    id = employee.Id,
                    name = employee.FullName,
                    message = "Employee created successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating employee: {ex.Message}");
                return StatusCode(500, new { message = "Error creating employee", error = ex.Message });
            }
        }

        // PUT: api/employees/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeDto updateDto)
        {
            try
            {
                var employee = await _dbContext.Employees.FindAsync(id);
                if (employee == null)
                    return NotFound(new { message = "Employee not found" });

                if (!string.IsNullOrEmpty(updateDto.FirstName))
                    employee.FirstName = updateDto.FirstName;
                if (!string.IsNullOrEmpty(updateDto.LastName))
                    employee.LastName = updateDto.LastName;
                if (!string.IsNullOrEmpty(updateDto.Phone))
                    employee.Phone = updateDto.Phone;
                if (!string.IsNullOrEmpty(updateDto.Department))
                    employee.Department = updateDto.Department;
                if (!string.IsNullOrEmpty(updateDto.Designation))
                    employee.Designation = updateDto.Designation;
                if (updateDto.Salary.HasValue)
                    employee.Salary = updateDto.Salary.Value;
                if (updateDto.Status.HasValue)
                    employee.Status = updateDto.Status.Value;

                _dbContext.Employees.Update(employee);
                await _dbContext.SaveChangesAsync();

                return Ok(new { message = "Employee updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating employee: {ex.Message}");
                return StatusCode(500, new { message = "Error updating employee", error = ex.Message });
            }
        }

        // DELETE: api/employees/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                var employee = await _dbContext.Employees.FindAsync(id);
                if (employee == null)
                    return NotFound(new { message = "Employee not found" });

                _dbContext.Employees.Remove(employee);
                await _dbContext.SaveChangesAsync();

                return Ok(new { message = "Employee deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting employee: {ex.Message}");
                return StatusCode(500, new { message = "Error deleting employee", error = ex.Message });
            }
        }
    }

    // DTOs
    public class CreateEmployeeDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public decimal Salary { get; set; }
    }

    public class UpdateEmployeeDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public decimal? Salary { get; set; }
        public int? Status { get; set; }
    }
}
