using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using ResourceryWorkflow.Workflow.Departments;
using ResourceryWorkflow.Workflow.Requests;
using ResourceryWorkflow.Workflow.Services;
using ResourceryWorkflow.Workflow.Workflows;
using ResourceryWorkflow.Workflow.Requests;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace ResourceryWorkflow.Workflow.EntityFrameworkCore;

[ConnectionStringName(ResourceryWorkflowNames.WorkflowDb)]
public interface IWorkflowDbContext : IEfCoreDbContext
{
    DbSet<Department> Departments { get; }

    DbSet<Service> Services { get; }

    DbSet<Request> Requests { get; }
    
    DbSet<global::ResourceryWorkflow.Workflow.Workflows.Workflow> Workflows { get; }
    DbSet<global::ResourceryWorkflow.Workflow.Workflows.WorkflowStep> WorkflowSteps { get; }

    DbSet<RequestWorkflow> RequestWorkflows { get; }
    DbSet<RequestWorkflowStep> RequestWorkflowSteps { get; }
}
