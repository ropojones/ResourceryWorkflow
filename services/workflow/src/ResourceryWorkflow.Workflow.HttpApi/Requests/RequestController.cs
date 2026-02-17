using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace ResourceryWorkflow.Workflow.Requests;

[Area(WorkflowRemoteServiceConsts.ModuleName)]
[RemoteService(Name = WorkflowRemoteServiceConsts.RemoteServiceName)]
[Route("api/workflow/requests")]
public class RequestController(IRequestAppService requestAppService)
    : WorkflowController,
        IRequestAppService
{
    private readonly IRequestAppService _requestAppService = requestAppService;

    [HttpGet("{id}")]
    public Task<RequestDto> GetAsync(Guid id)
    {
        return _requestAppService.GetAsync(id);
    }

    [HttpGet]
    public Task<PagedResultDto<RequestDto>> GetListAsync(
        [FromQuery] PagedAndSortedResultRequestDto input
    )
    {
        return _requestAppService.GetListAsync(input);
    }

    [HttpPost]
    public Task<RequestDto> CreateAsync([FromBody] CreateUpdateRequestDto input)
    {
        return _requestAppService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public Task<RequestDto> UpdateAsync(Guid id, [FromBody] CreateUpdateRequestDto input)
    {
        return _requestAppService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _requestAppService.DeleteAsync(id);
    }
}
