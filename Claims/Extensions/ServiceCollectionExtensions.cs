using System.Text.Json.Serialization;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Claims.Auditing;
using Claims.Configuration;
using Claims.Controllers;
using Claims.Data;
using Claims.Filters;
using Claims.Messaging;
using Claims.Repositories;
using Claims.Services;
using Claims.Validation;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Claims.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddControllersWithJsonOptions(this IServiceCollection services)
    {
        services
            .AddControllers(options => options.Filters.Add<ValidationExceptionFilter>())
            .AddJsonOptions(x => x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        return services;
    }

    public static IServiceCollection AddDbContexts(
        this IServiceCollection services,
        IConfiguration configuration,
        string sqlConnectionString,
        string mongoConnectionString)
    {
        services.AddDbContext<AuditContext>(options => options.UseSqlServer(sqlConnectionString));

        services.AddDbContext<ClaimsContext>(options =>
        {
            var client = new MongoClient(mongoConnectionString);
            var database = client.GetDatabase(configuration["MongoDb:DatabaseName"]);
            options.UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName);
        });

        return services;
    }

    public static IServiceCollection AddScopedServices(this IServiceCollection services)
    {
        services.AddScoped<IClaimsRepository, ClaimsRepository>();
        services.AddScoped<ICoversRepository, CoversRepository>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IClaimsService, ClaimsService>();
        services.AddScoped<ICoversService, CoversService>();
        services.AddScoped<IClaimValidator, ClaimValidator>();
        services.AddScoped<ICoverValidator, CoverValidator>();
        return services;
    }

    public static IServiceCollection AddAuditMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        var flags = configuration.GetSection(FeatureFlags.SectionName).Get<FeatureFlags>() ?? new FeatureFlags();
        services.Configure<FeatureFlags>(configuration.GetSection(FeatureFlags.SectionName));

        // If the flag is on - use Azure Service Bus for messaging;
        // otherwise, we use an in-memory bus.
        // default to in-memory bus if configuration is missing or invalid.
        if (flags.UseAzureServiceBus)
        {
            services.Configure<AzureServiceBusOptions>(configuration.GetSection(AzureServiceBusOptions.SectionName));

            services.AddSingleton(CreateServiceBusClient);
            services.AddSingleton<IAuditMessageBus, AzureServiceBusAuditMessageBus>();
            services.AddHostedService<AzureServiceBusAuditConsumer>();
        }
        else
        {
            services.AddSingleton<IAuditMessageBus, InMemoryAuditMessageBus>();
            services.AddHostedService<AuditMessageProcessor>();
        }

        return services;
    }

    // Create Azure Service Bus Client only if feature flag is on
    private static ServiceBusClient CreateServiceBusClient(IServiceProvider serviceProvider)
    {
        var options = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<AzureServiceBusOptions>>().Value;

        if (!string.IsNullOrWhiteSpace(options.ConnectionString))
            return new ServiceBusClient(options.ConnectionString);

        if (string.IsNullOrWhiteSpace(options.FullyQualifiedNamespace))
            throw new InvalidOperationException(
                "AzureServiceBus configuration requires either ConnectionString or FullyQualifiedNamespace.");

        // Managed identity / az login / Visual Studio credential, etc.
        return new ServiceBusClient(options.FullyQualifiedNamespace, new DefaultAzureCredential());
    }

    public static IServiceCollection AddSingletonServices(this IServiceCollection services)
    {
        services.AddSingleton<IPremiumCalculator, PremiumCalculator>();
        return services;
    }

}