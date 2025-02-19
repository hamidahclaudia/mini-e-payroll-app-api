using MiniEPayroll.Domain.Entities;

namespace MiniEPayroll.Domain.Repositories;

public interface ISalaryRepository : IRepository<Salary>
{
    IEnumerable<Salary> GetSalariesByMonth(int year, int month);
}