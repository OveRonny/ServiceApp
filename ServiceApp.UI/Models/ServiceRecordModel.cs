namespace ServiceApp.UI.Models;

public class ServiceRecordModel
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public VehicleModel? Vehicle { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public DateTime ServiceDate { get; set; } = DateTime.Now;
    public int ServiceTypeId { get; set; }
    public ServiceTypeModel? ServiceType { get; set; }
    public int ServiceCompanyId { get; set; }

    public int MileageHistoryId { get; set; }
    public MileageHistoryModel MileageHistory { get; set; } = new();

    public ServiceCompanyModel? ServiceCompany { get; set; }

    public int? VehicleInventoryId { get; set; }
    public decimal QuantityUsed { get; set; } = 1m;

    public string? ServiceTypeName { get; set; } = string.Empty;
    public string? VehicleName { get; set; }
    public int Mileage { get; set; }
    public int? Hours { get; set; }

    public List<UsedPartModel> UsedParts { get; set; } = new();

    public List<PartsModel>? Parts { get; set; } = new();
}
