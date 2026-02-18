using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace ResourceryWorkflow.Workflow.Workflows;

[Area(WorkflowRemoteServiceConsts.ModuleName)]
[RemoteService(Name = WorkflowRemoteServiceConsts.RemoteServiceName)]
[Route("api/flow/flows")]
public class FlowsController(IWorkflowAppService workflowAppService)
    : WorkflowController,
        IWorkflowAppService
{
    private readonly IWorkflowAppService _workflowAppService = workflowAppService;

    [HttpPost]
    public Task<WorkflowDto> CreateAsync([FromBody] CreateWorkflowDto input)
    {
        return _workflowAppService.CreateAsync(input);
    }

    [HttpGet("{id}")]
    public Task<FlowDto> GetAsync(Guid id)
    {
        return _workflowAppService.GetAsync(id);
    }

    [HttpGet]
    public Task<PagedResultDto<FlowDto>> GetListAsync([FromQuery] PagedAndSortedResultRequestDto input)
    {
        return _workflowAppService.GetListAsync(input);
    }
}
