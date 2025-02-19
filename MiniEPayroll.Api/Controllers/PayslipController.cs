using Microsoft.AspNetCore.Mvc;
using MiniEPayroll.Application.Interfaces.Services;

namespace MiniEPayroll.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PayslipController : ControllerBase
{
    private readonly IPayslipService _payslipService;

    public PayslipController(IPayslipService payslipService)
    {
        _payslipService = payslipService;
    }
    
    [HttpGet("{employeeId}/{year}/{month}")]
    public IActionResult GetPayslip(int employeeId, int year, int month, [FromQuery] string format = "html")
    {
        try
        {
            var html = _payslipService.GeneratePayslipHtml(employeeId, year, month);
            return Content(html, "text/html");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}