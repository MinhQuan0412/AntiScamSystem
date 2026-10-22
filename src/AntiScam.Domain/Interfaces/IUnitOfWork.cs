using AntiScam.Domain.Entities;

namespace AntiScam.Domain.Interfaces;

/// <summary>
/// Unit of Work pattern interface managing database transactions and repositories.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IRepository<ScamScenario> ScamScenarios { get; }
    IRepository<TrainingDataSample> TrainingDataSamples { get; }
    IRepository<ModelVersion> ModelVersions { get; }
    IRepository<PhoneReport> PhoneReports { get; }
    IRepository<PhoneReportDetail> PhoneReportDetails { get; }
    IRepository<EmergencyContact> EmergencyContacts { get; }
    IRepository<TravelRoute> TravelRoutes { get; }
    IRepository<LocationLog> LocationLogs { get; }
    IRepository<RouteAlert> RouteAlerts { get; }
    IRepository<AuditLog> AuditLogs { get; }
    IRepository<RelativeContact> RelativeContacts { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
