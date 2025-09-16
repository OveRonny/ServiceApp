namespace serviceApp.Server.Entities;

public class ServiceRecord
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public DateTime ServiceDate { get; set; } = DateTime.Now;
    public int ServiceTypeId { get; set; }
    public ServiceType? ServiceType { get; set; }
    public int ServiceCompanyId { get; set; }

    public int MileageHistoryId { get; set; }
    public Guid FamilyId { get; set; }


    public MileageHistory? MileageHistory { get; set; }

    public ServiceCompany? ServiceCompany { get; set; }

    public ICollection<Parts> Parts { get; set; } = new List<Parts>();
}
