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

    // Helper method for query filters
    private bool FamilyFilter(Guid? entityFamilyId) =>
        _httpContextAccessor.HttpContext == null || (CurrentFamilyId != null && entityFamilyId == CurrentFamilyId);

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

        // Vehicles
        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasQueryFilter(v => FamilyFilter(v.FamilyId));
        });

        // Owners
        modelBuilder.Entity<Owner>(entity =>
        {
            entity.HasQueryFilter(o => FamilyFilter(o.FamilyId));
        });

        // MileageHistories
        modelBuilder.Entity<MileageHistory>(entity =>
        {
            entity.HasQueryFilter(m => FamilyFilter(m.FamilyId));
        });

        // ConsumptionRecords
        modelBuilder.Entity<ConsumptionRecord>(entity =>
        {
            entity.HasQueryFilter(c => FamilyFilter(c.FamilyId));
            entity.Property(c => c.DieselPricePerLiter).HasPrecision(9, 2);
            entity.Property(c => c.DieselAdded).HasPrecision(9, 2);

            entity.HasOne(c => c.MileageHistory)
                  .WithMany()
                  .HasForeignKey(c => c.MileageHistoryId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // InsurancePolicies
        modelBuilder.Entity<InsurancePolicy>(entity =>
        {
            entity.HasQueryFilter(i => FamilyFilter(i.FamilyId));
            entity.Property(i => i.AnnualPrice).HasPrecision(9, 2);
        });

        // Parts
        modelBuilder.Entity<Parts>(entity =>
        {
            entity.HasQueryFilter(p => FamilyFilter(p.FamilyId));
            entity.Property(p => p.Price).HasPrecision(9, 2);
        });

        // VehicleInventories
        modelBuilder.Entity<VehicleInventory>(entity =>
        {
            entity.HasQueryFilter(v => FamilyFilter(v.FamilyId));
            entity.Property(v => v.Cost).HasPrecision(9, 2);
        });

        // ServiceRecords
        modelBuilder.Entity<ServiceRecord>(entity =>
        {
            entity.HasQueryFilter(s => FamilyFilter(s.FamilyId));
            entity.Property(s => s.Cost).HasPrecision(9, 2);
        });

        // ServiceTypes
        modelBuilder.Entity<ServiceType>(entity =>
        {
            entity.HasQueryFilter(s => FamilyFilter(s.FamilyId));
        });

        // ServiceCompanies
        modelBuilder.Entity<ServiceCompany>(entity =>
        {
            entity.HasQueryFilter(s => FamilyFilter(s.FamilyId));
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var fid = CurrentFamilyId;
        if (fid != null)
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