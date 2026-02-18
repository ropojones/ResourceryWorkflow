using System;
using System.Collections.Generic;

namespace ResourceryWorkflow.Workflow.Workflows;

public class RequestWorkflowDto
{
    public Guid Id { get; set; }
    public Guid RequestId { get; set; }
    public Guid WorkflowId { get; set; }
    public Guid? CurrentStepId { get; set; }
    public RequestWorkflowStatus Status { get; set; }
}

public class CreateUpdateRequestWorkflowDto
{
    public Guid RequestId { get; set; }
    public Guid WorkflowId { get; set; }
    public Guid? CurrentStepId { get; set; }
    public RequestWorkflowStatus Status { get; set; }
}

public class RequestWorkflowStepDto
{
    public Guid Id { get; set; }
    public Guid RequestWorkflowId { get; set; }
    public Guid WorkflowStepId { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public RequestWorkflowStepStatus Status { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string Comments { get; set; }
}

public class CreateUpdateRequestWorkflowStepDto
{
    public Guid RequestWorkflowId { get; set; }
    public Guid WorkflowStepId { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public RequestWorkflowStepStatus Status { get; set; }
    public string Comments { get; set; }
}
