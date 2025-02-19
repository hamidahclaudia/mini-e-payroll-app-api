namespace MiniEPayroll.Application.Interfaces.Services;

public interface IPayslipService
{
    string GeneratePayslipHtml(int employeeId, int year, int month);
}