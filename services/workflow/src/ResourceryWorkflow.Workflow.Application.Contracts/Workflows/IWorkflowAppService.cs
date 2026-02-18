using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;

namespace ResourceryWorkflow.Workflow.Workflows;

public interface IWorkflowAppService : IApplicationService
{
    Task<WorkflowDto> CreateAsync(CreateWorkflowDto input);
    Task<WorkflowDto> GetAsync(Guid id);
    Task<PagedResultDto<WorkflowDto>> GetListAsync(PagedAndSortedResultRequestDto input);
}
