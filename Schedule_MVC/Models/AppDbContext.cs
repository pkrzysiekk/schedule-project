using Microsoft.EntityFrameworkCore;

namespace Schedule_MVC.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<ScrapedData> ScrapedData { get; set; }

}
