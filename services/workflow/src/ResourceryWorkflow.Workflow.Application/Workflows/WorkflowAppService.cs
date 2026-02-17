using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResourceryWorkflow.Workflow.Workflows.Repositories;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

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
}
