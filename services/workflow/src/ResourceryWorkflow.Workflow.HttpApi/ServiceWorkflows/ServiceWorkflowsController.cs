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
[Route("api/app/service-workflows")]
public class ServiceWorkflowsController(IServiceWorkflowAppService workflowAppService)
    : AbpController,
        IServiceWorkflowAppService
{
    private readonly IServiceWorkflowAppService _workflowAppService = workflowAppService;

    [HttpPost]
    public Task<ServiceWorkflowDto> CreateAsync([FromBody] CreateUpdateServiceWorkflowDto input)
    {
        return _workflowAppService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public Task<ServiceWorkflowDto> UpdateAsync(Guid id, [FromBody] CreateUpdateServiceWorkflowDto input)
    {
        return _workflowAppService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _workflowAppService.DeleteAsync(id);
    }

    [HttpGet("{id}")]
    public Task<ServiceWorkflowDto> GetAsync(Guid id)
    {
        return _workflowAppService.GetAsync(id);
    }

    [HttpGet]
    public Task<PagedResultDto<ServiceWorkflowDto>> GetListAsync([FromQuery] PagedAndSortedResultRequestDto input)
    {
        return _workflowAppService.GetListAsync(input);
    }
}
