using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using AutoMapper;
using MiniEPayroll.Application.DTOs;
using MiniEPayroll.Application.Interfaces.Services;
using MiniEPayroll.Application.Services;
using MiniEPayroll.Domain.Entities;
using MiniEPayroll.Domain.Repositories;

namespace MiniEPayroll.Tests.Services
{
    public class SalaryServiceTests
    {
        private readonly Mock<IEmployeeRepository> _mockEmployeeRepository;
        private readonly Mock<ISalaryRepository> _mockSalaryRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ISalaryService _salaryService;

        public SalaryServiceTests()
        {
            _mockEmployeeRepository = new Mock<IEmployeeRepository>();
            _mockSalaryRepository = new Mock<ISalaryRepository>();
            _mockMapper = new Mock<IMapper>();

            _salaryService = new SalaryService(
                _mockEmployeeRepository.Object,
                _mockSalaryRepository.Object,
                _mockMapper.Object);
        }

        [Fact]
        public void ProcessSalary_ShouldCalculateCorrectly_AndInsertNewSalary()
        {
            // Arrange
            int employeeId = 1;
            DateTime salaryDate = new DateTime(2025, 2, 28);
            var employee = new Employee
            {
                Id = employeeId,
                FullName = "John Doe",
                JoinDate = new DateTime(2025, 2, 1),
                BasicSalary = 3000,
                Allowances = new List<Allowance> { new Allowance { Amount = 500 } },
                Deductions = new List<Deduction> { new Deduction { Amount = 200 } }
            };

            _mockEmployeeRepository.Setup(repo => repo.GetById(employeeId)).Returns(employee);
            _mockSalaryRepository.Setup(repo => repo.GetSalariesByMonth(2025, 2))
                .Returns(new List<Salary>().AsQueryable());

            _mockMapper.Setup(m => m.Map<SalaryDto>(It.IsAny<Salary>()))
                .Returns((Salary s) => new SalaryDto
                {
                    Id = s.Id,
                    EmployeeId = s.EmployeeId,
                    TotalSalary = s.TotalSalary,
                    EmployeeName = employee.FullName,
                    SalaryDate = salaryDate,
                });

            // Act
            var result = _salaryService.ProcessSalary(employeeId, salaryDate);

            // Assert
            _mockSalaryRepository.Verify(repo => repo.Add(It.IsAny<Salary>()), Times.Once);
            _mockSalaryRepository.Verify(repo => repo.Update(It.IsAny<Salary>()), Times.Never);
            Assert.Equal(3300, result.TotalSalary); // 3000 basic + 500 allowance - 200 deduction
        }

        [Fact]
        public void ProcessSalary_ShouldUpdateExistingSalary_WhenAlreadyExists()
        {
            // Arrange
            int employeeId = 1;
            DateTime salaryDate = new DateTime(2025, 2, 28);
            var employee = new Employee
            {
                Id = employeeId,
                FullName = "John Doe",
                JoinDate = new DateTime(2025, 2, 1),
                BasicSalary = 3000,
                Allowances = new List<Allowance> { new Allowance { Amount = 500 } },
                Deductions = new List<Deduction> { new Deduction { Amount = 200 } }
            };
            var existingSalary = new Salary
            {
                Id = 100,
                EmployeeId = employeeId,
                SalaryDate = salaryDate,
                TotalSalary = 0
            };

            _mockEmployeeRepository.Setup(repo => repo.GetById(employeeId)).Returns(employee);
            _mockSalaryRepository.Setup(repo => repo.GetSalariesByMonth(2025, 2))
                .Returns(new List<Salary> { existingSalary }.AsQueryable());

            _mockMapper.Setup(m => m.Map<SalaryDto>(It.IsAny<Salary>()))
                .Returns((Salary s) => new SalaryDto
                    { Id = s.Id, EmployeeId = s.EmployeeId, TotalSalary = s.TotalSalary });

            // Act
            var result = _salaryService.ProcessSalary(employeeId, salaryDate);

            // Assert
            _mockSalaryRepository.Verify(repo => repo.Update(It.IsAny<Salary>()), Times.Once);
            _mockSalaryRepository.Verify(repo => repo.Add(It.IsAny<Salary>()), Times.Never);
            Assert.Equal(3300, result.TotalSalary);
        }

        [Fact]
        public void ProcessSalary_ShouldCalculateProratedSalary_ForPartialMonthEmployee()
        {
            // Arrange
            int employeeId = 1;
            DateTime salaryDate = new DateTime(2025, 2, 28); // Salary paid at month-end

            var employee = new Employee
            {
                Id = employeeId,
                FullName = "John Partial",
                JoinDate = new DateTime(2025, 2, 15), // Joined mid-month
                ResignDate = null, // Still active
                BasicSalary = 3000,
                Allowances = new List<Allowance> { new Allowance { Amount = 500 } },
                Deductions = new List<Deduction> { new Deduction { Amount = 200 } }
            };

            // February 2025 has 20 working days (excluding weekends)
            // Employee worked from Feb 15 - Feb 28 (10 working days)
            int expectedTotalWorkingDays = 20; // Total for month
            int expectedEmployeeWorkingDays = 10; // Employee's actual working days
            decimal expectedRatio = (decimal)expectedEmployeeWorkingDays / expectedTotalWorkingDays; // 10/20 = 0.5

            decimal expectedProratedBasicSalary = employee.BasicSalary * expectedRatio; // 3000 * 0.5 = 1500
            decimal expectedProratedAllowances = 500;
            decimal expectedProratedDeductions = 200;

            decimal expectedTotalSalary =
                expectedProratedBasicSalary + expectedProratedAllowances -
                expectedProratedDeductions; // 1500 + 500 - 100 = 1900

            _mockEmployeeRepository.Setup(repo => repo.GetById(employeeId)).Returns(employee);
            _mockSalaryRepository.Setup(repo => repo.GetSalariesByMonth(2025, 2))
                .Returns(new List<Salary>().AsQueryable()); // No existing salary record

            _mockMapper.Setup(m => m.Map<SalaryDto>(It.IsAny<Salary>()))
                .Returns((Salary s) => new SalaryDto
                {
                    Id = s.Id,
                    EmployeeId = s.EmployeeId,
                    TotalSalary = s.TotalSalary,
                    EmployeeName = employee.FullName,
                    SalaryDate = salaryDate,
                });

            // Act
            var result = _salaryService.ProcessSalary(employeeId, salaryDate);

            // Assert
            _mockSalaryRepository.Verify(repo => repo.Add(It.IsAny<Salary>()), Times.Once);
            _mockSalaryRepository.Verify(repo => repo.Update(It.IsAny<Salary>()), Times.Never);
            Assert.Equal(expectedTotalSalary, result.TotalSalary); // Expect 1650
        }


        [Fact]
        public void GetSalariesForMonth_ShouldReturnMappedSalaries()
        {
            // Arrange
            int year = 2025, month = 2;
            var salaries = new List<Salary>
            {
                new Salary
                {
                    Id = 1, EmployeeId = 1, SalaryDate = new DateTime(2025, 2, 28), TotalSalary = 3200,
                    Employee = new Employee { FullName = "Alice" }
                },
                new Salary
                {
                    Id = 2, EmployeeId = 2, SalaryDate = new DateTime(2025, 2, 28), TotalSalary = 2800,
                    Employee = new Employee { FullName = "Bob" }
                }
            };

            _mockSalaryRepository.Setup(repo => repo.GetSalariesByMonth(year, month))
                .Returns(salaries.AsQueryable());

            _mockMapper.Setup(m => m.Map<IEnumerable<SalaryDto>>(It.IsAny<IEnumerable<Salary>>()))
                .Returns((IEnumerable<Salary> s) => s.Select(x => new SalaryDto
                {
                    Id = x.Id, EmployeeId = x.EmployeeId, EmployeeName = x.Employee.FullName,
                    TotalSalary = x.TotalSalary
                }));

            // Act
            var result = _salaryService.GetSalariesForMonth(year, month);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, s => s.EmployeeName == "Alice");
            Assert.Contains(result, s => s.EmployeeName == "Bob");
        }

        [Fact]
        public void DeleteSalary_ShouldCallRepositoryDelete()
        {
            // Arrange
            int salaryId = 1;

            // Act
            _salaryService.DeleteSalary(salaryId);

            // Assert
            _mockSalaryRepository.Verify(repo => repo.Delete(salaryId), Times.Once);
        }
    }
}