using System.Reflection;
using AntiScam.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AntiScam.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IScamScenarioService, ScamScenarioService>();
        services.AddScoped<IClassificationService, ClassificationService>();
        services.AddScoped<IChatAnalysisService, ChatAnalysisService>();
        services.AddScoped<IPhoneReportService, PhoneReportService>();
        services.AddScoped<ITravelRouteService, TravelRouteService>();
        services.AddScoped<IEmergencyContactService, EmergencyContactService>();
        services.AddScoped<IRelativeContactService, RelativeContactService>();
        services.AddScoped<IStatisticsService, StatisticsService>();
        services.AddScoped<ITrainingDataService, TrainingDataService>();

        return services;
    }
}
