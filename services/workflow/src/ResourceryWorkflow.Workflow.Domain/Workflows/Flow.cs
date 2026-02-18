using System;

namespace ResourceryWorkflow.Workflow.Workflows;

public class Flow : Workflow
{
    private Flow() { }

    public Flow(Guid id, Guid serviceId, string name, string description = null, bool isActive = true)
        : base(id, serviceId, name, description, isActive)
    {
    }
}
