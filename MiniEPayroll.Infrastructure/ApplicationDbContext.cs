using Microsoft.EntityFrameworkCore;
using MiniEPayroll.Domain.Entities;

namespace MiniEPayroll.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Allowance> Allowances { get; set; }
    public DbSet<Deduction> Deductions { get; set; }
    public DbSet<Salary> Salaries { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Allowance>()
            .HasOne(a => a.Employee)
            .WithMany(e => e.Allowances)
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Deduction>()
            .HasOne(d => d.Employee)
            .WithMany(e => e.Deductions)
            .HasForeignKey(d => d.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Salary>()
            .HasOne(s => s.Employee)
            .WithMany()
            .HasForeignKey(s => s.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    } 
}