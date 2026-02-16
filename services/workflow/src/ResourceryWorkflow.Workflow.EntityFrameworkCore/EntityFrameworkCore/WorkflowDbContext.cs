using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using ResourceryWorkflow.Workflow.Departments;
using ResourceryWorkflow.Workflow.Requests;
using ResourceryWorkflow.Workflow.Services;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace ResourceryWorkflow.Workflow.EntityFrameworkCore;

[ConnectionStringName(ResourceryWorkflowNames.WorkflowDb)]
public class WorkflowDbContext(DbContextOptions<WorkflowDbContext> options)
    : AbpDbContext<WorkflowDbContext>(options),
        IWorkflowDbContext
{
    public DbSet<Department> Departments { get; set; }

    public DbSet<Service> Services { get; set; }

    public DbSet<ServiceRelation> ServiceRelations { get; set; }

    public DbSet<Request> Requests { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureWorkflow();
    }
}
