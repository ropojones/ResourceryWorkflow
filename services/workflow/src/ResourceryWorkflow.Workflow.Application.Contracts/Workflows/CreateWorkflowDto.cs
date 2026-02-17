using System;
using System.Collections.Generic;

namespace ResourceryWorkflow.Workflow.Workflows;

public class CreateWorkflowDto
{
    public Guid ServiceId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; } = true;
    public List<CreateWorkflowStepDto> Steps { get; set; }
}

public class CreateWorkflowStepDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public int Order { get; set; }
    public WorkflowStepType StepType { get; set; }
    public Guid? AssignedRoleId { get; set; }
}
