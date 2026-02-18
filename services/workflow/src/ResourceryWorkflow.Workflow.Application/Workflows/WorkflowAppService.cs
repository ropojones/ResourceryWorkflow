using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResourceryWorkflow.Workflow.Workflows.Repositories;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Linq;
using Volo.Abp;

namespace ResourceryWorkflow.Workflow.Workflows;

public class WorkflowAppService : ApplicationService, ResourceryWorkflow.Workflow.Workflows.IWorkflowAppService
{
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IWorkflowStepRepository _workflowStepRepository;

    public WorkflowAppService(IWorkflowRepository workflowRepository, IWorkflowStepRepository workflowStepRepository)
    {
        _workflowRepository = workflowRepository;
        _workflowStepRepository = workflowStepRepository;
    }

    public async Task<WorkflowDto> CreateAsync(CreateWorkflowDto input)
    {
        var workflow = new Workflow(
            GuidGenerator.Create(),
            input.ServiceId,
            input.Name,
            input.Description,
            input.IsActive
        );
        await _workflowRepository.InsertAsync(workflow, autoSave: true);

        var steps = new List<WorkflowStep>();
        if (input.Steps != null)
        {
            foreach (var stepDto in input.Steps)
            {
                var step = new WorkflowStep(
                    GuidGenerator.Create(),
                    workflow.Id,
                    stepDto.Title,
                    stepDto.Order,
                    stepDto.StepType,
                    stepDto.Description,
                    stepDto.AssignedRoleId
                );
                steps.Add(step);
                await _workflowStepRepository.InsertAsync(step, autoSave: true);
            }
        }

        // Map to DTO
        return new WorkflowDto
        {
            Id = workflow.Id,
            ServiceId = workflow.ServiceId,
            Name = workflow.Name,
            Description = workflow.Description,
            IsActive = workflow.IsActive,
            Steps = steps.Select(s => new WorkflowStepDto
            {
                Id = s.Id,
                WorkflowId = s.WorkflowId,
                Title = s.Title,
                Description = s.Description,
                Order = s.Order,
                StepType = s.StepType,
                AssignedRoleId = s.AssignedRoleId
            }).ToList()
        };
    }

    public async Task<WorkflowDto> GetAsync(Guid id)
    {
        var workflow = await _workflowRepository.GetAsync(id);
        var steps = await _workflowStepRepository.GetListAsync(w => w.WorkflowId == workflow.Id);

        return new WorkflowDto
        {
            Id = workflow.Id,
            ServiceId = workflow.ServiceId,
            Name = workflow.Name,
            Description = workflow.Description,
            IsActive = workflow.IsActive,
            Steps = steps.Select(s => new WorkflowStepDto
            {
                Id = s.Id,
                WorkflowId = s.WorkflowId,
                Title = s.Title,
                Description = s.Description,
                Order = s.Order,
                StepType = s.StepType,
                AssignedRoleId = s.AssignedRoleId
            }).ToList()
        };
    }

    public async Task<PagedResultDto<WorkflowDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var query = await _workflowRepository.GetQueryableAsync();
        var totalCount = await _workflowRepository.GetCountAsync();

        query = query.OrderBy(x => x.Name)
            .PageBy(input.SkipCount, input.MaxResultCount);

        var items = await AsyncExecuter.ToListAsync(query);

        var dtos = new List<WorkflowDto>();
        foreach (var item in items)
        {
            var steps = await _workflowStepRepository.GetListAsync(w => w.WorkflowId == item.Id);
            dtos.Add(new WorkflowDto
            {
                Id = item.Id,
                ServiceId = item.ServiceId,
                Name = item.Name,
                Description = item.Description,
                IsActive = item.IsActive,
                Steps = steps.Select(s => new WorkflowStepDto
                {
                    Id = s.Id,
                    WorkflowId = s.WorkflowId,
                    Title = s.Title,
                    Description = s.Description,
                    Order = s.Order,
                    StepType = s.StepType,
                    AssignedRoleId = s.AssignedRoleId
                }).ToList()
            });
        }

        return new PagedResultDto<WorkflowDto>(totalCount, dtos);
    }
}
