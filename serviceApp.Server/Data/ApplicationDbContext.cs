using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Security.Claims;

namespace serviceApp.Server.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
                              IHttpContextAccessor httpContextAccessor)
      : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private Guid? CurrentFamilyId
    {
        get
        {
            var fidStr = _httpContextAccessor.HttpContext?.User?.FindFirstValue("fid");
            return Guid.TryParse(fidStr, out var fid) ? fid : null;
        }
    }

    public DbSet<Vehicle> Vehicles { get; set; }

    public DbSet<Owner> Owner { get; set; }
    public DbSet<InsurancePolicy> InsurancePolicies { get; set; }
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
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Vehicle>()
            .HasQueryFilter(v => CurrentFamilyId != null && v.FamilyId == CurrentFamilyId);

        modelBuilder.Entity<Owner>()
            .HasQueryFilter(o => CurrentFamilyId != null && o.FamilyId == CurrentFamilyId);

        modelBuilder.Entity<ConsumptionRecord>()
            .HasQueryFilter(c => CurrentFamilyId != null && c.FamilyId == CurrentFamilyId)
           .Property(c => c.DieselPricePerLiter)
              .HasPrecision(9, 2);


        modelBuilder.Entity<ConsumptionRecord>()
               .HasQueryFilter(c => CurrentFamilyId != null && c.FamilyId == CurrentFamilyId)
              .Property(c => c.DieselAdded)
                  .HasPrecision(9, 2);

        modelBuilder.Entity<InsurancePolicy>()
            .HasQueryFilter(c => CurrentFamilyId != null && c.FamilyId == CurrentFamilyId)
            .Property(i => i.AnnualPrice)
            .HasPrecision(9, 2);

        modelBuilder.Entity<Parts>()
            .HasQueryFilter(c => CurrentFamilyId != null && c.FamilyId == CurrentFamilyId)
            .Property(p => p.Price)
            .HasPrecision(9, 2);

        modelBuilder.Entity<VehicleInventory>()
            .HasQueryFilter(c => CurrentFamilyId != null && c.FamilyId == CurrentFamilyId)
            .Property(v => v.Cost)
            .HasPrecision(9, 2);

        modelBuilder.Entity<ServiceRecord>()
            .HasQueryFilter(c => CurrentFamilyId != null && c.FamilyId == CurrentFamilyId)
            .Property(s => s.Cost)
            .HasPrecision(9, 2);

        modelBuilder.Entity<ConsumptionRecord>()
            .HasQueryFilter(c => CurrentFamilyId != null && c.FamilyId == CurrentFamilyId)
           .HasOne(c => c.MileageHistory)
           .WithMany()
           .HasForeignKey(c => c.MileageHistoryId)
           .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MileageHistory>()
            .HasQueryFilter(m => CurrentFamilyId != null && m.FamilyId == CurrentFamilyId);

    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var fid = CurrentFamilyId;
        if (fid is not null)
        {
            foreach (var e in ChangeTracker.Entries<Vehicle>().Where(e => e.State == EntityState.Added))
            {
                if (e.Entity.FamilyId == Guid.Empty)
                    e.Entity.FamilyId = fid.Value;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}

