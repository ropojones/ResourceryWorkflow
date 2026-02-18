using System;
using Volo.Abp.Domain.Repositories;

namespace ResourceryWorkflow.Workflow.Workflows.Repositories;

public interface IRequestWorkflowRepository : IRepository<RequestWorkflow, Guid>
{
}
