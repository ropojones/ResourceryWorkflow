using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;

namespace ResourceryWorkflow.Workflow.ServiceWorkflows;

public interface IServiceWorkflowStepAppService : IApplicationService
{
    Task<ServiceWorkflowStepDto> CreateAsync(CreateServiceWorkflowStepDto input);
    Task<ServiceWorkflowStepDto> UpdateAsync(Guid id, CreateServiceWorkflowStepDto input);
    Task DeleteAsync(Guid id);

    Task<ServiceWorkflowStepDto> GetAsync(Guid id);
    Task<PagedResultDto<ServiceWorkflowStepDto>> GetListAsync(PagedAndSortedResultRequestDto input);
}
