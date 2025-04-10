using Microsoft.EntityFrameworkCore;
using serviceApp.Server.Entities;

namespace serviceApp.Server.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Vehicle> Vehicles { get; set; }
}

