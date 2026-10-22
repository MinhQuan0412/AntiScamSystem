using AntiScam.Domain.Entities;
using AntiScam.Domain.Enums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AntiScam.Infrastructure.Data;

/// <summary>
/// Primary EF Core database context for the Anti-Scam Warning System.
/// Extends <see cref="IdentityDbContext{TUser}"/> to incorporate ASP.NET Core Identity tables.
/// </summary>
public sealed class ApplicationDbContext : IdentityDbContext<User>
{
    // ─────────────────────────────────── DbSets ──────────────────────────────────

    /// <summary>Scam scenario knowledge base.</summary>
    public DbSet<ScamScenario> ScamScenarios => Set<ScamScenario>();

    /// <summary>Labelled samples used to train / evaluate ML models.</summary>
    public DbSet<TrainingDataSample> TrainingDataSamples => Set<TrainingDataSample>();

    /// <summary>Registered ML model versions.</summary>
    public DbSet<ModelVersion> ModelVersions => Set<ModelVersion>();

    /// <summary>Community-submitted phone number reports.</summary>
    public DbSet<PhoneReport> PhoneReports => Set<PhoneReport>();

    /// <summary>Individual user comments / details attached to a phone report.</summary>
    public DbSet<PhoneReportDetail> PhoneReportDetails => Set<PhoneReportDetail>();

    /// <summary>National / regional emergency hotline contacts.</summary>
    public DbSet<EmergencyContact> EmergencyContacts => Set<EmergencyContact>();

    /// <summary>User-defined travel routes.</summary>
    public DbSet<TravelRoute> TravelRoutes => Set<TravelRoute>();

    /// <summary>GPS location logs recorded along a travel route.</summary>
    public DbSet<LocationLog> LocationLogs => Set<LocationLog>();

    /// <summary>Alerts raised during a travel route (deviation, SOS, etc.).</summary>
    public DbSet<RouteAlert> RouteAlerts => Set<RouteAlert>();

    /// <summary>Application audit / activity logs.</summary>
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    /// <summary>Trusted relative / guardian contacts linked to a user.</summary>
    public DbSet<RelativeContact> RelativeContacts => Set<RelativeContact>();

    // ────────────────────────────────── ctor ─────────────────────────────────────

    /// <inheritdoc />
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    // ─────────────────────────────── Model Building ──────────────────────────────

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // identity tables

        // ── User ──────────────────────────────────────────────────────────────────
        builder.Entity<User>(e =>
        {
            e.Property(u => u.FullName).HasMaxLength(200).IsRequired();
            e.Property(u => u.Role).HasConversion<string>().HasMaxLength(50).IsRequired();
            e.Property(u => u.AvatarUrl).HasMaxLength(500);
            e.Property(u => u.TelegramChatId).HasMaxLength(100);
            e.Property(u => u.CreatedAt).IsRequired();
            e.HasIndex(u => u.TelegramChatId);
            e.HasIndex(u => u.Role);
        });

        // ── ScamScenario ─────────────────────────────────────────────────────────
        builder.Entity<ScamScenario>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.Title).HasMaxLength(300).IsRequired();
            e.Property(s => s.Description).HasMaxLength(4000).IsRequired();
            e.Property(s => s.ScamType).HasConversion<string>().HasMaxLength(50).IsRequired();
            e.Property(s => s.Status).HasConversion<string>().HasMaxLength(50).IsRequired();
            e.Property(s => s.CreatedById).HasMaxLength(450);
            e.Property(s => s.Tags).HasMaxLength(1000);
            e.Property(s => s.ImageUrl).HasMaxLength(500);
            e.Property(s => s.VideoUrl).HasMaxLength(500);

            e.HasIndex(s => s.ScamType);
            e.HasIndex(s => s.Status);
            e.HasIndex(s => s.IsDeleted);
            e.HasIndex(s => s.CreatedAt);

            // self-referencing: related scenarios
            e.HasMany(s => s.RelatedScenarios)
             .WithOne()
             .HasForeignKey(s => s.ParentScenarioId)
             .IsRequired(false)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(s => s.CreatedBy)
             .WithMany()
             .HasForeignKey(s => s.CreatedById)
             .IsRequired(false)
             .OnDelete(DeleteBehavior.SetNull);

            e.HasQueryFilter(s => !s.IsDeleted);
        });

        // ── TrainingDataSample ───────────────────────────────────────────────────
        builder.Entity<TrainingDataSample>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Content).HasMaxLength(8000).IsRequired();
            e.Property(t => t.Source).HasMaxLength(200);
            e.Property(t => t.ScamType).HasConversion<string>().HasMaxLength(50);
            e.Property(t => t.LabeledById).HasMaxLength(450);
            e.Property(t => t.BatchId).HasMaxLength(100);

            e.HasIndex(t => t.IsScam);
            e.HasIndex(t => t.ScamType);
            e.HasIndex(t => t.BatchId);
            e.HasIndex(t => t.CreatedAt);

            e.HasOne(t => t.LabeledBy)
             .WithMany()
             .HasForeignKey(t => t.LabeledById)
             .IsRequired(false)
             .OnDelete(DeleteBehavior.SetNull);
        });

        // ── ModelVersion ─────────────────────────────────────────────────────────
        builder.Entity<ModelVersion>(e =>
        {
            e.HasKey(m => m.Id);
            e.Property(m => m.VersionTag).HasMaxLength(50).IsRequired();
            e.Property(m => m.Description).HasMaxLength(500);
            e.Property(m => m.ModelType).HasConversion<string>().HasMaxLength(50).IsRequired();
            e.Property(m => m.ArtifactPath).HasMaxLength(500);
            e.Property(m => m.TrainedById).HasMaxLength(450);
            e.Property(m => m.Hyperparameters).HasMaxLength(4000);
            e.Property(m => m.Metrics).HasMaxLength(4000);

            e.HasIndex(m => m.IsActive);
            e.HasIndex(m => m.ModelType);
            e.HasIndex(m => new { m.ModelType, m.IsActive });

            e.HasOne(m => m.TrainedBy)
             .WithMany()
             .HasForeignKey(m => m.TrainedById)
             .IsRequired(false)
             .OnDelete(DeleteBehavior.SetNull);
        });

        // ── PhoneReport ──────────────────────────────────────────────────────────
        builder.Entity<PhoneReport>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.ReportedNumber).HasMaxLength(20).IsRequired();
            e.Property(p => p.NumberType).HasConversion<string>().HasMaxLength(50).IsRequired();
            e.Property(p => p.VerificationStatus).HasConversion<string>().HasMaxLength(50).IsRequired();
            e.Property(p => p.OwnerName).HasMaxLength(200);
            e.Property(p => p.OwnerOrganization).HasMaxLength(300);
            e.Property(p => p.PrimaryScamType).HasConversion<string>().HasMaxLength(50);
            e.Property(p => p.VerifiedById).HasMaxLength(450);
            e.Property(p => p.Tags).HasMaxLength(500);

            // unique composite index – one aggregate record per number+type
            e.HasIndex(p => new { p.ReportedNumber, p.NumberType }).IsUnique();
            e.HasIndex(p => p.RiskScore);
            e.HasIndex(p => p.VerificationStatus);
            e.HasIndex(p => p.ReportCount);

            e.HasOne(p => p.VerifiedBy)
             .WithMany()
             .HasForeignKey(p => p.VerifiedById)
             .IsRequired(false)
             .OnDelete(DeleteBehavior.SetNull);

            e.HasMany(p => p.Details)
             .WithOne(d => d.PhoneReport)
             .HasForeignKey(d => d.PhoneReportId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── PhoneReportDetail ────────────────────────────────────────────────────
        builder.Entity<PhoneReportDetail>(e =>
        {
            e.HasKey(d => d.Id);
            e.Property(d => d.PhoneReportId).IsRequired();
            e.Property(d => d.ReportedById).HasMaxLength(450).IsRequired();
            e.Property(d => d.Description).HasMaxLength(2000);
            e.Property(d => d.ScamType).HasConversion<string>().HasMaxLength(50);
            e.Property(d => d.ModeratedById).HasMaxLength(450);
            e.Property(d => d.ModerationNote).HasMaxLength(500);

            e.HasIndex(d => d.IsModerated);
            e.HasIndex(d => d.PhoneReportId);
            e.HasIndex(d => d.ReportedById);
            e.HasIndex(d => d.CreatedAt);

            e.HasOne(d => d.ReportedBy)
             .WithMany()
             .HasForeignKey(d => d.ReportedById)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(d => d.ModeratedBy)
             .WithMany()
             .HasForeignKey(d => d.ModeratedById)
             .IsRequired(false)
             .OnDelete(DeleteBehavior.SetNull);
        });

        // ── EmergencyContact ─────────────────────────────────────────────────────
        builder.Entity<EmergencyContact>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Name).HasMaxLength(200).IsRequired();
            e.Property(c => c.PhoneNumber).HasMaxLength(20).IsRequired();
            e.Property(c => c.Category).HasMaxLength(100);
            e.Property(c => c.ProvinceCode).HasMaxLength(10);
            e.Property(c => c.ProvinceName).HasMaxLength(100);
            e.Property(c => c.Description).HasMaxLength(500);

            e.HasIndex(c => c.ProvinceCode);
            e.HasIndex(c => c.IsNational);
            e.HasIndex(c => c.Category);
        });

        // ── TravelRoute ──────────────────────────────────────────────────────────
        builder.Entity<TravelRoute>(e =>
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.UserId).HasMaxLength(450).IsRequired();
            e.Property(r => r.Name).HasMaxLength(200);
            e.Property(r => r.Status).HasConversion<string>().HasMaxLength(50).IsRequired();
            e.Property(r => r.WaypointsJson).HasColumnType("longtext");
            e.Property(r => r.Notes).HasMaxLength(1000);

            e.HasIndex(r => new { r.UserId, r.IsActive });
            e.HasIndex(r => r.Status);
            e.HasIndex(r => r.StartedAt);

            e.HasOne(r => r.User)
             .WithMany()
             .HasForeignKey(r => r.UserId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasMany(r => r.LocationLogs)
             .WithOne(l => l.TravelRoute)
             .HasForeignKey(l => l.TravelRouteId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasMany(r => r.Alerts)
             .WithOne(a => a.TravelRoute)
             .HasForeignKey(a => a.TravelRouteId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── LocationLog ──────────────────────────────────────────────────────────
        builder.Entity<LocationLog>(e =>
        {
            e.HasKey(l => l.Id);
            e.Property(l => l.Latitude).HasColumnType("decimal(10,7)").IsRequired();
            e.Property(l => l.Longitude).HasColumnType("decimal(10,7)").IsRequired();
            e.Property(l => l.Accuracy).HasColumnType("decimal(8,2)");
            e.Property(l => l.Speed).HasColumnType("decimal(8,2)");
            e.Property(l => l.Bearing).HasColumnType("decimal(5,2)");

            e.HasIndex(l => new { l.TravelRouteId, l.RecordedAt });
            e.HasIndex(l => l.RecordedAt);
        });

        // ── RouteAlert ───────────────────────────────────────────────────────────
        builder.Entity<RouteAlert>(e =>
        {
            e.HasKey(a => a.Id);
            e.Property(a => a.AlertType).HasConversion<string>().HasMaxLength(50).IsRequired();
            e.Property(a => a.Resolution).HasConversion<string>().HasMaxLength(50).IsRequired();
            e.Property(a => a.Message).HasMaxLength(1000);
            e.Property(a => a.Latitude).HasColumnType("decimal(10,7)");
            e.Property(a => a.Longitude).HasColumnType("decimal(10,7)");
            e.Property(a => a.AcknowledgedById).HasMaxLength(450);

            e.HasIndex(a => a.TravelRouteId);
            e.HasIndex(a => a.AlertType);
            e.HasIndex(a => a.Resolution);
            e.HasIndex(a => a.CreatedAt);

            e.HasOne(a => a.AcknowledgedBy)
             .WithMany()
             .HasForeignKey(a => a.AcknowledgedById)
             .IsRequired(false)
             .OnDelete(DeleteBehavior.SetNull);
        });

        // ── AuditLog ─────────────────────────────────────────────────────────────
        builder.Entity<AuditLog>(e =>
        {
            e.HasKey(a => a.Id);
            e.Property(a => a.UserId).HasMaxLength(450);
            e.Property(a => a.Action).HasMaxLength(200).IsRequired();
            e.Property(a => a.EntityName).HasMaxLength(100);
            e.Property(a => a.EntityId).HasMaxLength(200);
            e.Property(a => a.OldValues).HasColumnType("longtext");
            e.Property(a => a.NewValues).HasColumnType("longtext");
            e.Property(a => a.IpAddress).HasMaxLength(50);
            e.Property(a => a.UserAgent).HasMaxLength(500);

            e.HasIndex(a => a.UserId);
            e.HasIndex(a => a.CreatedAt);
            e.HasIndex(a => a.Action);
            e.HasIndex(a => a.EntityName);

            e.HasOne(a => a.User)
             .WithMany()
             .HasForeignKey(a => a.UserId)
             .IsRequired(false)
             .OnDelete(DeleteBehavior.SetNull);
        });

        // ── RelativeContact ──────────────────────────────────────────────────────
        builder.Entity<RelativeContact>(e =>
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.UserId).HasMaxLength(450).IsRequired();
            e.Property(r => r.Name).HasMaxLength(200).IsRequired();
            e.Property(r => r.PhoneNumber).HasMaxLength(20);
            e.Property(r => r.Email).HasMaxLength(256);
            e.Property(r => r.TelegramChatId).HasMaxLength(100);
            e.Property(r => r.ContactType).HasConversion<string>().HasMaxLength(50).IsRequired();
            e.Property(r => r.Notes).HasMaxLength(500);

            e.HasIndex(r => r.UserId);
            e.HasIndex(r => r.ContactType);

            e.HasOne(r => r.User)
             .WithMany()
             .HasForeignKey(r => r.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── Seed: national emergency numbers ─────────────────────────────────────
        builder.Entity<EmergencyContact>().HasData(
            new EmergencyContact
            {
                Id        = new Guid("00000000-0000-0000-0000-000000000001"),
                Name        = "Cảnh sát - Police",
                PhoneNumber = "113",
                Category    = "Police",
                IsNational  = true,
                Description = "Đường dây khẩn cấp cảnh sát toàn quốc",
                CreatedAt   = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new EmergencyContact
            {
                Id        = new Guid("00000000-0000-0000-0000-000000000002"),
                Name        = "Cứu hỏa - Fire Department",
                PhoneNumber = "114",
                Category    = "Fire",
                IsNational  = true,
                Description = "Đường dây khẩn cấp cứu hỏa toàn quốc",
                CreatedAt   = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new EmergencyContact
            {
                Id        = new Guid("00000000-0000-0000-0000-000000000003"),
                Name        = "Cấp cứu - Medical Emergency",
                PhoneNumber = "115",
                Category    = "Medical",
                IsNational  = true,
                Description = "Đường dây khẩn cấp y tế toàn quốc",
                CreatedAt   = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
