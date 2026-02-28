using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;

namespace ResourceryWorkflow.Workflow.ServiceWorkflows;

public interface IRequestWorkflowStepAppService : IApplicationService
{
    Task<RequestWorkflowStepDto> CreateAsync(CreateUpdateRequestWorkflowStepDto input);
    Task<RequestWorkflowStepDto> UpdateAsync(Guid id, CreateUpdateRequestWorkflowStepDto input);
    Task DeleteAsync(Guid id);

    Task<RequestWorkflowStepDto> GetAsync(Guid id);
    Task<PagedResultDto<RequestWorkflowStepDto>> GetListAsync(PagedAndSortedResultRequestDto input);
}
