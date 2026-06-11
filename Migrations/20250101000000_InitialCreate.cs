using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aman_EMS_.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Amanhr_users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amanhr_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Amanhr_employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Department = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Designation = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Salary = table.Column<decimal>(type: "TEXT", nullable: false),
                    DateOfJoining = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amanhr_employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Amanhr_employees_Amanhr_users_UserId",
                        column: x => x.UserId,
                        principalTable: "Amanhr_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Amanhr_attendance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    AttendanceDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CheckInTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CheckOutTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amanhr_attendance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Amanhr_attendance_Amanhr_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Amanhr_employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Amanhr_leaves",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    LeaveType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amanhr_leaves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Amanhr_leaves_Amanhr_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Amanhr_employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Amanhr_payroll",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Month = table.Column<int>(type: "INTEGER", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    BaseSalary = table.Column<decimal>(type: "TEXT", nullable: false),
                    Deductions = table.Column<decimal>(type: "TEXT", nullable: false),
                    Bonuses = table.Column<decimal>(type: "TEXT", nullable: false),
                    NetSalary = table.Column<decimal>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amanhr_payroll", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Amanhr_payroll_Amanhr_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Amanhr_employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Amanhr_attendance_EmployeeId",
                table: "Amanhr_attendance",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Amanhr_employees_UserId",
                table: "Amanhr_employees",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Amanhr_leaves_EmployeeId",
                table: "Amanhr_leaves",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Amanhr_payroll_EmployeeId",
                table: "Amanhr_payroll",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Amanhr_attendance");

            migrationBuilder.DropTable(
                name: "Amanhr_leaves");

            migrationBuilder.DropTable(
                name: "Amanhr_payroll");

            migrationBuilder.DropTable(
                name: "Amanhr_employees");

            migrationBuilder.DropTable(
                name: "Amanhr_users");
        }
    }
}
