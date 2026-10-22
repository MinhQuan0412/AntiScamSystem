using AntiScam.Domain.Entities;
using AntiScam.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace AntiScam.Infrastructure.Data.Repositories;

/// <summary>
/// Unit of Work implementation for EF Core.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _currentTransaction;

    public IRepository<ScamScenario> ScamScenarios { get; }
    public IRepository<TrainingDataSample> TrainingDataSamples { get; }
    public IRepository<ModelVersion> ModelVersions { get; }
    public IRepository<PhoneReport> PhoneReports { get; }
    public IRepository<PhoneReportDetail> PhoneReportDetails { get; }
    public IRepository<EmergencyContact> EmergencyContacts { get; }
    public IRepository<TravelRoute> TravelRoutes { get; }
    public IRepository<LocationLog> LocationLogs { get; }
    public IRepository<RouteAlert> RouteAlerts { get; }
    public IRepository<AuditLog> AuditLogs { get; }
    public IRepository<RelativeContact> RelativeContacts { get; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        ScamScenarios = new GenericRepository<ScamScenario>(context);
        TrainingDataSamples = new GenericRepository<TrainingDataSample>(context);
        ModelVersions = new GenericRepository<ModelVersion>(context);
        PhoneReports = new GenericRepository<PhoneReport>(context);
        PhoneReportDetails = new GenericRepository<PhoneReportDetail>(context);
        EmergencyContacts = new GenericRepository<EmergencyContact>(context);
        TravelRoutes = new GenericRepository<TravelRoute>(context);
        LocationLogs = new GenericRepository<LocationLog>(context);
        RouteAlerts = new GenericRepository<RouteAlert>(context);
        AuditLogs = new GenericRepository<AuditLog>(context);
        RelativeContacts = new GenericRepository<RelativeContact>(context);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync()
    {
        if (_currentTransaction != null) return;
        _currentTransaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await SaveChangesAsync();
            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        try
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync();
            }
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
