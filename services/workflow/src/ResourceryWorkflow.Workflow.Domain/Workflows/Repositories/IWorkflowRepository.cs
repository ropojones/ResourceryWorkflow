using System;
using Volo.Abp.Domain.Repositories;

namespace ResourceryWorkflow.Workflow.Workflows.Repositories;

public interface IWorkflowRepository : IRepository<Workflow, Guid>
{
    // Add custom query methods if needed
}
