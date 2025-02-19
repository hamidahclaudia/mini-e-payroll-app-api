using System.Globalization;
using AutoMapper;
using MiniEPayroll.Application.Interfaces.Services;
using MiniEPayroll.Domain.Repositories;

namespace MiniEPayroll.Application.Services;

public class PayslipService : IPayslipService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ISalaryRepository _salaryRepository;
    private readonly IMapper _mapper;

    public PayslipService(IEmployeeRepository employeeRepository, ISalaryRepository salaryRepository, IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _salaryRepository = salaryRepository;
        _mapper = mapper;
    }

    public string GeneratePayslipHtml(int employeeId, int year, int month)
    {
        // Retrieve the employee.
        var employee = _employeeRepository.GetById(employeeId);
        if (employee == null)
            throw new Exception("Employee not found.");

        // Retrieve the salary record for the given month.
        var salary = _salaryRepository
            .GetSalariesByMonth(year, month)
            .FirstOrDefault(s => s.EmployeeId == employeeId);

        if (salary == null)
            throw new Exception("Salary for this employee for the specified month was not processed.");

        // Calculate total allowances and deductions
        decimal totalAllowances = employee.Allowances.Sum(a => a.Amount);
        decimal totalDeductions = employee.Deductions.Sum(d => d.Amount);

        // Generate a simple HTML payslip. You can modify the HTML and styles as needed.
        string html = $@"
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; }}
        .payslip-container {{
            width: 600px;
            margin: 0 auto;
            border: 1px solid #ccc;
            padding: 20px;
        }}
        .header {{
            text-align: center;
            margin-bottom: 20px;
        }}
        .section {{
            margin-bottom: 10px;
        }}
        .section label {{
            font-weight: bold;
        }}
    </style>
    </head>
    <body>
        <div class='payslip-container'>
            <div class='header'>
                <h2>Payslip for {employee.FullName}</h2>
                <p>Salary Month: {year}-{month:D2}</p>
            </div>
            <div class='section'>
                <label>Basic Salary:</label> {employee.BasicSalary.ToString("C", new CultureInfo("en-US"))}
            </div>
            <div class='section'>
                <label>Total Allowances:</label> {totalAllowances.ToString("C", new CultureInfo("en-US"))}
            </div>
            <div class='section'>
                <label>Total Deductions:</label> {totalDeductions.ToString("C", new CultureInfo("en-US"))}
            </div>
            <div class='section'>
                <label>Net Salary:</label> {salary.TotalSalary.ToString("C", new CultureInfo("en-US"))}
            </div>
            <hr>
            <div class='section'>
                <p>Thank you for your service.</p>
            </div>
        </div>
    </body>
</html>";

        return html;
    }
}