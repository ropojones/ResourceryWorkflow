using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace ResourceryWorkflow.Workflow.ServiceWorkflows;

public class RequestWorkflowStep : FullAuditedAggregateRoot<Guid>
{
    public Guid RequestWorkflowId { get; private set; }

    public Guid ServiceWorkflowStepId { get; private set; }

    public Guid? AssignedToUserId { get; private set; }

    public RequestWorkflowStepStatus Status { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public string Comments { get; private set; }

    private RequestWorkflowStep()
    {
    }

    public RequestWorkflowStep(Guid id, Guid requestWorkflowId, Guid serviceWorkflowStepId, Guid? assignedToUserId = null)
        : base(id)
    {
        SetRequestWorkflow(requestWorkflowId);
        SetServiceWorkflowStep(serviceWorkflowStepId);
        AssignedToUserId = assignedToUserId;
        Status = RequestWorkflowStepStatus.Pending;
    }

    public void SetRequestWorkflow(Guid requestWorkflowId)
    {
        if (requestWorkflowId == Guid.Empty) throw new ArgumentException("The identifier cannot be empty.", nameof(requestWorkflowId));
        RequestWorkflowId = requestWorkflowId;
    }

    public void SetServiceWorkflowStep(Guid serviceWorkflowStepId)
    {
        if (serviceWorkflowStepId == Guid.Empty) throw new ArgumentException("The identifier cannot be empty.", nameof(serviceWorkflowStepId));
        ServiceWorkflowStepId = serviceWorkflowStepId;
    }

    public void AssignTo(Guid userId)
    {
        if (userId == Guid.Empty) throw new ArgumentException("The identifier cannot be empty.", nameof(userId));
        AssignedToUserId = userId;
    }

    public void Complete(string comments = null)
    {
        Status = RequestWorkflowStepStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        Comments = comments;
    }

    public void Skip(string comments = null)
    {
        Status = RequestWorkflowStepStatus.Skipped;
        CompletedAt = DateTime.UtcNow;
        Comments = comments;
    }
}
