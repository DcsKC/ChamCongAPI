using Microsoft.EntityFrameworkCore;
using ChamCongAPI.Models;

namespace ChamCongAPI
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<ChamCong> ChamCongs { get; set; }
    }
}