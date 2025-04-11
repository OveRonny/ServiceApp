namespace serviceApp.Server.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Owner> Owner { get; set; }
    public DbSet<InsurancePolicy> InsurancePolicies { get; set; }
    public DbSet<InsuranceHistory> InsuranceHistories { get; set; }
    public DbSet<MileageHistory> MileageHistories { get; set; }
    public DbSet<VehicleInventory> VehicleInventories { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Parts> Parts { get; set; }
    public DbSet<ConsumptionRecord> ConsumptionRecords { get; set; }
    public DbSet<ServiceType> ServiceTypes { get; set; }
    public DbSet<ServiceRecord> ServiceRecords { get; set; }
    public DbSet<ServiceCompany> ServiceCompanies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConsumptionRecord>()
           .Property(c => c.DieselPricePerLiter)
              .HasPrecision(9, 2);

        modelBuilder.Entity<ConsumptionRecord>()
              .Property(c => c.DieselAdded)
                  .HasPrecision(9, 2);

        modelBuilder.Entity<InsuranceHistory>()
            .Property(i => i.AnnualPrice)
            .HasPrecision(9, 2);

        modelBuilder.Entity<InsurancePolicy>()
            .Property(i => i.AnnualPrice)
            .HasPrecision(9, 2);

        modelBuilder.Entity<Parts>()
            .Property(p => p.Price)
            .HasPrecision(9, 2);

        modelBuilder.Entity<VehicleInventory>()
            .Property(v => v.Cost)
            .HasPrecision(9, 2);

        modelBuilder.Entity<ServiceRecord>()
            .Property(s => s.Cost)
            .HasPrecision(9, 2);

        modelBuilder.Entity<ConsumptionRecord>()
       .HasOne(c => c.MileageHistory)
       .WithMany()
       .HasForeignKey(c => c.MileageHistoryId)
       .OnDelete(DeleteBehavior.Cascade);
    }
}

