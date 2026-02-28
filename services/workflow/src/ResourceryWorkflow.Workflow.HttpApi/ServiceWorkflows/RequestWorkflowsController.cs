using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using ResourceryWorkflow.Workflow.ServiceWorkflows;

namespace ResourceryWorkflow.Workflow.ServiceWorkflows;

[Area(WorkflowRemoteServiceConsts.ModuleName)]
[RemoteService(Name = WorkflowRemoteServiceConsts.RemoteServiceName)]
[Route("api/app/request-workflows")]
public class RequestWorkflowsController(IRequestWorkflowAppService requestWorkflowAppService)
    : AbpController,
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
    public Task<RequestWorkflowDto> GetAsync(Guid id)
    {
        return _requestWorkflowAppService.GetAsync(id);
    }

    [HttpGet]
    public Task<PagedResultDto<RequestWorkflowDto>> GetListAsync([FromQuery] PagedAndSortedResultRequestDto input)
    {
        return _requestWorkflowAppService.GetListAsync(input);
    }
}
