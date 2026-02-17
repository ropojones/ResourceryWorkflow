using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace ResourceryWorkflow.Workflow.Departments;

[Area(WorkflowRemoteServiceConsts.ModuleName)]
[RemoteService(Name = WorkflowRemoteServiceConsts.RemoteServiceName)]
[Route("api/workflow/departments")]
public class DepartmentController(IDepartmentAppService departmentAppService)
    : WorkflowController,
        IDepartmentAppService
{
    private readonly IDepartmentAppService _departmentAppService = departmentAppService;

    [HttpGet("{id}")]
    public Task<DepartmentDto> GetAsync(Guid id)
    {
        return _departmentAppService.GetAsync(id);
    }

    [HttpGet]
    public Task<PagedResultDto<DepartmentDto>> GetListAsync(
        [FromQuery] PagedAndSortedResultRequestDto input
    )
    {
        return _departmentAppService.GetListAsync(input);
    }

    [HttpPost]
    public Task<DepartmentDto> CreateAsync([FromBody] CreateUpdateDepartmentDto input)
    {
        return _departmentAppService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public Task<DepartmentDto> UpdateAsync(Guid id, [FromBody] CreateUpdateDepartmentDto input)
    {
        return _departmentAppService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _departmentAppService.DeleteAsync(id);
    }
}
