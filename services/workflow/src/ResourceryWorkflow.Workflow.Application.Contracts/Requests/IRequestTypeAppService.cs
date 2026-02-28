using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace ResourceryWorkflow.Workflow.Requests;

public interface IRequestTypeAppService : IApplicationService
{
    Task<RequestTypeDto> GetAsync(Guid id);

    Task<PagedResultDto<RequestTypeDto>> GetListAsync(PagedAndSortedResultRequestDto input);

    Task<RequestTypeDto> CreateAsync(CreateUpdateRequestTypeDto input);

    Task<RequestTypeDto> UpdateAsync(Guid id, CreateUpdateRequestTypeDto input);

    Task DeleteAsync(Guid id);
}
