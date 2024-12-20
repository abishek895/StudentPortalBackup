using Microsoft.EntityFrameworkCore;
using StudentPortal.Entities.Models;
using StudentPortal.Services.IServices;
using StudentPortal.Services.Services;
using Microsoft.AspNetCore.Cors;
using StudentPortal.Services;

namespace Student
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Enable Swagger for API documentation
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configure Entity Framework with SQL Server
            builder.Services.AddDbContext<StudentdbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("NewConnection")));

            // Register services
            builder.Services.AddScoped<IStudentService, StudentService>();
            builder.Services.AddScoped<IStudentMarkService, StudentMarkService>();
            builder.Services.AddScoped<ISemesterService, SemesterService>(); // Add this line
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();

            // Configure CORS to allow all origins
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => builder.AllowAnyOrigin()
                                      .AllowAnyHeader()
                                      .AllowAnyMethod());
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UseCors("AllowAllOrigins"); // Apply the CORS policy
            app.MapControllers();

            app.Run();
        }
    }
}
