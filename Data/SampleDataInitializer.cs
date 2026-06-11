using Aman_EMS_.Models;

namespace Aman_EMS_.Data
{
    public static class SampleDataInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context)
        {
            try
            {
                // Check if we already have data
                if (context.Users.Any())
                {
                    return; // Database has been seeded
                }

                // Create admin user
                var adminUser = new User
                {
                    Username = "admin",
                    Email = "admin@aman.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    FullName = "Administrator",
                    Status = 1,
                    CreatedAt = DateTime.UtcNow
                };

                context.Users.Add(adminUser);
                await context.SaveChangesAsync();

                // Create 80 Indian employees
                var employees = GenerateIndianEmployees(adminUser.Id);
                context.Employees.AddRange(employees);
                await context.SaveChangesAsync();

                // Generate attendance records for last 30 days
                var attendanceRecords = GenerateAttendanceRecords(employees);
                context.Attendances.AddRange(attendanceRecords);
                await context.SaveChangesAsync();

                // Generate leave records
                var leaveRecords = GenerateLeaveRecords(employees);
                context.Leaves.AddRange(leaveRecords);
                await context.SaveChangesAsync();

                // Generate payroll records
                var payrollRecords = GeneratePayrollRecords(employees);
                context.Payrolls.AddRange(payrollRecords);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log error but don't crash the application
                System.Diagnostics.Debug.WriteLine($"Error initializing sample data: {ex.Message}");
            }
        }

        private static List<Employee> GenerateIndianEmployees(int adminUserId)
        {
            var firstNames = new[] { "Rajesh", "Priya", "Amit", "Neha", "Vikram", "Anjali", "Rohan", "Pooja", "Arun", "Divya", "Arjun", "Shreya", "Manish", "Ananya", "Sanjay", "Ritika", "Deepak", "Sonali", "Nikhil", "Vaishali", "Ashok", "Nisha", "Rahul", "Isha", "Suresh", "Kavya", "Karan", "Diya", "Akshay", "Riya", "Vipin", "Meera", "Harsh", "Swati", "Naveen", "Preeti", "Gaurav", "Simran", "Varun", "Anuj" };
            var lastNames = new[] { "Sharma", "Patel", "Singh", "Gupta", "Kumar", "Verma", "Agarwal", "Mishra", "Joshi", "Iyer", "Nair", "Menon", "Reddy", "Rao", "Bhat", "Desai", "Shah", "Kulkarni", "Pandey", "Sinha", "Saxena", "Trivedi", "Banerjee", "Chaudhury", "Ghosh", "Ray", "Das", "Sen", "Dasgupta", "Bose", "Bhatt", "Kapur", "Malhotra", "Tandon", "Bhatia", "Chopra", "Jain", "Srivastava", "Yadav", "Chawla" };
            var departments = new[] { "Engineering", "Marketing", "Sales", "HR", "Finance", "Operations", "Customer Service", "Quality Assurance", "Business Development", "Administration", "Product Management", "Research & Development" };
            var designations = new[] { "Senior Developer", "Junior Developer", "Manager", "Executive", "Lead", "Coordinator", "Specialist", "Analyst", "Associate", "Director", "Principal Engineer", "Team Lead", "Consultant", "Engineer" };

            var employees = new List<Employee>();
            var random = new Random(42);

            for (int i = 1; i <= 80; i++)
            {
                var firstName = firstNames[random.Next(firstNames.Length)];
                var lastName = lastNames[random.Next(lastNames.Length)];
                var department = departments[random.Next(departments.Length)];
                var designation = designations[random.Next(designations.Length)];
                var salary = random.Next(300000, 1500000); // In INR

                employees.Add(new Employee
                {
                    UserId = adminUserId,
                    EmployeeCode = $"EMP{i:D4}",
                    FirstName = firstName,
                    LastName = lastName,
                    Phone = $"98{random.Next(10000000, 99999999)}",
                    Department = department,
                    Designation = $"{designation} - {department}",
                    Salary = salary,
                    DateOfJoining = new DateTime(2016 + random.Next(8), random.Next(1, 13), random.Next(1, 28)),
                    Status = random.Next(10) > 0 ? 1 : 0, // 90% active employees
                    CreatedAt = DateTime.UtcNow
                });
            }

            return employees;
        }

        private static List<Attendance> GenerateAttendanceRecords(List<Employee> employees)
        {
            var attendanceRecords = new List<Attendance>();
            var random = new Random(42);

            // Generate records for last 90 days (3 months)
            for (int dayOffset = -90; dayOffset < 0; dayOffset++)
            {
                var date = DateTime.UtcNow.AddDays(dayOffset).Date;

                // Skip weekends
                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                foreach (var employee in employees)
                {
                    // Weighted random status (75% present, 10% absent, 10% leave, 5% half-day)
                    var statusRoll = random.Next(100);
                    string status;
                    if (statusRoll < 75) status = "present";
                    else if (statusRoll < 85) status = "absent";
                    else if (statusRoll < 95) status = "leave";
                    else status = "half-day";

                    TimeOnly? checkIn = status == "absent" ? null : new TimeOnly(9, 0).AddMinutes(random.Next(-20, 40));
                    TimeOnly? checkOut = status == "absent" || status == "leave" ? null : 
                        (status == "half-day" ? new TimeOnly(14, 0).AddMinutes(random.Next(-15, 25)) :
                         new TimeOnly(18, 0).AddMinutes(random.Next(-30, 40)));

                    attendanceRecords.Add(new Attendance
                    {
                        EmployeeId = employee.Id,
                        AttendanceDate = date,
                        CheckIn = checkIn,
                        CheckOut = checkOut,
                        Status = status,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            return attendanceRecords;
        }

        private static List<Leave> GenerateLeaveRecords(List<Employee> employees)
        {
            var leaveRecords = new List<Leave>();
            var random = new Random(42);
            var leaveTypes = new[] { "Casual Leave", "Sick Leave", "Annual Leave", "Maternity Leave", "Paternity Leave" };

            // Generate 2-5 leave records per employee
            foreach (var employee in employees)
            {
                int leaveCount = random.Next(2, 6);
                for (int i = 0; i < leaveCount; i++)
                {
                    var startDate = DateTime.UtcNow.AddDays(random.Next(-180, 180));
                    var endDate = startDate.AddDays(random.Next(1, 8));
                    var leaveType = leaveTypes[random.Next(leaveTypes.Length)];

                    // Weighted status distribution
                    var statusRoll = random.Next(100);
                    var status = statusRoll < 60 ? "approved" : (statusRoll < 80 ? "pending" : "rejected");

                    leaveRecords.Add(new Leave
                    {
                        EmployeeId = employee.Id,
                        LeaveType = leaveType,
                        StartDate = startDate,
                        EndDate = endDate,
                        Reason = GenerateLeaveReason(leaveType, random),
                        Status = status,
                        AppliedAt = DateTime.UtcNow.AddDays(random.Next(-180, 0))
                    });
                }
            }

            return leaveRecords;
        }

        private static string GenerateLeaveReason(string leaveType, Random random)
        {
            return leaveType switch
            {
                "Casual Leave" => new[] { "Personal work", "Family commitment", "Medical checkup", "Emergency", "Urgent meeting" }[random.Next(5)],
                "Sick Leave" => new[] { "Fever and cold", "Dental appointment", "Medical treatment", "Surgery", "Health issues" }[random.Next(5)],
                "Annual Leave" => new[] { "Vacation planning", "Family holiday", "Rest and relaxation", "Travel", "Personal time" }[random.Next(5)],
                "Maternity Leave" => "Maternity leave",
                "Paternity Leave" => "Paternity leave",
                _ => leaveType
            };
        }

        private static List<Payroll> GeneratePayrollRecords(List<Employee> employees)
        {
            var payrollRecords = new List<Payroll>();
            var random = new Random(42);
            var months = new[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

            // Generate payroll for last 18 months
            for (int monthOffset = 0; monthOffset < 18; monthOffset++)
            {
                var monthIndex = (DateTime.UtcNow.Month - monthOffset - 1 + 120) % 12;
                var year = DateTime.UtcNow.Year - ((DateTime.UtcNow.Month - monthOffset - 1) < 0 ? 1 : 0);
                var month = months[monthIndex];

                foreach (var employee in employees)
                {
                    var basicSalary = (decimal)(employee.Salary ?? 500000);

                    // Variable allowances (10-20% of basic)
                    var allowancePercentage = random.Next(10, 21) / 100m;
                    var allowances = basicSalary * allowancePercentage;

                    // Variable deductions (5-12% of basic)
                    var deductionPercentage = random.Next(5, 13) / 100m;
                    var deductions = basicSalary * deductionPercentage;

                    var netSalary = basicSalary + allowances - deductions;

                    // Add slight date variation
                    var generatedDate = new DateTime(year, monthIndex + 1, 1).AddDays(random.Next(0, 5));

                    payrollRecords.Add(new Payroll
                    {
                        EmployeeId = employee.Id,
                        SalaryMonth = $"{month} {year}",
                        BasicSalary = basicSalary,
                        Allowances = Math.Round(allowances, 2),
                        Deductions = Math.Round(deductions, 2),
                        NetSalary = Math.Round(netSalary, 2),
                        GeneratedAt = generatedDate
                    });
                }
            }

            return payrollRecords;
        }
    }
}
