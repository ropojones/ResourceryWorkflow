using System;
using System.Collections.Generic;

namespace ResourceryWorkflow.Workflow.Workflows;

public class WorkflowDto
{
    public Guid Id { get; set; }
    public Guid ServiceId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public List<WorkflowStepDto> Steps { get; set; }
}

public class WorkflowStepDto
{
    public Guid Id { get; set; }
    public Guid WorkflowId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Order { get; set; }
    public WorkflowStepType StepType { get; set; }
    public Guid? AssignedRoleId { get; set; }
}
