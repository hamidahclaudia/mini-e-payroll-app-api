namespace MiniEPayroll.Domain.Entities;

public class Deduction
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int EmployeeId { get; set; }
    public virtual Employee Employee { get; set; }
}