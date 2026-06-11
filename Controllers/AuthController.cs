using SAPS_EMS_.Data;
using SAPS_EMS_.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace SAPS_EMS_.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Dashboard");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Email and password are required");
                return View(model);
            }

            var email = model.Email?.Trim().ToLower() ?? string.Empty;
            var password = model.Password?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(string.Empty, "Email and password are required");
                return View(model);
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => (u.Email != null && u.Email.ToLower() == email) ||
                                          (u.Username != null && u.Username.ToLower() == email));

            if (user != null && VerifyPassword(password, user.PasswordHash))
            {
                // Create claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim("UserId", user.Id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Dashboard");
            }

            ModelState.AddModelError(string.Empty, "Invalid email or password");
            return View(model);
        }

        [HttpGet]
        public IActionResult Signup()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Dashboard");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Signup(SignupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var fullName = model.FullName?.Trim() ?? string.Empty;
            var email = model.Email?.Trim().ToLower() ?? string.Empty;
            var password = model.Password?.Trim() ?? string.Empty;
            var confirmPassword = model.ConfirmPassword?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                ModelState.AddModelError(string.Empty, "All fields are required");
                return View(model);
            }

            if (password != confirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Passwords do not match");
                return View(model);
            }

            if (password.Length < 6)
            {
                ModelState.AddModelError(string.Empty, "Password must be at least 6 characters long");
                return View(model);
            }

            if (await _context.Users.AnyAsync(u => u.Email == email))
            {
                ModelState.AddModelError(string.Empty, "Email already registered. Please login or use a different email");
                return View(model);
            }

            try
            {
                var user = new User
                {
                    Username = email,
                    Email = email,
                    FullName = fullName,
                    Status = 1,
                    CreatedAt = DateTime.UtcNow
                };

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Sign in the user
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim("UserId", user.Id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                TempData["SuccessMessage"] = $"Welcome aboard, {user.FullName}! Your account is ready.";
                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error creating account: {ex.Message}");
                return View(model);
            }
        }

        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult Dashboard()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login");
            }

            var totalEmployees = _context.Employees.Count();
            var presentToday = _context.Attendances
                .Where(a => a.AttendanceDate.Date == DateTime.UtcNow.Date && a.Status == "present")
                .Count();
            var pendingLeaves = _context.Leaves.Where(l => l.Status == "pending").Count();
            var monthlyPayroll = _context.Payrolls.Sum(p => (decimal?)p.NetSalary) ?? 0;

            var dashboardData = new
            {
                TotalEmployees = totalEmployees,
                PresentToday = presentToday,
                PendingLeaves = pendingLeaves,
                MonthlyPayroll = monthlyPayroll
            };

            return View(dashboardData);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Employees()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult Attendance()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult Leaves()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult Payroll()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        private bool VerifyPassword(string password, string hash)
        {
            try
            {
                // Try BCrypt first
                if (hash.StartsWith("$2"))
                {
                    return BCrypt.Net.BCrypt.Verify(password, hash);
                }

                // Try PBKDF2 (from werkzeug/ASP.NET Identity)
                return VerifyPbkdf2Password(password, hash);
            }
            catch
            {
                return false;
            }
        }

        private bool VerifyPbkdf2Password(string password, string hash)
        {
            try
            {
                var hashManager = new PasswordHashManager();
                return hashManager.VerifyHashedPassword(null, hash, password) == PasswordVerificationResult.Success;
            }
            catch
            {
                return false;
            }
        }
    }

    public class LoginViewModel
    {
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Email is required")]
        [System.ComponentModel.DataAnnotations.EmailAddress(ErrorMessage = "Please enter a valid email")]
        public string? Email { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Password is required")]
        [System.ComponentModel.DataAnnotations.MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string? Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public class SignupViewModel
    {
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Full name is required")]
        [System.ComponentModel.DataAnnotations.StringLength(150, ErrorMessage = "Full name cannot exceed 150 characters")]
        public string? FullName { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Email is required")]
        [System.ComponentModel.DataAnnotations.EmailAddress(ErrorMessage = "Please enter a valid email")]
        public string? Email { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Password is required")]
        [System.ComponentModel.DataAnnotations.MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string? Password { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Please confirm your password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; }
    }

    public class PasswordHashManager
    {
        public PasswordVerificationResult VerifyHashedPassword(object? user, string hash, string providedPassword)
        {
            if (hash == null)
                throw new ArgumentNullException(nameof(hash));

            if (providedPassword == null)
                throw new ArgumentNullException(nameof(providedPassword));

            byte[] hashBytes = Convert.FromBase64String(hash.Substring(hash.IndexOf("$", 1) + 1));
            var pbkdf2 = new System.Security.Cryptography.Rfc2898DeriveBytes(providedPassword, hashBytes, 10000);
            byte[] testHash = pbkdf2.GetBytes(20);

            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != testHash[i])
                    return PasswordVerificationResult.Failed;
            }

            return PasswordVerificationResult.Success;
        }
    }

    public enum PasswordVerificationResult
    {
        Failed = 0,
        Success = 1
    }
}
