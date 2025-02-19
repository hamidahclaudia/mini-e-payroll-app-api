using Microsoft.EntityFrameworkCore;
using MiniEPayroll.Domain.Entities;
using MiniEPayroll.Domain.Repositories;

namespace MiniEPayroll.Infrastructure.Repository;

public class SalaryRepository : Repository<Salary>, ISalaryRepository
{
    public SalaryRepository(ApplicationDbContext context) : base(context) { }

    public IEnumerable<Salary> GetSalariesByMonth(int year, int month)
    {
        return _context.Salaries
            .Include(es => es.Employee) // Ensure Employee Data is Loaded
            .Where(es => es.SalaryDate.Year == year && es.SalaryDate.Month == month)
            .ToList();
    }
}