using ChamCongAPI.Models;
using System.Text.Json.Serialization;

public class Employee
{
    public int Id { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Rules { get; set; } = "Employee";

    [JsonIgnore] // Ngăn việc serialize danh sách chấm công
    public ICollection<ChamCong> ChamCongs { get; set; } = new List<ChamCong>();

}
