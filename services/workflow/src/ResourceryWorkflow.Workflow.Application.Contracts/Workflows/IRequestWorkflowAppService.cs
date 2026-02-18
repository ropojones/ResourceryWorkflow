using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;

namespace ResourceryWorkflow.Workflow.Workflows;

public interface IRequestWorkflowAppService : IApplicationService
{
    Task<RequestWorkflowDto> CreateAsync(CreateUpdateRequestWorkflowDto input);
    Task<RequestWorkflowDto> UpdateAsync(Guid id, CreateUpdateRequestWorkflowDto input);
    Task DeleteAsync(Guid id);

    Task<RequestWorkflowDto> GetAsync(Guid id);
    Task<PagedResultDto<RequestWorkflowDto>> GetListAsync(PagedAndSortedResultRequestDto input);
}
