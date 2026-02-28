using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace ResourceryWorkflow.Workflow.Requests;

[Area(WorkflowRemoteServiceConsts.ModuleName)]
[RemoteService(Name = WorkflowRemoteServiceConsts.RemoteServiceName)]
[Route("api/workflow/request-types")]
public class RequestTypeController(IRequestTypeAppService requestTypeAppService)
    : WorkflowController,
        IRequestTypeAppService
{
    private readonly IRequestTypeAppService _requestTypeAppService = requestTypeAppService;

    [HttpGet("{id}")]
    public Task<RequestTypeDto> GetAsync(Guid id)
    {
        return _requestTypeAppService.GetAsync(id);
    }

    [HttpGet]
    public Task<PagedResultDto<RequestTypeDto>> GetListAsync(
        [FromQuery] PagedAndSortedResultRequestDto input
    )
    {
        return _requestTypeAppService.GetListAsync(input);
    }

    [HttpPost]
    public Task<RequestTypeDto> CreateAsync([FromBody] CreateUpdateRequestTypeDto input)
    {
        return _requestTypeAppService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public Task<RequestTypeDto> UpdateAsync(Guid id, [FromBody] CreateUpdateRequestTypeDto input)
    {
        return _requestTypeAppService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _requestTypeAppService.DeleteAsync(id);
    }
}
