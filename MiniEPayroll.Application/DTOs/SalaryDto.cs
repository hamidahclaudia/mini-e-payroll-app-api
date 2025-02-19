namespace MiniEPayroll.Application.DTOs;

public class SalaryDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public DateTime SalaryDate { get; set; }
    public decimal TotalSalary { get; set; }
}