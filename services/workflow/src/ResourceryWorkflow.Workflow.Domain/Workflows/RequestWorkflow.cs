using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace ResourceryWorkflow.Workflow.Workflows;

public class RequestWorkflow : FullAuditedAggregateRoot<Guid>
{
    public Guid RequestId { get; private set; }

    public Guid WorkflowId { get; private set; }

    public Guid? CurrentStepId { get; private set; }

    public RequestWorkflowStatus Status { get; private set; }

    private RequestWorkflow()
    {
    }

    public RequestWorkflow(Guid id, Guid requestId, Guid workflowId)
        : base(id)
    {
        SetRequest(requestId);
        SetWorkflow(workflowId);
        Status = RequestWorkflowStatus.NotStarted;
    }

    public void SetRequest(Guid requestId)
    {
        if (requestId == Guid.Empty) throw new ArgumentException("The identifier cannot be empty.", nameof(requestId));
        RequestId = requestId;
    }

    public void SetWorkflow(Guid workflowId)
    {
        if (workflowId == Guid.Empty) throw new ArgumentException("The identifier cannot be empty.", nameof(workflowId));
        WorkflowId = workflowId;
    }

    public void SetCurrentStep(Guid? stepId)
    {
        CurrentStepId = stepId == null || stepId == Guid.Empty ? null : stepId;
    }

    public void Start()
    {
        Status = RequestWorkflowStatus.InProgress;
    }

    public void Complete()
    {
        Status = RequestWorkflowStatus.Completed;
    }

    public void Cancel()
    {
        Status = RequestWorkflowStatus.Cancelled;
    }
}
