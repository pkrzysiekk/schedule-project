using App.Models;
using Microsoft.EntityFrameworkCore;

namespace Schedule_MVC.Models;

public class DbUpdate
{
    private readonly AppDbContext _context;
    public DbUpdate(AppDbContext context)
    {
        _context = context;
    }
    public async Task ClearDb()
    {
        try
        {
            var records = await _context.ScrapedData.ToListAsync();

            _context.ScrapedData.RemoveRange(records);

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Wystąpił błąd: {ex.Message}");
        }
    }
}
