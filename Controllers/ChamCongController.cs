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

        // Check-in
        [HttpPost("checkin")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInModel model)
        {
            var today = DateTime.Today;
            var now = DateTime.Now;

            var existingRecord = await _context.ChamCongs
                .FirstOrDefaultAsync(c => c.EmployeeId == model.EmployeeId && c.CheckInTime.Date == today);

            if (existingRecord != null)
            {
                return BadRequest(new { message = "Bạn đã check-in hôm nay." });
            }

            var chamCong = new ChamCong
            {
                EmployeeId = model.EmployeeId,
                CheckInTime = now,
                IsLate = now.TimeOfDay > new TimeSpan(8, 0, 0) // Trễ nếu check-in sau 8:00 AM
            };

            _context.ChamCongs.Add(chamCong);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Check-in thành công", chamCong });
        }

        // Check-out
        [HttpPost("checkout")]
        public async Task<IActionResult> CheckOut([FromBody] CheckOutModel model)
        {
            var today = DateTime.Today;
            var now = DateTime.Now;

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

            record.CheckOutTime = now;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Check-out thành công", record });
        }

        // Lấy lịch sử chấm công
        // Lấy lịch sử chấm công
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
                    startDate = now.AddDays(-((int)now.DayOfWeek - 1)); // Lấy thứ Hai đầu tuần
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

            var result = history.Select(c => new
            {
                c.EmployeeId,
                c.CheckInTime,
                c.CheckOutTime,
                WorkHours = c.CheckOutTime.HasValue
                    ? (c.CheckOutTime.Value - c.CheckInTime).TotalHours // Tính giờ công nếu đã check-out
                    : (now - c.CheckInTime).TotalHours // Tính giờ công nếu chưa check-out
            }).ToList();

            return Ok(result);
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
