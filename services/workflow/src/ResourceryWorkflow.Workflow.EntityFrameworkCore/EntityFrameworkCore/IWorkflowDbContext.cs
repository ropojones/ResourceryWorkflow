using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using ResourceryWorkflow.Workflow.Departments;
using ResourceryWorkflow.Workflow.Requests;
using ResourceryWorkflow.Workflow.Services;
using ResourceryWorkflow.Workflow.ServiceWorkflows;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace ResourceryWorkflow.Workflow.EntityFrameworkCore;

[ConnectionStringName(ResourceryWorkflowNames.WorkflowDb)]
public interface IWorkflowDbContext : IEfCoreDbContext
{
    DbSet<Department> Departments { get; }
    DbSet<Service> Services { get; }
    DbSet<Request> Requests { get; }    
    DbSet<ServiceWorkflow> ServiceWorkflows { get; }
    DbSet<ServiceWorkflowStep> ServiceWorkflowSteps { get; }
    DbSet<RequestWorkflow> RequestWorkflows { get; }
    DbSet<RequestWorkflowStep> RequestWorkflowSteps { get; }
}
