using Serilog;
using ResourceryWorkflow.Administration.EntityFrameworkCore;
using ResourceryWorkflow.SaaS.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using ResourceryWorkflow.Workflow.EntityFrameworkCore;

namespace ResourceryWorkflow.DbMigrator;

internal class Program
{
    private static async Task Main(string[] args)
    {
        ResourceryWorkflowLogging.Initialize();

        var builder = Host.CreateApplicationBuilder(args);

        builder.AddServiceDefaults();
        builder.AddNpgsqlDbContext<AdministrationDbContext>(
            connectionName: ResourceryWorkflowNames.AdministrationDb
        );
        builder.AddNpgsqlDbContext<IdentityDbContext>(connectionName: ResourceryWorkflowNames.IdentityServiceDb);
        builder.AddNpgsqlDbContext<SaaSDbContext>(connectionName: ResourceryWorkflowNames.SaaSDb);       
        builder.AddNpgsqlDbContext<WorkflowDbContext>(connectionName: ResourceryWorkflowNames.WorkflowDb);
       
        builder.Configuration.AddAppSettingsSecretsJson();

        builder.Logging.AddSerilog();

        builder.Services.AddHostedService<DbMigratorHostedService>();

        var host = builder.Build();

        await host.RunAsync();
    }
}
