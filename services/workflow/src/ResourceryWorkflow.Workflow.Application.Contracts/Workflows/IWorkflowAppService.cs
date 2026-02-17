using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ResourceryWorkflow.Workflow.Workflows;

public interface IWorkflowAppService : IApplicationService
{
    Task<WorkflowDto> CreateAsync(CreateWorkflowDto input);
}
