namespace ChamCongAPI.Models
{
    public class ChamCong
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; } // Sửa từ "EmployeId" thành "EmployeeId"
        public DateTime CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public bool IsLate { get; set; }
    }
}