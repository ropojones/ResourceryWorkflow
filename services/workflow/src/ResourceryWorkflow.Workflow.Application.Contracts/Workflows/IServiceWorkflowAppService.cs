using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;

namespace ResourceryWorkflow.Workflow.ServiceWorkflows;

public interface IServiceWorkflowAppService : IApplicationService
{
    Task<ServiceWorkflowDto> CreateAsync(CreateUpdateServiceWorkflowDto input);
    Task<ServiceWorkflowDto> UpdateAsync(Guid id, CreateUpdateServiceWorkflowDto input);
    Task DeleteAsync(Guid id);

    Task<ServiceWorkflowDto> GetAsync(Guid id);
    Task<PagedResultDto<ServiceWorkflowDto>> GetListAsync(PagedAndSortedResultRequestDto input);
}
