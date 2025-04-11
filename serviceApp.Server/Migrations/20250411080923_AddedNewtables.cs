using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace serviceApp.Server.Migrations;

/// <inheritdoc />
public partial class AddedNewtables : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "InsurancePolicies",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CompanyName = table.Column<string>(type: "nvarchar(50)", nullable: false),
                AnnualPrice = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: false),
                AnnualMileageLimit = table.Column<int>(type: "int", nullable: false),
                VehicleId = table.Column<int>(type: "int", nullable: false),
                RenewalDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                StartingMileage = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_InsurancePolicies", x => x.Id);
                table.ForeignKey(
                    name: "FK_InsurancePolicies_Vehicles_VehicleId",
                    column: x => x.VehicleId,
                    principalTable: "Vehicles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.NoAction);
            });

        migrationBuilder.CreateTable(
            name: "MileageHistories",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                VehicleId = table.Column<int>(type: "int", nullable: false),
                Mileage = table.Column<int>(type: "int", nullable: false),
                Hours = table.Column<int>(type: "int", nullable: true),
                RecordedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                Type = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MileageHistories", x => x.Id);
                table.ForeignKey(
                    name: "FK_MileageHistories_Vehicles_VehicleId",
                    column: x => x.VehicleId,
                    principalTable: "Vehicles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.NoAction);
            });

        migrationBuilder.CreateTable(
            name: "ServiceCompanies",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(50)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ServiceCompanies", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ServiceTypes",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(50)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ServiceTypes", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Suppliers",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(50)", nullable: false),
                ContactEmail = table.Column<string>(type: "nvarchar(50)", nullable: false),
                ContactPhone = table.Column<string>(type: "nvarchar(50)", nullable: false),
                Address = table.Column<string>(type: "nvarchar(50)", nullable: false),
                City = table.Column<string>(type: "nvarchar(50)", nullable: false),
                PostalCode = table.Column<string>(type: "nvarchar(50)", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Suppliers", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "InsuranceHistories",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                VehicleId = table.Column<int>(type: "int", nullable: false),
                CompanyName = table.Column<string>(type: "nvarchar(50)", nullable: false),
                AnnualPrice = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: false),
                AnnualMileageLimit = table.Column<int>(type: "int", nullable: false),
                StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                InsurancePolicyId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_InsuranceHistories", x => x.Id);
                table.ForeignKey(
                    name: "FK_InsuranceHistories_InsurancePolicies_InsurancePolicyId",
                    column: x => x.InsurancePolicyId,
                    principalTable: "InsurancePolicies",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_InsuranceHistories_Vehicles_VehicleId",
                    column: x => x.VehicleId,
                    principalTable: "Vehicles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.NoAction);
            });

        migrationBuilder.CreateTable(
            name: "ConsumptionRecords",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                VehicleId = table.Column<int>(type: "int", nullable: false),
                Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                DieselAdded = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: false),
                DieselPricePerLiter = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: false),
                MileageHistoryId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ConsumptionRecords", x => x.Id);
                table.ForeignKey(
                    name: "FK_ConsumptionRecords_MileageHistories_MileageHistoryId",
                    column: x => x.MileageHistoryId,
                    principalTable: "MileageHistories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.NoAction);
                table.ForeignKey(
                    name: "FK_ConsumptionRecords_Vehicles_VehicleId",
                    column: x => x.VehicleId,
                    principalTable: "Vehicles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.NoAction);
            });

        migrationBuilder.CreateTable(
            name: "ServiceRecords",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                VehicleId = table.Column<int>(type: "int", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Cost = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: false),
                ServiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                ServiceTypeId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ServiceRecords", x => x.Id);
                table.ForeignKey(
                    name: "FK_ServiceRecords_ServiceTypes_ServiceTypeId",
                    column: x => x.ServiceTypeId,
                    principalTable: "ServiceTypes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.NoAction);
                table.ForeignKey(
                    name: "FK_ServiceRecords_Vehicles_VehicleId",
                    column: x => x.VehicleId,
                    principalTable: "Vehicles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.NoAction);
            });

        migrationBuilder.CreateTable(
            name: "VehicleInventories",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                PartName = table.Column<string>(type: "nvarchar(50)", nullable: false),
                QuantityInStock = table.Column<int>(type: "int", nullable: true),
                ReorderThreshold = table.Column<int>(type: "int", nullable: true),
                Cost = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: false),
                PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                VehicleId = table.Column<int>(type: "int", nullable: false),
                SupplierId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_VehicleInventories", x => x.Id);
                table.ForeignKey(
                    name: "FK_VehicleInventories_Suppliers_SupplierId",
                    column: x => x.SupplierId,
                    principalTable: "Suppliers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.NoAction);
                table.ForeignKey(
                    name: "FK_VehicleInventories_Vehicles_VehicleId",
                    column: x => x.VehicleId,
                    principalTable: "Vehicles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.NoAction);
            });

        migrationBuilder.CreateTable(
            name: "Parts",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(50)", nullable: false),
                Price = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: true),
                Quantity = table.Column<int>(type: "int", nullable: false),
                Description = table.Column<string>(type: "nvarchar(200)", nullable: false),
                ServiceRecordId = table.Column<int>(type: "int", nullable: false),
                VehicleInventoryId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Parts", x => x.Id);
                table.ForeignKey(
                    name: "FK_Parts_ServiceRecords_ServiceRecordId",
                    column: x => x.ServiceRecordId,
                    principalTable: "ServiceRecords",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.NoAction);
                table.ForeignKey(
                    name: "FK_Parts_VehicleInventories_VehicleInventoryId",
                    column: x => x.VehicleInventoryId,
                    principalTable: "VehicleInventories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ConsumptionRecords_MileageHistoryId",
            table: "ConsumptionRecords",
            column: "MileageHistoryId");

        migrationBuilder.CreateIndex(
            name: "IX_ConsumptionRecords_VehicleId",
            table: "ConsumptionRecords",
            column: "VehicleId");

        migrationBuilder.CreateIndex(
            name: "IX_InsuranceHistories_InsurancePolicyId",
            table: "InsuranceHistories",
            column: "InsurancePolicyId");

        migrationBuilder.CreateIndex(
            name: "IX_InsuranceHistories_VehicleId",
            table: "InsuranceHistories",
            column: "VehicleId");

        migrationBuilder.CreateIndex(
            name: "IX_InsurancePolicies_VehicleId",
            table: "InsurancePolicies",
            column: "VehicleId");

        migrationBuilder.CreateIndex(
            name: "IX_MileageHistories_VehicleId",
            table: "MileageHistories",
            column: "VehicleId");

        migrationBuilder.CreateIndex(
            name: "IX_Parts_ServiceRecordId",
            table: "Parts",
            column: "ServiceRecordId");

        migrationBuilder.CreateIndex(
            name: "IX_Parts_VehicleInventoryId",
            table: "Parts",
            column: "VehicleInventoryId");

        migrationBuilder.CreateIndex(
            name: "IX_ServiceRecords_ServiceTypeId",
            table: "ServiceRecords",
            column: "ServiceTypeId");

        migrationBuilder.CreateIndex(
            name: "IX_ServiceRecords_VehicleId",
            table: "ServiceRecords",
            column: "VehicleId");

        migrationBuilder.CreateIndex(
            name: "IX_VehicleInventories_SupplierId",
            table: "VehicleInventories",
            column: "SupplierId");

        migrationBuilder.CreateIndex(
            name: "IX_VehicleInventories_VehicleId",
            table: "VehicleInventories",
            column: "VehicleId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ConsumptionRecords");

        migrationBuilder.DropTable(
            name: "InsuranceHistories");

        migrationBuilder.DropTable(
            name: "Parts");

        migrationBuilder.DropTable(
            name: "ServiceCompanies");

        migrationBuilder.DropTable(
            name: "MileageHistories");

        migrationBuilder.DropTable(
            name: "InsurancePolicies");

        migrationBuilder.DropTable(
            name: "ServiceRecords");

        migrationBuilder.DropTable(
            name: "VehicleInventories");

        migrationBuilder.DropTable(
            name: "ServiceTypes");

        migrationBuilder.DropTable(
            name: "Suppliers");
    }
}
