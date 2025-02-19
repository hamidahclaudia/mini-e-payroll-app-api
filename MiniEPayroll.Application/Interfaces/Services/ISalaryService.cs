using MiniEPayroll.Application.DTOs;

namespace MiniEPayroll.Application.Interfaces.Services;

public interface ISalaryService
{
    SalaryDto ProcessSalary(int employeeId, DateTime salaryDate);
    IEnumerable<SalaryDto> GetSalariesForMonth(int year, int month);
    void DeleteSalary(int salaryId);
}