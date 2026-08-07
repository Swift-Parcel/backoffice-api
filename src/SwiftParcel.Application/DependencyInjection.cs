using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using SwiftParcel.Application.Common.Behaviors;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Settings;
using SwiftParcel.Application.Services;

namespace SwiftParcel.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddScoped<ICaseAssignmentService, CaseAssignmentService>();

        services.Configure<SlaOptions>(configuration.GetSection(nameof(SlaOptions)));
        
        var assembly = Assembly.GetExecutingAssembly();
        
        services.AddValidatorsFromAssembly(assembly);
        services.AddAutoMapper(cfg => cfg.AddMaps(assembly));
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            
            cfg.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        
        return services;
    }
}