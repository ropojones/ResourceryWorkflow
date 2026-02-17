using Microsoft.Extensions.Hosting;
using Projects;


namespace ResourceryWorkflow.AppHost;

internal class Program
{
    private static void Main(string[] args)
    {
        const string LaunchProfileName = "Aspire";
        var builder = DistributedApplication.CreateBuilder(args);

        var postgres = builder.AddPostgres(ResourceryWorkflowNames.Postgres).WithPgWeb();
        var rabbitMq = builder.AddRabbitMQ(ResourceryWorkflowNames.RabbitMq).WithManagementPlugin();
        var redis = builder.AddRedis(ResourceryWorkflowNames.Redis).WithRedisCommander();
        var seq = builder.AddSeq(ResourceryWorkflowNames.Seq);

        var adminDb = postgres.AddDatabase(ResourceryWorkflowNames.AdministrationDb);
        var identityDb = postgres.AddDatabase(ResourceryWorkflowNames.IdentityServiceDb);
        var saasDb = postgres.AddDatabase(ResourceryWorkflowNames.SaaSDb);
        var workflowDb = postgres.AddDatabase(ResourceryWorkflowNames.WorkflowDb);

        var migrator = builder
            .AddProject<ResourceryWorkflow_DbMigrator>(
                ResourceryWorkflowNames.DbMigrator,
                launchProfileName: LaunchProfileName
            )
            .WithReference(adminDb)
            .WithReference(identityDb)
            .WithReference(workflowDb)
            .WithReference(saasDb)
            .WithReference(seq)
            .WaitFor(postgres);

        var admin = builder
            .AddProject<ResourceryWorkflow_Administration_HttpApi_Host>(
                ResourceryWorkflowNames.AdministrationApi,
                launchProfileName: LaunchProfileName
            )
            .WithExternalHttpEndpoints()
            .WithReference(adminDb)
            .WithReference(identityDb)
            .WithReference(rabbitMq)
            .WithReference(redis)
            .WithReference(seq)
            .WaitForCompletion(migrator);

        var identity = builder
            .AddProject<ResourceryWorkflow_IdentityService_HttpApi_Host>(
                ResourceryWorkflowNames.IdentityServiceApi,
                launchProfileName: LaunchProfileName
            )
            .WithExternalHttpEndpoints()
            .WithReference(adminDb)
            .WithReference(identityDb)
            .WithReference(saasDb)
            .WithReference(rabbitMq)
            .WithReference(redis)
            .WithReference(seq)
            .WaitForCompletion(migrator);

        var saas = builder
            .AddProject<ResourceryWorkflow_SaaS_HttpApi_Host>(
                ResourceryWorkflowNames.SaaSApi,
                launchProfileName: LaunchProfileName
            )
            .WithExternalHttpEndpoints()
            .WithReference(adminDb)
            .WithReference(saasDb)
            .WithReference(rabbitMq)
            .WithReference(redis)
            .WithReference(seq)
            .WaitForCompletion(migrator);

        builder
            .AddProject<ResourceryWorkflow_Workflow_HttpApi_Host>(
                ResourceryWorkflowNames.WorkflowApi,
                launchProfileName: LaunchProfileName
            )
            .WithExternalHttpEndpoints()
            .WithReference(adminDb)
            .WithReference(workflowDb)
            .WithReference(rabbitMq)
            .WithReference(redis)
            .WithReference(seq)
            .WaitForCompletion(migrator);

        var gateway = builder
            .AddProject<ResourceryWorkflow_Gateway>(ResourceryWorkflowNames.Gateway, launchProfileName: LaunchProfileName)
            .WithExternalHttpEndpoints()
            .WithReference(seq)
            .WaitFor(admin)
            .WaitFor(identity)
            .WaitFor(saas);

        var authserver = builder
            .AddProject<ResourceryWorkflow_AuthServer>(
                ResourceryWorkflowNames.AuthServer,
                launchProfileName: LaunchProfileName
            )
            .WithExternalHttpEndpoints()
            .WithReference(adminDb)
            .WithReference(identityDb)
            .WithReference(saasDb)
            .WithReference(rabbitMq)
            .WithReference(redis)
            .WithReference(seq)
            .WaitForCompletion(migrator);

        builder
            .AddProject<ResourceryWorkflow_WebApp_Blazor>(
                ResourceryWorkflowNames.WebAppClient,
                launchProfileName: LaunchProfileName
            )
            .WithExternalHttpEndpoints()
            .WithReference(seq)
            .WaitFor(authserver)
            .WaitFor(gateway);

        builder.Build().Run();
    }
}
