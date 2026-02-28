using System;

namespace ResourceryWorkflow.Workflow.ServiceWorkflows;

public enum WorkflowStepType
{
    Approval = 0,
    Task = 1,
    Notification = 2
}

public enum RequestWorkflowStatus
{
    NotStarted = 0,
    InProgress = 1,
    Completed = 2,
    Cancelled = 3
}

public enum RequestWorkflowStepStatus
{
    Pending = 0,
    InProgress = 1,
    Completed = 2,
    Skipped = 3,
    Failed = 4
}
