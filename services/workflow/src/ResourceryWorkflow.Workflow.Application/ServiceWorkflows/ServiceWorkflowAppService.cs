using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResourceryWorkflow.Workflow.ServiceWorkflows;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;
using Volo.Abp;

namespace ResourceryWorkflow.Workflow.ServiceWorkflows;

public class ServiceWorkflowAppService : ApplicationService, IServiceWorkflowAppService
{
    private readonly IRepository<ServiceWorkflow, Guid> _workflowRepository;
    private readonly IRepository<ServiceWorkflowStep, Guid> _workflowStepRepository;

    public ServiceWorkflowAppService(
        IRepository<ServiceWorkflow, Guid> workflowRepository,
        IRepository<ServiceWorkflowStep, Guid> workflowStepRepository)
    {
        _workflowRepository = workflowRepository;
        _workflowStepRepository = workflowStepRepository;
    }

    public async Task<ServiceWorkflowDto> CreateAsync(CreateUpdateServiceWorkflowDto input)
    {
        var workflow = new ServiceWorkflow(
            GuidGenerator.Create(),
            input.ServiceId,
            input.Title,
            input.Description,
            input.Activities,
            input.Outcomes,
            input.Details,
            input.HasChecklist,
            input.DefaultSlaHours,
            input.IsActive
        );
        await _workflowRepository.InsertAsync(workflow, autoSave: true);

        var steps = new List<ServiceWorkflowStep>();
        if (input.Steps != null)
        {
            foreach (var stepDto in input.Steps)
            {
                var step = new ServiceWorkflowStep(
                    GuidGenerator.Create(),
                    workflow.Id,
                    stepDto.Title,
                    stepDto.Order,
                    stepDto.Description,
                    stepDto.AssignedRoleId
                );
                steps.Add(step);
                await _workflowStepRepository.InsertAsync(step, autoSave: true);
            }
        }

        // Map to DTO
        return new ServiceWorkflowDto
        {
            Id = workflow.Id,
            ServiceId = workflow.ServiceId,
            Title = workflow.Title,
            Description = workflow.Description,
            Activities = workflow.Activities,
            Outcomes = workflow.Outcomes,
            Details = workflow.Details,
            HasChecklist = workflow.HasChecklist,
            DefaultSlaHours = workflow.DefaultSlaHours,
            IsActive = workflow.IsActive,
            Steps = steps.Select(s => new ServiceWorkflowStepDto
            {
                Id = s.Id,
                ServiceWorkflowId = s.ServiceWorkflowId,
                Title = s.Title,
                Description = s.Description,
                Order = s.Order,
                AssignedRoleId = s.AssignedRoleId
            }).ToList()
        };
    }

    public async Task<ServiceWorkflowDto> GetAsync(Guid id)
    {
        var workflow = await _workflowRepository.GetAsync(id);
        var steps = await _workflowStepRepository.GetListAsync(w => w.ServiceWorkflowId == workflow.Id);

        return new ServiceWorkflowDto
        {
            Id = workflow.Id,
            ServiceId = workflow.ServiceId,
            Title = workflow.Title,
            Description = workflow.Description,
            Activities = workflow.Activities,
            Outcomes = workflow.Outcomes,
            Details = workflow.Details,
            HasChecklist = workflow.HasChecklist,
            DefaultSlaHours = workflow.DefaultSlaHours,
            IsActive = workflow.IsActive,
            Steps = steps.Select(s => new ServiceWorkflowStepDto
            {
                Id = s.Id,
                ServiceWorkflowId = s.ServiceWorkflowId,
                Title = s.Title,
                Description = s.Description,
                Order = s.Order,
                AssignedRoleId = s.AssignedRoleId
            }).ToList()
        };
    }

    public async Task<PagedResultDto<ServiceWorkflowDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var query = await _workflowRepository.GetQueryableAsync();
        var totalCount = await _workflowRepository.GetCountAsync();

        query = query.OrderBy(x => x.Title)
            .PageBy(input.SkipCount, input.MaxResultCount);

        var items = await AsyncExecuter.ToListAsync(query);

        var dtos = new List<ServiceWorkflowDto>();
        foreach (var item in items)
        {
            var steps = await _workflowStepRepository.GetListAsync(w => w.ServiceWorkflowId == item.Id);
            dtos.Add(new ServiceWorkflowDto
            {
                Id = item.Id,
                ServiceId = item.ServiceId,
                Title = item.Title,
                Description = item.Description,
                Activities = item.Activities,
                Outcomes = item.Outcomes,
                Details = item.Details,
                HasChecklist = item.HasChecklist,
                DefaultSlaHours = item.DefaultSlaHours,
                IsActive = item.IsActive,
                Steps = steps.Select(s => new ServiceWorkflowStepDto
                {
                    Id = s.Id,
                    ServiceWorkflowId = s.ServiceWorkflowId,
                    Title = s.Title,
                    Description = s.Description,
                    Order = s.Order,
                    AssignedRoleId = s.AssignedRoleId
                }).ToList()
            });
        }

        return new PagedResultDto<ServiceWorkflowDto>(totalCount, dtos);
    }

    public async Task<ServiceWorkflowDto> UpdateAsync(Guid id, CreateUpdateServiceWorkflowDto input)
    {
        var workflow = await _workflowRepository.GetAsync(id);
        workflow.SetTitle(input.Title);
        workflow.SetDescription(input.Description);
        workflow.SetActivities(input.Activities);
        workflow.SetOutcomes(input.Outcomes);
        workflow.SetDetails(input.Details);
        workflow.SetHasChecklist(input.HasChecklist);
        workflow.SetDefaultSlaHours(input.DefaultSlaHours);
        workflow.SetService(input.ServiceId);
        if (!input.IsActive)
        {
            workflow.Deactivate();
        }
        else
        {
            workflow.Activate();
        }

        await _workflowRepository.UpdateAsync(workflow, autoSave: true);

        // Update steps if provided
        if (input.Steps != null)
        {
            var existingSteps = await _workflowStepRepository.GetListAsync(s => s.ServiceWorkflowId == workflow.Id);
            foreach (var existingStep in existingSteps)
            {
                await _workflowStepRepository.DeleteAsync(existingStep.Id, autoSave: true);
            }

            foreach (var stepDto in input.Steps)
            {
                var step = new ServiceWorkflowStep(
                    GuidGenerator.Create(),
                    workflow.Id,
                    stepDto.Title,
                    stepDto.Order,
                    stepDto.Description,
                    stepDto.AssignedRoleId
                );
                await _workflowStepRepository.InsertAsync(step, autoSave: true);
            }
        }

        var steps = await _workflowStepRepository.GetListAsync(w => w.ServiceWorkflowId == workflow.Id);
        return new ServiceWorkflowDto
        {
            Id = workflow.Id,
            ServiceId = workflow.ServiceId,
            Title = workflow.Title,
            Description = workflow.Description,
            Activities = workflow.Activities,
            Outcomes = workflow.Outcomes,
            Details = workflow.Details,
            HasChecklist = workflow.HasChecklist,
            DefaultSlaHours = workflow.DefaultSlaHours,
            IsActive = workflow.IsActive,
            Steps = steps.Select(s => new ServiceWorkflowStepDto
            {
                Id = s.Id,
                ServiceWorkflowId = s.ServiceWorkflowId,
                Title = s.Title,
                Description = s.Description,
                Order = s.Order,
                AssignedRoleId = s.AssignedRoleId
            }).ToList()
        };
    }

    public async Task DeleteAsync(Guid id)
    {
        var steps = await _workflowStepRepository.GetListAsync(s => s.ServiceWorkflowId == id);
        foreach (var step in steps)
        {
            await _workflowStepRepository.DeleteAsync(step.Id, autoSave: true);
        }

        await _workflowRepository.DeleteAsync(id);
    }
}
