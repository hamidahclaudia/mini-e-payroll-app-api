using Microsoft.EntityFrameworkCore;
using MiniEPayroll.Application.Interfaces.Services;
using MiniEPayroll.Application.Services;
using MiniEPayroll.Domain.Repositories;
using MiniEPayroll.Infrastructure;
using MiniEPayroll.Infrastructure.Repository;

namespace MiniEPayroll.Api;

public class Startup
{
    public IConfiguration Configuration { get; }
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }
        
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        // Configure EF Core to use SQL Server.
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("MiniEPayrollConnection")));

        // Register repository implementations.
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<ISalaryRepository, SalaryRepository>();

        // Register application services.
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<ISalaryService, SalaryService>();
        services.AddScoped<IPayslipService, PayslipService>();

        // Register AutoMapper with our mapping profile.
        services.AddAutoMapper(typeof(MappingProfile));

        // Add Swagger for API documentation (optional).
        services.AddSwaggerGen();
        
        // Add CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin", policy =>
            {
                policy.WithOrigins("http://localhost:4200")  // ✅ Only allow frontend running on localhost:4200
                    .AllowAnyMethod()   // Allow GET, POST, PUT, DELETE, etc.
                    .AllowAnyHeader()   // Allow all headers (e.g., Authorization, Content-Type)
                    .AllowCredentials(); // Allow credentials (cookies, JWT tokens, etc.)
            });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            // Enable Swagger middleware.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MiniEPayroll API V1");
            });
        }
        app.UseRouting();
        app.UseCors("AllowSpecificOrigin"); 
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}