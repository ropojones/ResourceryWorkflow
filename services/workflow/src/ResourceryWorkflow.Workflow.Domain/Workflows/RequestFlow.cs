using System;

namespace ResourceryWorkflow.Workflow.Workflows;

public class RequestFlow : RequestWorkflow
{
    private RequestFlow() { }

    public RequestFlow(Guid id, Guid requestId, Guid flowId)
        : base(id, requestId, flowId)
    {
    }
}
