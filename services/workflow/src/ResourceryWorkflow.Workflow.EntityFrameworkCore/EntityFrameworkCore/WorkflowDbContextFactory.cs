using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ResourceryWorkflow.Workflow.EntityFrameworkCore;

public class WorkflowDbContextFactory : IDesignTimeDbContextFactory<WorkflowDbContext>
{
    public WorkflowDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<WorkflowDbContext>().UseNpgsql(
            GetConnectionStringFromConfiguration()
        );

        return new WorkflowDbContext(builder.Options);
    }

    private static string GetConnectionStringFromConfiguration()
    {
        return BuildConfiguration().GetConnectionString(WorkflowDbProperties.ConnectionStringName);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        // Try the default relative path first (used in some environments)
        var defaultHostPath = Path.Combine(
            Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName,
            $"host{Path.DirectorySeparatorChar}ResourceryWorkflow.Workflow.HttpApi.Host"
        );

        string hostPath;
        if (Directory.Exists(defaultHostPath))
        {
            hostPath = defaultHostPath;
        }
        else
        {
            // Fallback to the path under services/workflow/host when running tools from solution root
            hostPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "services", "workflow", "host", "ResourceryWorkflow.Workflow.HttpApi.Host"));
        }

        var builder = new ConfigurationBuilder()
            .SetBasePath(hostPath)
            .AddJsonFile("appsettings.json", false);

        return builder.Build();
    }
}
