using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aman_EMS_.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        [StringLength(50)]
        public string? EmployeeCode { get; set; }

        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? LastName { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(200)]
        public string? Department { get; set; }

        [StringLength(100)]
        public string? Designation { get; set; }

        public DateTime? DateOfJoining { get; set; }

        public decimal? Salary { get; set; }

        public int Status { get; set; } = 1;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual User? User { get; set; }
        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public virtual ICollection<Leave> Leaves { get; set; } = new List<Leave>();
        public virtual ICollection<Payroll> Payrolls { get; set; } = new List<Payroll>();

        // Helper properties
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}".Trim();

        [NotMapped]
        public string Position => Designation ?? string.Empty;
    }
}
