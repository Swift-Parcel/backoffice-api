using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SwiftParcel.Application.Common.Behaviors;

namespace SwiftParcel.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
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