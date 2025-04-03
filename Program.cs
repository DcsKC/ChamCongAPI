using System.Text;
using ChamCongAPI;
using ChamCongAPI.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Cấu hình Entity Framework Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Thêm Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "ChamCongAPI",
        Version = "v1",
        Description = "API cho hệ thống chấm công"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    // Kích hoạt Swagger và Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChamCongAPI v1");
        c.RoutePrefix = "swagger"; // Đặt Swagger UI tại /swagger
    });
}
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated(); // Đảm bảo database đã tạo

    // Kiểm tra nếu chưa có Admin thì thêm vào
    if (!context.Employees.Any(e => e.Email == "admin@company.com"))
    {
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword("Admin@123");

        var admin = new Employee
        {
            EmployeeCode = "ADMIN001",
            Name = "Administrator",
            Department = "Management",
            Position = "Admin",
            Email = "admin@company.com",
            PasswordHash = hashedPassword,
            Rules = "Admin"
        };

        context.Employees.Add(admin);
        context.SaveChanges();
    }
}

app.UseHttpsRedirection();
app.UseRouting();

app.MapControllers();

app.Run();