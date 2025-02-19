using AutoMapper;
using MiniEPayroll.Application.DTOs;
using MiniEPayroll.Application.Interfaces.Services;
using MiniEPayroll.Domain.Entities;
using MiniEPayroll.Domain.Repositories;

namespace MiniEPayroll.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IMapper _mapper;

    public EmployeeService(IEmployeeRepository employeeRepository, IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _mapper = mapper;
    }
    
    public IEnumerable<EmployeeDto> GetAll(string? name, DateTime? dob, string? status, string sortField, string sortDirection)
    {
        var employees = _employeeRepository.GetAll();
        
        if (!string.IsNullOrEmpty(name))
        {
            employees = employees.Where(emp => emp.FullName.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        if (dob.HasValue)
        {
            employees = employees.Where(emp => emp.DOB.Date == dob.Value.Date);
        }

        if (!string.IsNullOrEmpty(status))
        {
            if (status == "active")
            {
                employees = employees.Where(emp => emp.ResignDate == null);
            }
            else if (status == "resigned")
            {
                employees = employees.Where(emp => emp.ResignDate != null);
            }
        }
        
        employees = sortDirection == "asc"
            ? employees.OrderBy(e => GetPropertyValue(e, sortField))
            : employees.OrderByDescending(e => GetPropertyValue(e, sortField));

        return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
    }

    private static object GetPropertyValue(Employee employee, string propertyName)
    {
        var property = typeof(Employee).GetProperty(propertyName);
        return property?.GetValue(employee) ?? "";
    }
    
    public EmployeeDto GetById(int id)
    {
        var employee = _employeeRepository.GetById(id);
        return employee == null ? null : _mapper.Map<EmployeeDto>(employee);
    }

    public EmployeeDto Add(EmployeeDto employeeDto)
    {
        // Example validations
        if ((DateTime.Now - employeeDto.DOB).TotalDays / 365.25 < 18)
            throw new Exception("Employee must be above 18 years.");
        if (employeeDto.ResignDate.HasValue && employeeDto.ResignDate < employeeDto.JoinDate)
            throw new Exception("Resign date must not be earlier than join date.");

        var employee = _mapper.Map<Employee>(employeeDto);
        var created = _employeeRepository.Add(employee);
        return _mapper.Map<EmployeeDto>(created);
    }

    public void Update(EmployeeDto employeeDto)
    {
        if ((DateTime.Now - employeeDto.DOB).TotalDays / 365.25 < 18)
            throw new Exception("Employee must be above 18 years.");
        if (employeeDto.ResignDate.HasValue && employeeDto.ResignDate < employeeDto.JoinDate)
            throw new Exception("Resign date must not be earlier than join date.");

        var employee = _mapper.Map<Employee>(employeeDto);
        _employeeRepository.Update(employee);
    }

    public void Delete(int id)
    {
        _employeeRepository.Delete(id);
    }
    
    public IEnumerable<EmployeeDto> GetEligibleEmployees(DateTime payday)
    {
        var employees = _employeeRepository.GetAll().Where(emp =>
                emp.JoinDate <= payday &&  // Employee must have joined on or before payday
                (emp.ResignDate == null || emp.ResignDate >= payday) // Employee not resigned or resigned after payday
        );
        return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
    }

}