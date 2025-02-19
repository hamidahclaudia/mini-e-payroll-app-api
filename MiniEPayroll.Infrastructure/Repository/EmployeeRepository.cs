using Microsoft.EntityFrameworkCore;
using MiniEPayroll.Domain.Entities;
using MiniEPayroll.Domain.Repositories;

namespace MiniEPayroll.Infrastructure.Repository;

public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(ApplicationDbContext context) : base(context) { }
    
    public new IEnumerable<Employee> GetAll()
    {
        return _context.Employees
            .Include(e => e.Allowances)
            .Include(e => e.Deductions)
            .AsQueryable();
    }
    
    public new Employee? GetById(int id)
    {
        return _context.Employees
            .Where(emp => emp.Id == id)
            .Include(e => e.Allowances)
            .Include(e => e.Deductions)
            .FirstOrDefault();
    }

}
