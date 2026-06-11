using SAPS_EMS_.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SAPS_EMS_.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(ApplicationDbContext dbContext, ILogger<DashboardController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // GET: api/dashboard/stats
        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetDashboardStats()
        {
            try
            {
                var today = DateTime.Now.Date;

                // Get total employees
                var totalEmployees = await _dbContext.Employees.CountAsync();

                // Get present today
                var presentToday = await _dbContext.Attendances
                    .Where(a => a.AttendanceDate.Date == today && a.Status == "present")
                    .CountAsync();

                // Get pending leaves
                var pendingLeaves = await _dbContext.Leaves
                    .Where(l => l.Status == "pending")
                    .CountAsync();

                // Get monthly payroll
                var currentMonth = DateTime.Now.ToString("MMMM");
                var currentYear = DateTime.Now.Year;
                var monthlyPayroll = await _dbContext.Payrolls
                    .Where(p => p.SalaryMonth == currentMonth && p.GeneratedAt.Year == currentYear)
                    .SumAsync(p => p.NetSalary);

                var stats = new
                {
                    totalEmployees,
                    presentToday,
                    pendingLeaves,
                    monthlyPayroll,
                    attendancePercentage = totalEmployees > 0 ? (presentToday * 100 / totalEmployees) : 0
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching dashboard stats: {ex.Message}");
                return StatusCode(500, new { message = "Error fetching dashboard stats", error = ex.Message });
            }
        }

        // GET: api/dashboard/monthly-attendance
        [HttpGet("monthly-attendance")]
        public async Task<ActionResult<object>> GetMonthlyAttendance()
        {
            try
            {
                var today = DateTime.Now;
                var lastSixMonths = Enumerable.Range(0, 6)
                    .Select(i => today.AddMonths(-i))
                    .OrderBy(d => d)
                    .ToList();

                var attendanceData = new List<object>();

                foreach (var month in lastSixMonths)
                {
                    var monthStart = new DateTime(month.Year, month.Month, 1);
                    var monthEnd = monthStart.AddMonths(1).AddDays(-1);
                    var workDays = 20; // Approximate work days per month

                    var attendedDays = await _dbContext.Attendances
                        .Where(a => a.AttendanceDate >= monthStart && a.AttendanceDate <= monthEnd && a.Status == "present")
                        .CountAsync();

                    var percentage = workDays > 0 ? (attendedDays * 100 / workDays) : 0;

                    attendanceData.Add(new
                    {
                        month = monthStart.ToString("MMM"),
                        percentage = percentage
                    });
                }

                return Ok(new { data = attendanceData });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching monthly attendance: {ex.Message}");
                return StatusCode(500, new { message = "Error fetching monthly attendance", error = ex.Message });
            }
        }

        // GET: api/dashboard/department-distribution
        [HttpGet("department-distribution")]
        public async Task<ActionResult<object>> GetDepartmentDistribution()
        {
            try
            {
                var departments = await _dbContext.Employees
                    .Where(e => !string.IsNullOrEmpty(e.Department))
                    .GroupBy(e => e.Department)
                    .Select(g => new
                    {
                        name = g.Key,
                        count = g.Count()
                    })
                    .ToListAsync();

                return Ok(new
                {
                    departments = departments.Select(d => d.name).ToList(),
                    counts = departments.Select(d => d.count).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching department distribution: {ex.Message}");
                return StatusCode(500, new { message = "Error fetching department distribution", error = ex.Message });
            }
        }

        // GET: api/dashboard/recent-employees
        [HttpGet("recent-employees")]
        public async Task<ActionResult<IEnumerable<object>>> GetRecentEmployees([FromQuery] int limit = 5)
        {
            try
            {
                var employees = await _dbContext.Employees
                    .Include(e => e.User)
                    .OrderByDescending(e => e.CreatedAt)
                    .Take(limit)
                    .Select(e => new
                    {
                        id = e.Id,
                        name = e.FullName,
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
                _logger.LogError($"Error fetching recent employees: {ex.Message}");
                return StatusCode(500, new { message = "Error fetching recent employees", error = ex.Message });
            }
        }
    }
}
