namespace MiniEPayroll.Application.DTOs;

public class DeductionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}