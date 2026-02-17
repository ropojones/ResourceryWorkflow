using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace ResourceryWorkflow.Workflow.Requests;

public interface IRequestAppService : IApplicationService
{
    Task<RequestDto> GetAsync(Guid id);

    Task<PagedResultDto<RequestDto>> GetListAsync(PagedAndSortedResultRequestDto input);

    Task<RequestDto> CreateAsync(CreateUpdateRequestDto input);

    Task<RequestDto> UpdateAsync(Guid id, CreateUpdateRequestDto input);

    Task DeleteAsync(Guid id);
}
