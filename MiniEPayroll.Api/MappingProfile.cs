using AutoMapper;
using MiniEPayroll.Application.DTOs;
using MiniEPayroll.Domain.Entities;

namespace MiniEPayroll.Api;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Employee, EmployeeDto>();
        CreateMap<EmployeeDto, Employee>();

        CreateMap<Allowance, AllowanceDto>();
        CreateMap<AllowanceDto, Allowance>();

        CreateMap<Deduction, DeductionDto>();
        CreateMap<DeductionDto, Deduction>();
        
        CreateMap<Salary, SalaryDto>();
        CreateMap<SalaryDto, Salary>();
    }
}