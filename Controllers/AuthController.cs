using Microsoft.AspNetCore.Mvc;
using ChamCongAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ChamCongAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Email == model.Email && e.PasswordHash == model.Password);

            if (employee == null)
            {
                return Unauthorized(new { message = "Email hoặc mật khẩu không đúng." });
            }

            return Ok(new { userId = employee.Id });
        }
    }

    public class LoginModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}