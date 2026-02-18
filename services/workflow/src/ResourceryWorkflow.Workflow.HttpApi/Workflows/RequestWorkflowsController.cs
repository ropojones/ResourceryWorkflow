using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace ResourceryWorkflow.Workflow.Workflows;

[Area(WorkflowRemoteServiceConsts.ModuleName)]
[RemoteService(Name = WorkflowRemoteServiceConsts.RemoteServiceName)]
[Route("api/flow/request-flows")]
public class RequestFlowsController(IRequestWorkflowAppService requestWorkflowAppService)
    : WorkflowController,
        IRequestWorkflowAppService
{
    private readonly IRequestWorkflowAppService _requestWorkflowAppService = requestWorkflowAppService;

    [HttpPost]
    public Task<RequestWorkflowDto> CreateAsync([FromBody] CreateUpdateRequestWorkflowDto input)
    {
        return _requestWorkflowAppService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public Task<RequestWorkflowDto> UpdateAsync(Guid id, [FromBody] CreateUpdateRequestWorkflowDto input)
    {
        return _requestWorkflowAppService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _requestWorkflowAppService.DeleteAsync(id);
    }

    [HttpGet("{id}")]
    public Task<RequestFlowDto> GetAsync(Guid id)
    {
        return _requestWorkflowAppService.GetAsync(id);
    }

    [HttpGet]
    public Task<PagedResultDto<RequestFlowDto>> GetListAsync([FromQuery] PagedAndSortedResultRequestDto input)
    {
        return _requestWorkflowAppService.GetListAsync(input);
    }
}
