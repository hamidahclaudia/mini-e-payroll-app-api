using AutoMapper;
using MiniEPayroll.Application.DTOs;
using MiniEPayroll.Application.Interfaces.Services;
using MiniEPayroll.Domain.Entities;
using MiniEPayroll.Domain.Repositories;

namespace MiniEPayroll.Application.Services;

public class SalaryService : ISalaryService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ISalaryRepository _salaryRepository;
    private readonly IMapper _mapper;

    public SalaryService(IEmployeeRepository employeeRepository, ISalaryRepository salaryRepository, IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _salaryRepository = salaryRepository;
        _mapper = mapper;
    }

    public SalaryDto ProcessSalary(int employeeId, DateTime salaryDate)
    {
        var employee = _employeeRepository.GetById(employeeId);
        if (employee == null)
            throw new Exception("Employee not found.");

        // 🔹 Determine full month boundaries for salaryDate
        DateTime fullMonthStart = new DateTime(salaryDate.Year, salaryDate.Month, 1);
        DateTime fullMonthEnd = fullMonthStart.AddMonths(1).AddDays(-1);

        // 🔹 Adjust period based on employee's join date, resign date, and salaryDate
        DateTime periodStart = employee.JoinDate > fullMonthStart ? employee.JoinDate : fullMonthStart;
        DateTime periodEnd = (employee.ResignDate.HasValue && employee.ResignDate.Value < salaryDate)
            ? employee.ResignDate.Value
            : salaryDate; // Salary is only processed up to salaryDate

        // 🔹 Get total working days in the full month (Excluding Saturdays & Sundays)
        int totalWorkingDays = CountWorkingDays(fullMonthStart, fullMonthEnd);

        // 🔹 Get employee’s actual working days up to salaryDate
        int employeeWorkingDays = CountWorkingDays(periodStart, periodEnd);

        // 🔹 Calculate prorated salary based on actual working days
        decimal ratio = totalWorkingDays > 0 ? (decimal)employeeWorkingDays / totalWorkingDays : 0m;
        decimal basicSalaryProRated = employee.BasicSalary * ratio;

        // 🔹 Compute prorated allowances and deductions (this case prorate only for basic salary)
        decimal totalAllowances = employee.Allowances.Sum(a => a.Amount);
        decimal totalDeductions = employee.Deductions.Sum(d => d.Amount);

        decimal totalSalary = basicSalaryProRated + totalAllowances - totalDeductions;

        // 🔹 Check if a salary record already exists for the same employee & month.
        var existingSalary = _salaryRepository
            .GetSalariesByMonth(salaryDate.Year, salaryDate.Month)
            .FirstOrDefault(s => s.EmployeeId == employeeId);

        if (existingSalary != null)
        {
            // 🔹 Update existing record instead of adding a new one.
            existingSalary.TotalSalary = totalSalary;
            existingSalary.SalaryDate = salaryDate;
            _salaryRepository.Update(existingSalary);
            return _mapper.Map<SalaryDto>(existingSalary);
        }
        else
        {
            // 🔹 If no record exists, create a new one.
            var salaryEntity = new Salary
            {
                EmployeeId = employee.Id,
                SalaryDate = salaryDate,
                TotalSalary = totalSalary
            };

            _salaryRepository.Add(salaryEntity);
            return _mapper.Map<SalaryDto>(salaryEntity);
        }
    }
    
    private int CountWorkingDays(DateTime start, DateTime end)
    {
        int workingDays = 0;
        for (DateTime date = start; date <= end; date = date.AddDays(1))
        {
            if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
            {
                workingDays++;
            }
        }
        return workingDays;
    }
    
    public IEnumerable<SalaryDto> GetSalariesForMonth(int year, int month)
    {
        var salaries = _salaryRepository.GetSalariesByMonth(year, month)
            .Select(s => new SalaryDto
            {
                Id = s.Id,
                EmployeeId = s.EmployeeId,
                EmployeeName = s.Employee.FullName,
                SalaryDate = s.SalaryDate,
                TotalSalary = s.TotalSalary
            });
        return salaries;
    }

    public void DeleteSalary(int salaryId)
    {
        _salaryRepository.Delete(salaryId);
    }
}