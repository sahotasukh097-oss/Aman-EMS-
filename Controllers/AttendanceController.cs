using SAPS_EMS_.Data;
using SAPS_EMS_.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SAPS_EMS_.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AttendanceController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<AttendanceController> _logger;

        public AttendanceController(ApplicationDbContext dbContext, ILogger<AttendanceController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // GET: api/attendance?date=2025-01-15
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAttendanceByDate([FromQuery] DateTime? date = null)
        {
            try
            {
                var targetDate = date ?? DateTime.Now;

                var attendance = await _dbContext.Attendances
                    .Include(a => a.Employee)
                    .Where(a => a.AttendanceDate.Date == targetDate.Date)
                    .Select(a => new
                    {
                        id = a.Id,
                        employeeId = a.EmployeeId,
                        employeeName = a.Employee.FullName,
                        date = a.AttendanceDate,
                        status = a.Status,
                        checkIn = a.CheckIn,
                        checkOut = a.CheckOut
                    })
                    .ToListAsync();

                return Ok(attendance);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching attendance: {ex.Message}");
                return StatusCode(500, new { message = "Error fetching attendance", error = ex.Message });
            }
        }

        // POST: api/attendance
        [HttpPost]
        public async Task<ActionResult<object>> MarkAttendance([FromBody] MarkAttendanceDto markDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Check if employee exists
                var employee = await _dbContext.Employees.FindAsync(markDto.EmployeeId);
                if (employee == null)
                    return NotFound(new { message = "Employee not found" });

                // Check if attendance already marked for today
                var today = DateTime.Now.Date;
                var existingAttendance = await _dbContext.Attendances
                    .FirstOrDefaultAsync(a => a.EmployeeId == markDto.EmployeeId && a.AttendanceDate.Date == today);

                if (existingAttendance != null)
                {
                    // Update existing attendance
                    if (markDto.Status?.ToLower() == "check-out" || markDto.Status?.ToLower() == "checkout")
                    {
                        existingAttendance.CheckOut = TimeOnly.FromDateTime(DateTime.Now);
                        existingAttendance.Status = "present";
                    }
                    else
                    {
                        existingAttendance.CheckIn = TimeOnly.FromDateTime(DateTime.Now);
                        existingAttendance.Status = markDto.Status ?? "present";
                    }
                    _dbContext.Attendances.Update(existingAttendance);
                }
                else
                {
                    // Create new attendance record
                    var attendance = new Attendance
                    {
                        EmployeeId = markDto.EmployeeId,
                        AttendanceDate = DateTime.Now,
                        Status = markDto.Status ?? "present",
                        CheckIn = markDto.Status?.ToLower() == "check-out" ? null : TimeOnly.FromDateTime(DateTime.Now),
                        CheckOut = markDto.Status?.ToLower() == "check-out" ? TimeOnly.FromDateTime(DateTime.Now) : null
                    };
                    _dbContext.Attendances.Add(attendance);
                }

                await _dbContext.SaveChangesAsync();

                return Ok(new { message = "Attendance marked successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error marking attendance: {ex.Message}");
                return StatusCode(500, new { message = "Error marking attendance", error = ex.Message });
            }
        }

        // GET: api/attendance/stats
        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetAttendanceStats()
        {
            try
            {
                var today = DateTime.Now.Date;
                var totalEmployees = await _dbContext.Employees.CountAsync();
                var presentToday = await _dbContext.Attendances
                    .Where(a => a.AttendanceDate.Date == today && a.Status == "present")
                    .CountAsync();

                var stats = new
                {
                    totalEmployees,
                    presentToday,
                    absentToday = totalEmployees - presentToday,
                    attendancePercentage = totalEmployees > 0 ? (presentToday * 100 / totalEmployees) : 0
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching attendance stats: {ex.Message}");
                return StatusCode(500, new { message = "Error fetching attendance stats", error = ex.Message });
            }
        }
    }

    // DTOs
    public class MarkAttendanceDto
    {
        public int EmployeeId { get; set; }
        public string Status { get; set; } // "present", "absent", "leave"
        public DateTime? Date { get; set; }
    }
}
