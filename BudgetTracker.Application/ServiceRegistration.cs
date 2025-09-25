using System.Reflection;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Application.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace BudgetTracker.Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        services.AddValidatorsFromAssembly(assembly);
        services.AddScoped<IBudgetTrackerService, BudgetTrackerService>();
        
        return services;
    }
}