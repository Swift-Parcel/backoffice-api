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
        
        //automatically finds all AbstractValidator and then registrates them
        services.AddValidatorsFromAssembly(assembly);
        
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            
            //registrates pipeline behavior
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        
        return services;
    }
}