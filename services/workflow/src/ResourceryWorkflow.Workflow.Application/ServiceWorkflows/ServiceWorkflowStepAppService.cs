using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using ResourceryWorkflow.Workflow.ServiceWorkflows;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;

namespace ResourceryWorkflow.Workflow.ServiceWorkflows;

[RemoteService(IsEnabled = false)]
public class ServiceWorkflowStepAppService : ApplicationService, IServiceWorkflowStepAppService
{
    private readonly IRepository<ServiceWorkflowStep, Guid> _stepRepository;

    public ServiceWorkflowStepAppService(
        IRepository<ServiceWorkflowStep, Guid> stepRepository)
    {
        _stepRepository = stepRepository;
    }

    public async Task<ServiceWorkflowStepDto> CreateAsync(CreateServiceWorkflowStepDto input)
    {
        var step = new ServiceWorkflowStep(
            GuidGenerator.Create(),
            Guid.Empty, 
            input.Title,
            input.Order,
            input.Description,
            input.AssignedRoleId
        );
        await _stepRepository.InsertAsync(step, autoSave: true);

        return new ServiceWorkflowStepDto
        {
            Id = step.Id,
            ServiceWorkflowId = step.ServiceWorkflowId,
            Title = step.Title,
            Description = step.Description,
            Order = step.Order,
            AssignedRoleId = step.AssignedRoleId
        };
    }

    public async Task<ServiceWorkflowStepDto> UpdateAsync(Guid id, CreateServiceWorkflowStepDto input)
    {
        var step = await _stepRepository.GetAsync(id);
        step.SetTitle(input.Title);
        step.SetOrder(input.Order);
        
        await _stepRepository.UpdateAsync(step, autoSave: true);

        return new ServiceWorkflowStepDto
        {
            Id = step.Id,
            ServiceWorkflowId = step.ServiceWorkflowId,
            Title = step.Title,
            Description = step.Description,
            Order = step.Order,
            AssignedRoleId = step.AssignedRoleId
        };
    }

    public async Task DeleteAsync(Guid id)
    {
        await _stepRepository.DeleteAsync(id);
    }

    public async Task<ServiceWorkflowStepDto> GetAsync(Guid id)
    {
        var step = await _stepRepository.GetAsync(id);
        return new ServiceWorkflowStepDto
        {
            Id = step.Id,
            ServiceWorkflowId = step.ServiceWorkflowId,
            Title = step.Title,
            Description = step.Description,
            Order = step.Order,
            AssignedRoleId = step.AssignedRoleId
        };
    }

    public async Task<PagedResultDto<ServiceWorkflowStepDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var query = await _stepRepository.GetQueryableAsync();
        var totalCount = await _stepRepository.GetCountAsync();

        query = query.OrderBy(x => x.Order)
            .PageBy(input.SkipCount, input.MaxResultCount);

        var items = await AsyncExecuter.ToListAsync(query);

        var dtos = items.Select(step => new ServiceWorkflowStepDto
        {
            Id = step.Id,
            ServiceWorkflowId = step.ServiceWorkflowId,
            Title = step.Title,
            Description = step.Description,
            Order = step.Order,
            AssignedRoleId = step.AssignedRoleId
        }).ToList();

        return new PagedResultDto<ServiceWorkflowStepDto>(totalCount, dtos);
    }
}
