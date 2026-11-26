using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AntiScam.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
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
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AvatarUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TelegramChatId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                name: "EmergencyContacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProvinceCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    ProvinceName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsNational = table.Column<bool>(type: "bit", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerificationSource = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmergencyContacts", x => x.Id);
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
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Action = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EntityId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    OldValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ModelVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VersionTag = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VersionNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ModelType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TaskType = table.Column<int>(type: "int", nullable: false),
                    Accuracy = table.Column<double>(type: "float", nullable: false),
                    Precision = table.Column<double>(type: "float", nullable: false),
                    Recall = table.Column<double>(type: "float", nullable: false),
                    F1Score = table.Column<double>(type: "float", nullable: false),
                    Metrics = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ConfusionMatrix = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hyperparameters = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    TrainingSamples = table.Column<int>(type: "int", nullable: false),
                    TestSamples = table.Column<int>(type: "int", nullable: false),
                    DataBatch = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrainedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArtifactPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ModelFilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrainedById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModelVersions_AspNetUsers_TrainedById",
                        column: x => x.TrainedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PhoneReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportedNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NumberType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OwnerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    OwnerOrganization = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    PrimaryScamType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReportCount = table.Column<int>(type: "int", nullable: false),
                    UniqueReporterCount = table.Column<int>(type: "int", nullable: false),
                    FirstSeenAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VerificationStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VerifiedById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    RiskScore = table.Column<double>(type: "float", nullable: false),
                    RiskFactors = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhoneReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhoneReports_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PhoneReports_AspNetUsers_VerifiedById",
                        column: x => x.VerifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RelativeContacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    TelegramChatId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContactType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelativeContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RelativeContacts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScamScenarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ScamType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    RecognitionSigns = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnonymizedExample = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreventionAdvice = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    VideoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ParentScenarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScamScenarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScamScenarios_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ScamScenarios_ScamScenarios_ParentScenarioId",
                        column: x => x.ParentScenarioId,
                        principalTable: "ScamScenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TravelRoutes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WaypointsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeviationThresholdMeters = table.Column<double>(type: "float", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelRoutes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TravelRoutes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PhoneReportDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhoneReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportedById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ScamType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EvidenceUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsModerated = table.Column<bool>(type: "bit", nullable: false),
                    ModeratedById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ModeratedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    ModerationNote = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhoneReportDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhoneReportDetails_AspNetUsers_ModeratedById",
                        column: x => x.ModeratedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PhoneReportDetails_AspNetUsers_ReportedById",
                        column: x => x.ReportedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhoneReportDetails_PhoneReports_PhoneReportId",
                        column: x => x.PhoneReportId,
                        principalTable: "PhoneReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingDataSamples",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false),
                    IsScam = table.Column<bool>(type: "bit", nullable: false),
                    ScamType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BatchId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DataBatch = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LabeledById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    VerifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ScenarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingDataSamples", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingDataSamples_AspNetUsers_LabeledById",
                        column: x => x.LabeledById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TrainingDataSamples_ScamScenarios_ScenarioId",
                        column: x => x.ScenarioId,
                        principalTable: "ScamScenarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LocationLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TravelRouteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(10,7)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(10,7)", nullable: false),
                    Accuracy = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    Speed = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    Bearing = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DistanceFromRoute = table.Column<double>(type: "float", nullable: false),
                    IsDeviation = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationLogs_TravelRoutes_TravelRouteId",
                        column: x => x.TravelRouteId,
                        principalTable: "TravelRoutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RouteAlerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TravelRouteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AlertType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Resolution = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(10,7)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(10,7)", nullable: true),
                    CountdownSeconds = table.Column<int>(type: "int", nullable: false),
                    AcknowledgedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AcknowledgedById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    NotificationsSent = table.Column<bool>(type: "bit", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteAlerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RouteAlerts_AspNetUsers_AcknowledgedById",
                        column: x => x.AcknowledgedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RouteAlerts_TravelRoutes_TravelRouteId",
                        column: x => x.TravelRouteId,
                        principalTable: "TravelRoutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "EmergencyContacts",
                columns: new[] { "Id", "Category", "CreatedAt", "CreatedBy", "Description", "IsDeleted", "IsNational", "Name", "PhoneNumber", "ProvinceCode", "ProvinceName", "UpdatedAt", "UpdatedBy", "VerificationSource", "VerifiedAt" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), "Police", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Đường dây khẩn cấp cảnh sát toàn quốc", false, true, "Cảnh sát - Police", "113", null, null, new DateTime(2026, 11, 26, 5, 49, 42, 250, DateTimeKind.Utc).AddTicks(6960), null, null, null },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "Fire", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Đường dây khẩn cấp cứu hỏa toàn quốc", false, true, "Cứu hỏa - Fire Department", "114", null, null, new DateTime(2026, 11, 26, 5, 49, 42, 250, DateTimeKind.Utc).AddTicks(7119), null, null, null },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "Medical", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Đường dây khẩn cấp y tế toàn quốc", false, true, "Cấp cứu - Medical Emergency", "115", null, null, new DateTime(2026, 11, 26, 5, 49, 42, 250, DateTimeKind.Utc).AddTicks(7130), null, null, null }
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
                name: "IX_AspNetUsers_Role",
                table: "AspNetUsers",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TelegramChatId",
                table: "AspNetUsers",
                column: "TelegramChatId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Action",
                table: "AuditLogs",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_CreatedAt",
                table: "AuditLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityName",
                table: "AuditLogs",
                column: "EntityName");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyContacts_Category",
                table: "EmergencyContacts",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyContacts_IsNational",
                table: "EmergencyContacts",
                column: "IsNational");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyContacts_ProvinceCode",
                table: "EmergencyContacts",
                column: "ProvinceCode");

            migrationBuilder.CreateIndex(
                name: "IX_LocationLogs_RecordedAt",
                table: "LocationLogs",
                column: "RecordedAt");

            migrationBuilder.CreateIndex(
                name: "IX_LocationLogs_TravelRouteId_RecordedAt",
                table: "LocationLogs",
                columns: new[] { "TravelRouteId", "RecordedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ModelVersions_IsActive",
                table: "ModelVersions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ModelVersions_ModelType",
                table: "ModelVersions",
                column: "ModelType");

            migrationBuilder.CreateIndex(
                name: "IX_ModelVersions_ModelType_IsActive",
                table: "ModelVersions",
                columns: new[] { "ModelType", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_ModelVersions_TrainedById",
                table: "ModelVersions",
                column: "TrainedById");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneReportDetails_CreatedAt",
                table: "PhoneReportDetails",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneReportDetails_IsModerated",
                table: "PhoneReportDetails",
                column: "IsModerated");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneReportDetails_ModeratedById",
                table: "PhoneReportDetails",
                column: "ModeratedById");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneReportDetails_PhoneReportId",
                table: "PhoneReportDetails",
                column: "PhoneReportId");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneReportDetails_ReportedById",
                table: "PhoneReportDetails",
                column: "ReportedById");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneReports_ReportCount",
                table: "PhoneReports",
                column: "ReportCount");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneReports_ReportedNumber_NumberType",
                table: "PhoneReports",
                columns: new[] { "ReportedNumber", "NumberType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhoneReports_RiskScore",
                table: "PhoneReports",
                column: "RiskScore");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneReports_UserId",
                table: "PhoneReports",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneReports_VerificationStatus",
                table: "PhoneReports",
                column: "VerificationStatus");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneReports_VerifiedById",
                table: "PhoneReports",
                column: "VerifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_RelativeContacts_ContactType",
                table: "RelativeContacts",
                column: "ContactType");

            migrationBuilder.CreateIndex(
                name: "IX_RelativeContacts_UserId",
                table: "RelativeContacts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RouteAlerts_AcknowledgedById",
                table: "RouteAlerts",
                column: "AcknowledgedById");

            migrationBuilder.CreateIndex(
                name: "IX_RouteAlerts_AlertType",
                table: "RouteAlerts",
                column: "AlertType");

            migrationBuilder.CreateIndex(
                name: "IX_RouteAlerts_CreatedAt",
                table: "RouteAlerts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_RouteAlerts_Resolution",
                table: "RouteAlerts",
                column: "Resolution");

            migrationBuilder.CreateIndex(
                name: "IX_RouteAlerts_TravelRouteId",
                table: "RouteAlerts",
                column: "TravelRouteId");

            migrationBuilder.CreateIndex(
                name: "IX_ScamScenarios_CreatedAt",
                table: "ScamScenarios",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ScamScenarios_CreatedById",
                table: "ScamScenarios",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ScamScenarios_IsDeleted",
                table: "ScamScenarios",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ScamScenarios_ParentScenarioId",
                table: "ScamScenarios",
                column: "ParentScenarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ScamScenarios_ScamType",
                table: "ScamScenarios",
                column: "ScamType");

            migrationBuilder.CreateIndex(
                name: "IX_ScamScenarios_Status",
                table: "ScamScenarios",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingDataSamples_BatchId",
                table: "TrainingDataSamples",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingDataSamples_CreatedAt",
                table: "TrainingDataSamples",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingDataSamples_IsScam",
                table: "TrainingDataSamples",
                column: "IsScam");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingDataSamples_LabeledById",
                table: "TrainingDataSamples",
                column: "LabeledById");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingDataSamples_ScamType",
                table: "TrainingDataSamples",
                column: "ScamType");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingDataSamples_ScenarioId",
                table: "TrainingDataSamples",
                column: "ScenarioId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelRoutes_StartedAt",
                table: "TravelRoutes",
                column: "StartedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TravelRoutes_Status",
                table: "TravelRoutes",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TravelRoutes_UserId_IsActive",
                table: "TravelRoutes",
                columns: new[] { "UserId", "IsActive" });
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
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "EmergencyContacts");

            migrationBuilder.DropTable(
                name: "LocationLogs");

            migrationBuilder.DropTable(
                name: "ModelVersions");

            migrationBuilder.DropTable(
                name: "PhoneReportDetails");

            migrationBuilder.DropTable(
                name: "RelativeContacts");

            migrationBuilder.DropTable(
                name: "RouteAlerts");

            migrationBuilder.DropTable(
                name: "TrainingDataSamples");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "PhoneReports");

            migrationBuilder.DropTable(
                name: "TravelRoutes");

            migrationBuilder.DropTable(
                name: "ScamScenarios");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
