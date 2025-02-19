namespace MiniEPayroll.Application.DTOs;

public class SalaryProcessRequestDto
{
    public List<int> EmployeeIds { get; set; }
    public DateTime SalaryDate { get; set; }
}