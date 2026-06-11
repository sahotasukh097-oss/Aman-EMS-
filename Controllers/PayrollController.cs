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
    public class PayrollController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<PayrollController> _logger;

        public PayrollController(ApplicationDbContext dbContext, ILogger<PayrollController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // GET: api/payroll?month=January&year=2025
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetPayrollByMonth(
            [FromQuery] string month = "", 
            [FromQuery] int year = 0)
        {
            try
            {
                if (string.IsNullOrEmpty(month))
                    month = DateTime.Now.ToString("MMMM");
                if (year == 0)
                    year = DateTime.Now.Year;

                var payrolls = await _dbContext.Payrolls
                    .Include(p => p.Employee)
                    .ThenInclude(e => e.User)
                    .Where(p => p.SalaryMonth == month && p.GeneratedAt.Year == year)
                    .Select(p => new
                    {
                        id = p.Id,
                        employeeId = p.EmployeeId,
                        employeeName = p.Employee.FullName,
                        basicSalary = p.BasicSalary,
                        allowances = p.Allowances,
                        deductions = p.Deductions,
                        netSalary = p.NetSalary,
                        month = p.SalaryMonth,
                        year = p.GeneratedAt.Year,
                        generatedDate = p.GeneratedAt
                    })
                    .ToListAsync();

                return Ok(payrolls);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching payroll: {ex.Message}");
                return StatusCode(500, new { message = "Error fetching payroll", error = ex.Message });
            }
        }

        // GET: api/payroll/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetPayrollById(int id)
        {
            try
            {
                var payroll = await _dbContext.Payrolls
                    .Include(p => p.Employee)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (payroll == null)
                    return NotFound(new { message = "Payroll record not found" });

                return Ok(new
                {
                    id = payroll.Id,
                    employeeId = payroll.EmployeeId,
                    employeeName = payroll.Employee.FullName,
                    basicSalary = payroll.BasicSalary,
                    allowances = payroll.Allowances,
                    deductions = payroll.Deductions,
                    netSalary = payroll.NetSalary,
                    month = payroll.SalaryMonth,
                    year = payroll.GeneratedAt.Year,
                    generatedDate = payroll.GeneratedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching payroll: {ex.Message}");
                return StatusCode(500, new { message = "Error fetching payroll", error = ex.Message });
            }
        }

        // POST: api/payroll/generate
        [HttpPost("generate")]
        public async Task<ActionResult<IEnumerable<object>>> GeneratePayroll([FromBody] GeneratePayrollDto generateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var month = generateDto.Month ?? DateTime.Now.ToString("MMMM");
                var year = generateDto.Year ?? DateTime.Now.Year;

                // Get all active employees (Status = 1)
                var employees = await _dbContext.Employees
                    .Where(e => e.Status == 1)
                    .ToListAsync();

                var generatedPayrolls = new List<object>();

                foreach (var employee in employees)
                {
                    // Check if payroll already exists
                    var existingPayroll = await _dbContext.Payrolls
                        .FirstOrDefaultAsync(p => p.EmployeeId == employee.Id && p.SalaryMonth == month && p.GeneratedAt.Year == year);

                    if (existingPayroll == null)
                    {
                        var basicSalary = employee.Salary ?? 0;
                        var allowances = basicSalary * 0.1m; // 10% allowance
                        var deductions = basicSalary * 0.05m; // 5% deduction
                        var netSalary = basicSalary + allowances - deductions;

                        var payroll = new Payroll
                        {
                            EmployeeId = employee.Id,
                            BasicSalary = basicSalary,
                            Allowances = allowances,
                            Deductions = deductions,
                            NetSalary = netSalary,
                            SalaryMonth = month
                        };

                        _dbContext.Payrolls.Add(payroll);

                        generatedPayrolls.Add(new
                        {
                            id = payroll.Id,
                            employeeId = payroll.EmployeeId,
                            employeeName = employee.FullName,
                            basicSalary = payroll.BasicSalary,
                            allowances = payroll.Allowances,
                            deductions = payroll.Deductions,
                            netSalary = payroll.NetSalary,
                            month = payroll.SalaryMonth
                        });
                    }
                }

                await _dbContext.SaveChangesAsync();

                return Ok(generatedPayrolls);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating payroll: {ex.Message}");
                return StatusCode(500, new { message = "Error generating payroll", error = ex.Message });
            }
        }

        // GET: api/payroll/stats
        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetPayrollStats()
        {
            try
            {
                var currentMonth = DateTime.Now.ToString("MMMM");
                var currentYear = DateTime.Now.Year;

                var payrolls = await _dbContext.Payrolls
                    .Where(p => p.SalaryMonth == currentMonth && p.GeneratedAt.Year == currentYear)
                    .ToListAsync();

                var totalPayroll = payrolls.Sum(p => p.NetSalary);
                var averageSalary = payrolls.Count > 0 ? payrolls.Average(p => p.NetSalary) : 0;

                var stats = new
                {
                    totalPayroll,
                    averageSalary,
                    employeesProcessed = payrolls.Count,
                    month = currentMonth,
                    year = currentYear
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching payroll stats: {ex.Message}");
                return StatusCode(500, new { message = "Error fetching payroll stats", error = ex.Message });
            }
        }
    }

    // DTOs
    public class GeneratePayrollDto
    {
        public string Month { get; set; }
        public int? Year { get; set; }
    }
}
