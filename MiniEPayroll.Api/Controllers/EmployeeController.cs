using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MiniEPayroll.Application.DTOs;
using MiniEPayroll.Application.Interfaces.Services;

namespace MiniEPayroll.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly IMapper _mapper;

    public EmployeeController(IEmployeeService employeeService, IMapper mapper)
    {
        _employeeService = employeeService;
        _mapper = mapper;
    }
    
    [HttpGet]
    public IActionResult GetAll(
        [FromQuery] string? name = null,
        [FromQuery] DateTime? dob = null,
        [FromQuery] string? status = null,
        [FromQuery] string sortField = "fullName",
        [FromQuery] string sortDirection = "asc")
    {
        try
        {
            var employees = _employeeService.GetAll(name, dob, status, sortField, sortDirection);
            return Ok(employees);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var employee = _employeeService.GetById(id);
        if (employee == null)
            return NotFound();
        return Ok(employee);
    }

    [HttpPost]
    public IActionResult Add([FromBody] EmployeeDto employeeDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = _employeeService.Add(employeeDto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] EmployeeDto employeeDto)
    {
        if (id != employeeDto.Id)
            return BadRequest("Employee ID mismatch.");

        _employeeService.Update(employeeDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _employeeService.Delete(id);
        return NoContent();
    }

    [HttpGet("eligible")]
    public IActionResult GetEligibleEmployees([FromQuery] DateTime? payday = null)
    {
        try
        {
            // Default to today's date if payday is not provided
            DateTime actualPayday = payday ?? DateTime.Today;
            
            var employees = _employeeService.GetEligibleEmployees(actualPayday);
            return Ok(employees);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}