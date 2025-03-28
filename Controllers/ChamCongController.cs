using Microsoft.AspNetCore.Mvc;
using ChamCongAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ChamCongAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChamCongController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ChamCongController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("checkin")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInModel model)
        {
            var today = DateTime.Today;
            var existingRecord = await _context.ChamCongs
                .FirstOrDefaultAsync(c => c.EmployeeId == model.EmployeeId && c.CheckInTime.Date == today);

            if (existingRecord != null)
            {
                return BadRequest(new { message = "Bạn đã check-in hôm nay." });
            }

            var chamCong = new ChamCong
            {
                EmployeeId = model.EmployeeId,
                CheckInTime = DateTime.Now,
                IsLate = DateTime.Now.TimeOfDay > new TimeSpan(8, 0, 0) // Trễ nếu check-in sau 8:00 AM
            };

            _context.ChamCongs.Add(chamCong);
            await _context.SaveChangesAsync();
            return Ok(chamCong);
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> CheckOut([FromBody] CheckOutModel model)
        {
            var today = DateTime.Today;
            var record = await _context.ChamCongs
                .FirstOrDefaultAsync(c => c.EmployeeId == model.EmployeeId && c.CheckInTime.Date == today);

            if (record == null)
            {
                return BadRequest(new { message = "Bạn chưa check-in hôm nay." });
            }

            if (record.CheckOutTime.HasValue)
            {
                return BadRequest(new { message = "Bạn đã check-out hôm nay." });
            }

            record.CheckOutTime = DateTime.Now;
            await _context.SaveChangesAsync();
            return Ok(record);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory([FromQuery] int employeeId, [FromQuery] string filter = "month")
        {
            var now = DateTime.Now;
            DateTime startDate;

            switch (filter.ToLower())
            {
                case "day":
                    startDate = now.Date;
                    break;
                case "week":
                    startDate = now.AddDays(-(int)now.DayOfWeek);
                    break;
                case "month":
                default:
                    startDate = new DateTime(now.Year, now.Month, 1);
                    break;
            }

            var history = await _context.ChamCongs
                .Where(c => c.EmployeeId == employeeId && c.CheckInTime >= startDate)
                .OrderByDescending(c => c.CheckInTime)
                .ToListAsync();

            return Ok(history);
        }
    }

    public class CheckInModel
    {
        public int EmployeeId { get; set; }
    }

    public class CheckOutModel
    {
        public int EmployeeId { get; set; }
    }
}