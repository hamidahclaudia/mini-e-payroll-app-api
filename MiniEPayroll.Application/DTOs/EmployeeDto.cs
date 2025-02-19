namespace MiniEPayroll.Application.DTOs;

public class EmployeeDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateTime DOB { get; set; } = DateTime.Now;
    public string Gender { get; set; } = string.Empty;
    public DateTime JoinDate { get; set; }
    public DateTime? ResignDate { get; set; }
    public decimal BasicSalary { get; set; }
    public List<AllowanceDto> Allowances { get; set; } = new List<AllowanceDto>();
    public List<DeductionDto> Deductions { get; set; } = new List<DeductionDto>();
}