namespace ChamCongAPI.Models { 
public class ChamCong
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }

    public DateTime CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public bool IsLate { get; set; }
    public double WorkHours { get; set; } // Số giờ làm việc
}
}