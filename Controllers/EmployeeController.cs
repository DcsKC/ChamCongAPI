using Microsoft.AspNetCore.Mvc;
using ChamCongAPI.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace ChamCongAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _context.Employees.ToListAsync();
            return Ok(employees);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return Ok(employee);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] Employee employee)
        {
            if (id != employee.Id)
            {
                return BadRequest();
            }

            _context.Entry(employee).State = EntityState.Modified;
            var existingEmployee = await _context.Employees.FindAsync(id);
            if (existingEmployee == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(employee.PasswordHash))
            {
                existingEmployee.PasswordHash = BCrypt.Net.BCrypt.HashPassword(employee.PasswordHash);
            }

            // Cập nhật các thông tin khác
            existingEmployee.EmployeeCode = employee.EmployeeCode;
            existingEmployee.Name = employee.Name;
            existingEmployee.Department = employee.Department;
            existingEmployee.Position = employee.Position;
            existingEmployee.Email = employee.Email;
            existingEmployee.LateDays = employee.LateDays;
            existingEmployee.LeaveDays = employee.LeaveDays;
            existingEmployee.IsManager = employee.IsManager;

            _context.Entry(existingEmployee).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return Ok(employee);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}