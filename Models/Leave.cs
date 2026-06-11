using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aman_EMS_.Models
{
    public class Leave
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(50)]
        public string LeaveType { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string? Reason { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "pending";

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual Employee? Employee { get; set; }
    }
}
