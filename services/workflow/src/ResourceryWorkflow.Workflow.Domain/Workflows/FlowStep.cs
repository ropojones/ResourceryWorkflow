using System;

namespace ResourceryWorkflow.Workflow.Workflows;

public class FlowStep : WorkflowStep
{
    private FlowStep() { }

    public FlowStep(Guid id, Guid flowId, string title, int order, WorkflowStepType stepType = WorkflowStepType.Approval, string description = null, Guid? assignedRoleId = null)
        : base(id, flowId, title, order, stepType, description, assignedRoleId)
    {
    }
}
