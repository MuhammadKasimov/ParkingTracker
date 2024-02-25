using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Reflection;

namespace DahuaTracker.Contexts
{
    public class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            string folder = Path.GetDirectoryName(assemblyPath);
            string dbPath = Path.Combine(folder, "DahuaDb.sqlite");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
        public DbSet<EventInfo> EventInfos { get; set; }
        public DbSet<CameraCredentials> CameraCredentials { get; set; }
    }
}
