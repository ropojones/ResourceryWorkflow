using Medallion.Threading;
using Medallion.Threading.Redis;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using StackExchange.Redis;
using ResourceryWorkflow.MultiTenancy;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs.RabbitMQ;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Data;
using Volo.Abp.DistributedLocking;
using Volo.Abp.EventBus.RabbitMq;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.RabbitMQ;
using Volo.Abp.Swashbuckle;

namespace ResourceryWorkflow;

[DependsOn(typeof(AbpAspNetCoreMultiTenancyModule))]
[DependsOn(typeof(AbpAspNetCoreSerilogModule))]
[DependsOn(typeof(AbpAutofacModule))]
[DependsOn(typeof(AbpBackgroundJobsRabbitMqModule))]
[DependsOn(typeof(AbpCachingStackExchangeRedisModule))]
[DependsOn(typeof(AbpDataModule))]
[DependsOn(typeof(AbpDistributedLockingModule))]
[DependsOn(typeof(AbpEventBusRabbitMqModule))]
[DependsOn(typeof(AbpSwashbuckleModule))]
[DependsOn(typeof(ResourceryWorkflowSharedModule))]
public class ResourceryWorkflowHostingModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        ConfigureDistributedLocking(context, configuration);

        Configure<AbpMultiTenancyOptions>(options =>
        {
            options.IsEnabled = MultiTenancyConsts.IsEnabled;
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Languages.Add(new LanguageInfo("en", "en", "English"));
            options.Languages.Add(new LanguageInfo("fr", "fr", "Français"));
            options.Languages.Add(new LanguageInfo("pt-BR", "pt-BR", "Português"));
        });

        Configure<AbpRabbitMqOptions>(options =>
        {
            var cstr = configuration.GetConnectionString(ResourceryWorkflowNames.RabbitMq);
            options.Connections.Default = new ConnectionFactory() { Uri = new Uri(cstr!) };
        });

        Configure<AbpRabbitMqEventBusOptions>(options =>
        {
            options.ClientName = configuration["RabbitMQ:EventBus:ClientName"]!;
            options.ExchangeName = configuration["RabbitMQ:EventBus:ExchangeName"]!;
        });
    }

    private static void ConfigureDistributedLocking(
        ServiceConfigurationContext context,
        IConfiguration configuration
    )
    {
        context.Services.AddSingleton<IDistributedLockProvider>(sp =>
        {
            var connection = ConnectionMultiplexer.Connect(
                configuration.GetConnectionString(ResourceryWorkflowNames.Redis)!
            );

            return new RedisDistributedSynchronizationProvider(connection.GetDatabase());
        });
    }
}

public static class HostingExtensions
{
    public static ServiceConfigurationContext ConfigureDataProtection(
        this ServiceConfigurationContext context,
        IWebHostEnvironment hostingEnvironment,
        IConfiguration configuration,
        string name
    )
    {
        var dataProtectionBuilder = context
            .Services.AddDataProtection()
            .SetApplicationName(ResourceryWorkflowNames.ResourceryWorkflow);
        if (!hostingEnvironment.IsDevelopment())
        {
            var redis = ConnectionMultiplexer.Connect(
                configuration.GetConnectionString(ResourceryWorkflowNames.Redis)!
            );
            dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, $"{name}-Keys");
        }

        return context;
    }
}
