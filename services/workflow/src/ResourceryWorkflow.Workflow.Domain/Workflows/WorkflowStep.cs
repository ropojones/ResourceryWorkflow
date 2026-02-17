using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp;

namespace ResourceryWorkflow.Workflow.Workflows;

public class WorkflowStep : FullAuditedAggregateRoot<Guid>
{
    public Guid WorkflowId { get; private set; }

    public string Title { get; private set; }

    public string Description { get; private set; }

    public int Order { get; private set; }

    public WorkflowStepType StepType { get; private set; }

    public Guid? AssignedRoleId { get; private set; }

    private WorkflowStep()
    {
    }

    public WorkflowStep(Guid id, Guid workflowId, string title, int order, WorkflowStepType stepType = WorkflowStepType.Approval, string description = null, Guid? assignedRoleId = null)
        : base(id)
    {
        SetWorkflow(workflowId);
        SetTitle(title);
        SetOrder(order);
        StepType = stepType;
        Description = description;
        AssignedRoleId = assignedRoleId;
    }

    public void SetWorkflow(Guid workflowId)
    {
        if (workflowId == Guid.Empty) throw new ArgumentException("The identifier cannot be empty.", nameof(workflowId));
        WorkflowId = workflowId;
    }

    public void SetTitle(string title)
    {
        Title = Check.NotNullOrWhiteSpace(title, nameof(title));
    }

    public void SetOrder(int order)
    {
        if (order < 0) throw new ArgumentOutOfRangeException(nameof(order));
        Order = order;
    }
}
