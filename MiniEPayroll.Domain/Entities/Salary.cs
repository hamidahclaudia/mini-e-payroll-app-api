namespace MiniEPayroll.Domain.Entities;

public class Salary
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public virtual Employee Employee { get; set; } = null!;
    public DateTime SalaryDate { get; set; }
    public decimal TotalSalary { get; set; }
    
}