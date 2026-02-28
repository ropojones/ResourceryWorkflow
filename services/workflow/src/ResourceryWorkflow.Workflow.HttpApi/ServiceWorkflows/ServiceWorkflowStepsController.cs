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
[Route("api/app/service-workflow-steps")]
public class ServiceWorkflowStepsController(IServiceWorkflowStepAppService serviceWorkflowStepAppService)
    : AbpController,
        IServiceWorkflowStepAppService
{
    private readonly IServiceWorkflowStepAppService _serviceWorkflowStepAppService = serviceWorkflowStepAppService;

    [HttpPost]
    public Task<ServiceWorkflowStepDto> CreateAsync([FromBody] CreateServiceWorkflowStepDto input)
    {
        return _serviceWorkflowStepAppService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public Task<ServiceWorkflowStepDto> UpdateAsync(Guid id, [FromBody] CreateServiceWorkflowStepDto input)
    {
        return _serviceWorkflowStepAppService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _serviceWorkflowStepAppService.DeleteAsync(id);
    }

    [HttpGet("{id}")]
    public Task<ServiceWorkflowStepDto> GetAsync(Guid id)
    {
        return _serviceWorkflowStepAppService.GetAsync(id);
    }

    [HttpGet]
    public Task<PagedResultDto<ServiceWorkflowStepDto>> GetListAsync([FromQuery] PagedAndSortedResultRequestDto input)
    {
        return _serviceWorkflowStepAppService.GetListAsync(input);
    }
}
