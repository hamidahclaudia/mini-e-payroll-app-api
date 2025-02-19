using Microsoft.AspNetCore.Mvc;
using MiniEPayroll.Application.DTOs;
using MiniEPayroll.Application.Interfaces.Services;

namespace MiniEPayroll.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalaryController : ControllerBase
{
    private readonly ISalaryService _salaryService;

    public SalaryController(ISalaryService salaryService)
    {
        _salaryService = salaryService;
    }
    
    [HttpPost("process")]
    public IActionResult ProcessSalaries([FromBody] SalaryProcessRequestDto request)
    {
        if (request.EmployeeIds == null || !request.EmployeeIds.Any())
            return BadRequest("No employees selected.");

        List<SalaryDto> processedSalaries = new List<SalaryDto>();

        foreach (var empId in request.EmployeeIds)
        {
            try
            {
                var salary = _salaryService.ProcessSalary(empId, request.SalaryDate);
                processedSalaries.Add(salary);
            }
            catch (System.Exception ex)
            {
                return BadRequest($"Error processing employee {empId}: {ex.Message}");
            }
        }
        return Ok(processedSalaries);
    }
    
    [HttpGet]
    public IActionResult GetSalaries([FromQuery] int year, [FromQuery] int month)
    {
        var salaries = _salaryService.GetSalariesForMonth(year, month);
        return Ok(salaries);
    }
    
    [HttpDelete("{id}")]
    public IActionResult DeleteSalary(int id)
    {
        _salaryService.DeleteSalary(id);
        return NoContent();
    }
}