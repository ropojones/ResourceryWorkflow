using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace ResourceryWorkflow.Workflow.Workflows;

[Area(WorkflowRemoteServiceConsts.ModuleName)]
[RemoteService(Name = WorkflowRemoteServiceConsts.RemoteServiceName)]
[Route("api/flow/request-flow-steps")]
public class RequestFlowStepsController(IRequestWorkflowStepAppService requestWorkflowStepAppService)
    : WorkflowController,
        IRequestWorkflowStepAppService
{
    private readonly IRequestWorkflowStepAppService _requestWorkflowStepAppService = requestWorkflowStepAppService;

    [HttpPost]
    public Task<RequestWorkflowStepDto> CreateAsync([FromBody] CreateUpdateRequestWorkflowStepDto input)
    {
        return _requestWorkflowStepAppService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public Task<RequestWorkflowStepDto> UpdateAsync(Guid id, [FromBody] CreateUpdateRequestWorkflowStepDto input)
    {
        return _requestWorkflowStepAppService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _requestWorkflowStepAppService.DeleteAsync(id);
    }

    [HttpGet("{id}")]
    public Task<RequestFlowStepDto> GetAsync(Guid id)
    {
        return _requestWorkflowStepAppService.GetAsync(id);
    }

    [HttpGet]
    public Task<PagedResultDto<RequestFlowStepDto>> GetListAsync([FromQuery] PagedAndSortedResultRequestDto input)
    {
        return _requestWorkflowStepAppService.GetListAsync(input);
    }
}
