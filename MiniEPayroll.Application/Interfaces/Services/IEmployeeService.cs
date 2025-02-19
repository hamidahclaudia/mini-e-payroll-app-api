using MiniEPayroll.Application.DTOs;

namespace MiniEPayroll.Application.Interfaces.Services;

public interface IEmployeeService
{
    IEnumerable<EmployeeDto> GetAll(string? name, DateTime? dob, string? status, string sortField, string sortDirection);
    EmployeeDto GetById(int id);
    EmployeeDto Add(EmployeeDto employeeDto);
    void Update(EmployeeDto employeeDto);
    void Delete(int id);
    IEnumerable<EmployeeDto> GetEligibleEmployees(DateTime payday);
}
