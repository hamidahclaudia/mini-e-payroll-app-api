namespace MiniEPayroll.Domain.Entities;

public class Employee
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateTime DOB { get; set; } = DateTime.Now;
    public string Gender { get; set; } = string.Empty;
    public DateTime JoinDate { get; set; }
    public DateTime? ResignDate { get; set; }
    public decimal BasicSalary { get; set; }
    public List<Allowance> Allowances { get; set; } = new List<Allowance>();
    public List<Deduction> Deductions { get; set; } = new List<Deduction>();
}