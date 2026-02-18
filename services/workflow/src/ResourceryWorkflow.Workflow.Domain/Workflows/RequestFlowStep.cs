using System;

namespace ResourceryWorkflow.Workflow.Workflows;

public class RequestFlowStep : RequestWorkflowStep
{
    private RequestFlowStep() { }

    public RequestFlowStep(Guid id, Guid requestFlowId, Guid flowStepId, Guid? assignedToUserId = null)
        : base(id, requestFlowId, flowStepId, assignedToUserId)
    {
    }
}
