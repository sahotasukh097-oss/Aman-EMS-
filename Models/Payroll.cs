using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAPS_EMS_.Models
{
    public class Payroll
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }

        [StringLength(20)]
        public string? SalaryMonth { get; set; }

        public decimal BasicSalary { get; set; }

        public decimal Allowances { get; set; } = 0;

        public decimal Deductions { get; set; } = 0;

        public decimal NetSalary { get; set; }

        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual Employee? Employee { get; set; }
    }
}
