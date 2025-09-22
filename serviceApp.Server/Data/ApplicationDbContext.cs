using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Security.Claims;

namespace serviceApp.Server.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Guid? _familyId;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
                                IHttpContextAccessor httpContextAccessor)
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
        var fidStr = httpContextAccessor.HttpContext?.User?.FindFirstValue("fid");
        if (Guid.TryParse(fidStr, out var fid))
            _familyId = fid;
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

        // Multi-tenant query filters (deny-by-default when _familyId is null)
        modelBuilder.Entity<Vehicle>()
            .HasQueryFilter(v => _familyId != null && v.FamilyId == _familyId);

        modelBuilder.Entity<Owner>()
            .HasQueryFilter(o => _familyId != null && o.FamilyId == _familyId);

        modelBuilder.Entity<ConsumptionRecord>(b =>
        {
            b.HasQueryFilter(c => _familyId != null && c.FamilyId == _familyId);
            b.Property(c => c.DieselPricePerLiter).HasPrecision(18, 2);
            b.Property(c => c.DieselAdded).HasPrecision(18, 2);
        });

        modelBuilder.Entity<InsurancePolicy>(b =>
        {
            b.HasQueryFilter(i => _familyId != null && i.FamilyId == _familyId);
            b.Property(i => i.AnnualPrice).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Parts>(b =>
        {
            b.HasQueryFilter(p => _familyId != null && p.FamilyId == _familyId);
            b.Property(p => p.Price).HasPrecision(18, 2);
            b.HasIndex(p => new { p.FamilyId });
        });

        modelBuilder.Entity<VehicleInventory>(b =>
        {
            b.HasQueryFilter(v => _familyId != null && v.FamilyId == _familyId);
            b.Property(v => v.Cost).HasPrecision(18, 2);
            b.Property(v => v.QuantityInStock).HasPrecision(18, 2);
            b.Property(v => v.ReorderThreshold).HasPrecision(18, 2);
            b.HasIndex(v => new { v.FamilyId, v.VehicleId });
        });

        modelBuilder.Entity<ServiceRecord>(b =>
        {
            b.HasQueryFilter(s => _familyId != null && s.FamilyId == _familyId);
            b.Property(s => s.Cost).HasPrecision(18, 2);
            b.HasIndex(s => new { s.FamilyId, s.VehicleId });
        });

        modelBuilder.Entity<MileageHistory>()
            .HasQueryFilter(m => _familyId != null && m.FamilyId == _familyId);

        // Optional: additional helpful indexes
        modelBuilder.Entity<Supplier>().HasIndex(x => new { x.FamilyId });
        modelBuilder.Entity<ServiceType>().HasIndex(x => new { x.FamilyId });
        modelBuilder.Entity<ServiceCompany>().HasIndex(x => new { x.FamilyId });
        modelBuilder.Entity<Owner>().HasIndex(x => new { x.FamilyId });
        modelBuilder.Entity<Vehicle>().HasIndex(x => new { x.FamilyId });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Stamp FamilyId on inserts for all tracked entities that have a FamilyId property
        if (_familyId is Guid fid)
        {
            foreach (var entry in ChangeTracker.Entries()
                         .Where(e => e.State == EntityState.Added))
            {
                var familyProp = entry.Properties.FirstOrDefault(p => p.Metadata.Name == nameof(ServiceRecord.FamilyId));
                if (familyProp != null)
                {
                    var current = familyProp.CurrentValue as Guid?;
                    if (current is null || current == Guid.Empty)
                        familyProp.CurrentValue = fid;
                }
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
