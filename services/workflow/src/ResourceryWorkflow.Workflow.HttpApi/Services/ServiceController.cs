using System;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace ResourceryWorkflow.Workflow.Services;

[Area(WorkflowRemoteServiceConsts.ModuleName)]
[RemoteService(Name = WorkflowRemoteServiceConsts.RemoteServiceName)]
[ControllerName("Service")]
[Route("api/workflow/services")]
public class ServiceController(IServiceAppService serviceAppService)
    : WorkflowController,
        IServiceAppService
{
    private readonly IServiceAppService _serviceAppService = serviceAppService;

    [HttpGet("{id}")]
    public Task<ServiceDto> GetAsync(Guid id)
    {
        return _serviceAppService.GetAsync(id);
    }

    [HttpGet]
    public Task<PagedResultDto<ServiceDto>> GetListAsync(
        [FromQuery] PagedAndSortedResultRequestDto input
    )
    {
        return _serviceAppService.GetListAsync(input);
    }

    [HttpPost]
    public Task<ServiceDto> CreateAsync([FromBody] CreateUpdateServiceDto input)
    {
        return _serviceAppService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public Task<ServiceDto> UpdateAsync(Guid id, [FromBody] CreateUpdateServiceDto input)
    {
        return _serviceAppService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _serviceAppService.DeleteAsync(id);
    }
}
