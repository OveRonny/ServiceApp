using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace serviceApp.Server.Migrations;

/// <inheritdoc />
public partial class Init : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "AspNetRoles",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetRoles", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUsers",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                FamilyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PhoneNumber = table.Column<string>(type: "nvarchar(50)", nullable: true),
                PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                AccessFailedCount = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUsers", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Genres",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(100)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Genres", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ImageFiles",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Url = table.Column<string>(type: "nvarchar(500)", nullable: false),
                EntityType = table.Column<int>(type: "int", nullable: false),
                EntityId = table.Column<int>(type: "int", nullable: false),
                UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                IsPrimary = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ImageFiles", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Owner",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                FirstName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                LastName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                PhoneNumber = table.Column<string>(type: "nvarchar(50)", nullable: false),
                Email = table.Column<string>(type: "nvarchar(200)", nullable: false),
                Address = table.Column<string>(type: "nvarchar(200)", nullable: false),
                PostalCode = table.Column<string>(type: "nvarchar(20)", nullable: false),
                City = table.Column<string>(type: "nvarchar(100)", nullable: false),
                FamilyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Owner", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ServiceCompanies",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                FamilyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                Name = table.Column<string>(type: "nvarchar(100)", nullable: false)
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
                Name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                ContactEmail = table.Column<string>(type: "nvarchar(100)", nullable: false),
                ContactPhone = table.Column<string>(type: "nvarchar(50)", nullable: false),
                Address = table.Column<string>(type: "nvarchar(200)", nullable: false),
                City = table.Column<string>(type: "nvarchar(100)", nullable: false),
                PostalCode = table.Column<string>(type: "nvarchar(20)", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Suppliers", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AspNetRoleClaims",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                table.ForeignKey(
                    name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "AspNetRoles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUserClaims",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                table.ForeignKey(
                    name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUserLogins",
            columns: table => new
            {
                LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                table.ForeignKey(
                    name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUserRoles",
            columns: table => new
            {
                UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                table.ForeignKey(
                    name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "AspNetRoles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUserTokens",
            columns: table => new
            {
                UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                table.ForeignKey(
                    name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "MediaItems",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Title = table.Column<string>(type: "nvarchar(200)", nullable: false),
                Type = table.Column<int>(type: "int", nullable: false),
                DurationMinutes = table.Column<int>(type: "int", nullable: true),
                PosterPath = table.Column<string>(type: "nvarchar(500)", nullable: true),
                Seasons = table.Column<int>(type: "int", nullable: true),
                Episodes = table.Column<int>(type: "int", nullable: true),
                AverageEpisodeMinutes = table.Column<int>(type: "int", nullable: true),
                TmdbId = table.Column<int>(type: "int", nullable: false),
                ImdbId = table.Column<string>(type: "nvarchar(100)", nullable: true),
                ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MediaItems", x => x.Id);
                table.ForeignKey(
                    name: "FK_MediaItems_AspNetUsers_ApplicationUserId",
                    column: x => x.ApplicationUserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "Vehicles",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Make = table.Column<string>(type: "nvarchar(100)", nullable: false),
                Model = table.Column<string>(type: "nvarchar(100)", nullable: false),
                Year = table.Column<int>(type: "int", nullable: false),
                Color = table.Column<string>(type: "nvarchar(50)", nullable: false),
                LicensePlate = table.Column<string>(type: "nvarchar(50)", nullable: false),
                OwnerId = table.Column<int>(type: "int", nullable: false),
                DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                UserId = table.Column<string>(type: "nvarchar(500)", nullable: false),
                FamilyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Vehicles", x => x.Id);
                table.ForeignKey(
                    name: "FK_Vehicles_Owner_OwnerId",
                    column: x => x.OwnerId,
                    principalTable: "Owner",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.NoAction);
            });

        migrationBuilder.CreateTable(
            name: "MediaItemGenres",
            columns: table => new
            {
                MediaItemId = table.Column<int>(type: "int", nullable: false),
                GenreId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MediaItemGenres", x => new { x.MediaItemId, x.GenreId });
                table.ForeignKey(
                    name: "FK_MediaItemGenres_Genres_GenreId",
                    column: x => x.GenreId,
                    principalTable: "Genres",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_MediaItemGenres_MediaItems_MediaItemId",
                    column: x => x.MediaItemId,
                    principalTable: "MediaItems",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Seasons",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                SeasonNumber = table.Column<int>(type: "int", nullable: false),
                Name = table.Column<string>(type: "nvarchar(500)", nullable: false),
                Overview = table.Column<string>(type: "nvarchar(4000)", nullable: true),
                PosterPath = table.Column<string>(type: "nvarchar(500)", nullable: true),
                AirDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                VoteAverage = table.Column<double>(type: "float", nullable: true),
                EpisodeCount = table.Column<int>(type: "int", nullable: true),
                MediaItemId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Seasons", x => x.Id);
                table.ForeignKey(
                    name: "FK_Seasons_MediaItems_MediaItemId",
                    column: x => x.MediaItemId,
                    principalTable: "MediaItems",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "InsurancePolicies",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CompanyName = table.Column<string>(type: "nvarchar(500)", nullable: false),
                AnnualPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                TraficInsurancePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                AnnualMileageLimit = table.Column<int>(type: "int", nullable: false),
                VehicleId = table.Column<int>(type: "int", nullable: false),
                RenewalDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                StartingMileage = table.Column<int>(type: "int", nullable: false),
                FamilyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                EndDate = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_InsurancePolicies", x => x.Id);
                table.ForeignKey(
                    name: "FK_InsurancePolicies_Vehicles_VehicleId",
                    column: x => x.VehicleId,
                    principalTable: "Vehicles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
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
                Type = table.Column<int>(type: "int", nullable: false),
                FamilyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MileageHistories", x => x.Id);
                table.ForeignKey(
                    name: "FK_MileageHistories_Vehicles_VehicleId",
                    column: x => x.VehicleId,
                    principalTable: "Vehicles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "VehicleInventories",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                PartName = table.Column<string>(type: "nvarchar(500)", nullable: false),
                QuantityInStock = table.Column<int>(type: "int", precision: 18, scale: 2, nullable: true),
                ReorderThreshold = table.Column<int>(type: "int", precision: 18, scale: 2, nullable: true),
                Description = table.Column<string>(type: "nvarchar(4000)", nullable: false),
                Cost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                VehicleId = table.Column<int>(type: "int", nullable: false),
                SupplierId = table.Column<int>(type: "int", nullable: false),
                FamilyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_VehicleInventories", x => x.Id);
                table.ForeignKey(
                    name: "FK_VehicleInventories_Suppliers_SupplierId",
                    column: x => x.SupplierId,
                    principalTable: "Suppliers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_VehicleInventories_Vehicles_VehicleId",
                    column: x => x.VehicleId,
                    principalTable: "Vehicles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Episodes",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                EpisodeNumber = table.Column<int>(type: "int", nullable: false),
                Name = table.Column<string>(type: "nvarchar(500)", nullable: false),
                Overview = table.Column<string>(type: "nvarchar(4000)", nullable: true),
                AirDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                VoteAverage = table.Column<double>(type: "float", nullable: true),
                SeasonId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Episodes", x => x.Id);
                table.ForeignKey(
                    name: "FK_Episodes_Seasons_SeasonId",
                    column: x => x.SeasonId,
                    principalTable: "Seasons",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ConsumptionRecords",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                VehicleId = table.Column<int>(type: "int", nullable: false),
                Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                DieselAdded = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                DieselPricePerLiter = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                MileageHistoryId = table.Column<int>(type: "int", nullable: false),
                FamilyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                Description = table.Column<string>(type: "nvarchar(4000)", nullable: false),
                Cost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                ServiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                ServiceTypeId = table.Column<int>(type: "int", nullable: false),
                ServiceCompanyId = table.Column<int>(type: "int", nullable: false),
                MileageHistoryId = table.Column<int>(type: "int", nullable: false),
                FamilyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ServiceRecords", x => x.Id);
                table.ForeignKey(
                    name: "FK_ServiceRecords_MileageHistories_MileageHistoryId",
                    column: x => x.MileageHistoryId,
                    principalTable: "MileageHistories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ServiceRecords_ServiceCompanies_ServiceCompanyId",
                    column: x => x.ServiceCompanyId,
                    principalTable: "ServiceCompanies",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ServiceRecords_ServiceTypes_ServiceTypeId",
                    column: x => x.ServiceTypeId,
                    principalTable: "ServiceTypes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ServiceRecords_Vehicles_VehicleId",
                    column: x => x.VehicleId,
                    principalTable: "Vehicles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.NoAction);
            });

        migrationBuilder.CreateTable(
            name: "WatchHistories",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                MediaItemId = table.Column<int>(type: "int", nullable: false),
                WatchDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                SeasonId = table.Column<int>(type: "int", nullable: true),
                EpisodeId = table.Column<int>(type: "int", nullable: true),
                TimeSpentMinutes = table.Column<int>(type: "int", nullable: true),
                Progress = table.Column<double>(type: "float", nullable: false),
                UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Liked = table.Column<bool>(type: "bit", nullable: true),
                Rating = table.Column<int>(type: "int", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_WatchHistories", x => x.Id);
                table.ForeignKey(
                    name: "FK_WatchHistories_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_WatchHistories_Episodes_EpisodeId",
                    column: x => x.EpisodeId,
                    principalTable: "Episodes",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_WatchHistories_MediaItems_MediaItemId",
                    column: x => x.MediaItemId,
                    principalTable: "MediaItems",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_WatchHistories_Seasons_SeasonId",
                    column: x => x.SeasonId,
                    principalTable: "Seasons",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "Parts",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                Quantity = table.Column<int>(type: "int", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ServiceRecordId = table.Column<int>(type: "int", nullable: false),
                VehicleInventoryId = table.Column<int>(type: "int", nullable: true),
                FamilyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Parts", x => x.Id);
                table.ForeignKey(
                    name: "FK_Parts_ServiceRecords_ServiceRecordId",
                    column: x => x.ServiceRecordId,
                    principalTable: "ServiceRecords",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Parts_VehicleInventories_VehicleInventoryId",
                    column: x => x.VehicleInventoryId,
                    principalTable: "VehicleInventories",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateIndex(
            name: "IX_AspNetRoleClaims_RoleId",
            table: "AspNetRoleClaims",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            name: "RoleNameIndex",
            table: "AspNetRoles",
            column: "NormalizedName",
            unique: true,
            filter: "[NormalizedName] IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUserClaims_UserId",
            table: "AspNetUserClaims",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUserLogins_UserId",
            table: "AspNetUserLogins",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUserRoles_RoleId",
            table: "AspNetUserRoles",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            name: "EmailIndex",
            table: "AspNetUsers",
            column: "NormalizedEmail");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUsers_FamilyId",
            table: "AspNetUsers",
            column: "FamilyId");

        migrationBuilder.CreateIndex(
            name: "UserNameIndex",
            table: "AspNetUsers",
            column: "NormalizedUserName",
            unique: true,
            filter: "[NormalizedUserName] IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_ConsumptionRecords_MileageHistoryId",
            table: "ConsumptionRecords",
            column: "MileageHistoryId");

        migrationBuilder.CreateIndex(
            name: "IX_ConsumptionRecords_VehicleId",
            table: "ConsumptionRecords",
            column: "VehicleId");

        migrationBuilder.CreateIndex(
            name: "IX_Episodes_SeasonId",
            table: "Episodes",
            column: "SeasonId");

        migrationBuilder.CreateIndex(
            name: "IX_Genres_Name",
            table: "Genres",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_InsurancePolicies_VehicleId",
            table: "InsurancePolicies",
            column: "VehicleId");

        migrationBuilder.CreateIndex(
            name: "IX_MediaItemGenres_GenreId",
            table: "MediaItemGenres",
            column: "GenreId");

        migrationBuilder.CreateIndex(
            name: "IX_MediaItems_ApplicationUserId",
            table: "MediaItems",
            column: "ApplicationUserId");

        migrationBuilder.CreateIndex(
            name: "IX_MediaItems_TmdbId_Type",
            table: "MediaItems",
            columns: new[] { "TmdbId", "Type" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_MileageHistories_VehicleId",
            table: "MileageHistories",
            column: "VehicleId");

        migrationBuilder.CreateIndex(
            name: "IX_Owner_FamilyId",
            table: "Owner",
            column: "FamilyId");

        migrationBuilder.CreateIndex(
            name: "IX_Parts_FamilyId",
            table: "Parts",
            column: "FamilyId");

        migrationBuilder.CreateIndex(
            name: "IX_Parts_ServiceRecordId",
            table: "Parts",
            column: "ServiceRecordId");

        migrationBuilder.CreateIndex(
            name: "IX_Parts_VehicleInventoryId",
            table: "Parts",
            column: "VehicleInventoryId");

        migrationBuilder.CreateIndex(
            name: "IX_Seasons_MediaItemId_SeasonNumber",
            table: "Seasons",
            columns: new[] { "MediaItemId", "SeasonNumber" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_ServiceCompanies_FamilyId",
            table: "ServiceCompanies",
            column: "FamilyId");

        migrationBuilder.CreateIndex(
            name: "IX_ServiceRecords_FamilyId_VehicleId",
            table: "ServiceRecords",
            columns: new[] { "FamilyId", "VehicleId" });

        migrationBuilder.CreateIndex(
            name: "IX_ServiceRecords_MileageHistoryId",
            table: "ServiceRecords",
            column: "MileageHistoryId");

        migrationBuilder.CreateIndex(
            name: "IX_ServiceRecords_ServiceCompanyId",
            table: "ServiceRecords",
            column: "ServiceCompanyId");

        migrationBuilder.CreateIndex(
            name: "IX_ServiceRecords_ServiceTypeId",
            table: "ServiceRecords",
            column: "ServiceTypeId");

        migrationBuilder.CreateIndex(
            name: "IX_ServiceRecords_VehicleId",
            table: "ServiceRecords",
            column: "VehicleId");

        migrationBuilder.CreateIndex(
            name: "IX_VehicleInventories_FamilyId_VehicleId",
            table: "VehicleInventories",
            columns: new[] { "FamilyId", "VehicleId" });

        migrationBuilder.CreateIndex(
            name: "IX_VehicleInventories_SupplierId",
            table: "VehicleInventories",
            column: "SupplierId");

        migrationBuilder.CreateIndex(
            name: "IX_VehicleInventories_VehicleId",
            table: "VehicleInventories",
            column: "VehicleId");

        migrationBuilder.CreateIndex(
            name: "IX_Vehicles_FamilyId",
            table: "Vehicles",
            column: "FamilyId");

        migrationBuilder.CreateIndex(
            name: "IX_Vehicles_OwnerId",
            table: "Vehicles",
            column: "OwnerId");

        migrationBuilder.CreateIndex(
            name: "IX_WatchHistories_EpisodeId",
            table: "WatchHistories",
            column: "EpisodeId");

        migrationBuilder.CreateIndex(
            name: "IX_WatchHistories_MediaItemId",
            table: "WatchHistories",
            column: "MediaItemId");

        migrationBuilder.CreateIndex(
            name: "IX_WatchHistories_SeasonId",
            table: "WatchHistories",
            column: "SeasonId");

        migrationBuilder.CreateIndex(
            name: "IX_WatchHistories_UserId_MediaItemId_SeasonId_EpisodeId",
            table: "WatchHistories",
            columns: new[] { "UserId", "MediaItemId", "SeasonId", "EpisodeId" },
            unique: true,
            filter: "[SeasonId] IS NOT NULL AND [EpisodeId] IS NOT NULL");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "AspNetRoleClaims");

        migrationBuilder.DropTable(
            name: "AspNetUserClaims");

        migrationBuilder.DropTable(
            name: "AspNetUserLogins");

        migrationBuilder.DropTable(
            name: "AspNetUserRoles");

        migrationBuilder.DropTable(
            name: "AspNetUserTokens");

        migrationBuilder.DropTable(
            name: "ConsumptionRecords");

        migrationBuilder.DropTable(
            name: "ImageFiles");

        migrationBuilder.DropTable(
            name: "InsurancePolicies");

        migrationBuilder.DropTable(
            name: "MediaItemGenres");

        migrationBuilder.DropTable(
            name: "Parts");

        migrationBuilder.DropTable(
            name: "WatchHistories");

        migrationBuilder.DropTable(
            name: "AspNetRoles");

        migrationBuilder.DropTable(
            name: "Genres");

        migrationBuilder.DropTable(
            name: "ServiceRecords");

        migrationBuilder.DropTable(
            name: "VehicleInventories");

        migrationBuilder.DropTable(
            name: "Episodes");

        migrationBuilder.DropTable(
            name: "MileageHistories");

        migrationBuilder.DropTable(
            name: "ServiceCompanies");

        migrationBuilder.DropTable(
            name: "ServiceTypes");

        migrationBuilder.DropTable(
            name: "Suppliers");

        migrationBuilder.DropTable(
            name: "Seasons");

        migrationBuilder.DropTable(
            name: "Vehicles");

        migrationBuilder.DropTable(
            name: "MediaItems");

        migrationBuilder.DropTable(
            name: "Owner");

        migrationBuilder.DropTable(
            name: "AspNetUsers");
    }
}
