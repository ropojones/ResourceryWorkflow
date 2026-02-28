using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp;

namespace ResourceryWorkflow.Workflow.ServiceWorkflows;

public class ServiceWorkflowStep : FullAuditedAggregateRoot<Guid>
{
    public Guid ServiceWorkflowId { get; private set; }

    public string Title { get; private set; }

    public string Description { get; private set; }

    public int Order { get; private set; }

    public Guid? AssignedRoleId { get; private set; }

    private ServiceWorkflowStep()
    {
    }

    public ServiceWorkflowStep(Guid id, Guid serviceWorkflowId, string title, int order, string description = null, Guid? assignedRoleId = null)
        : base(id)
    {
        SetWorkflow(serviceWorkflowId);
        SetTitle(title);
        SetOrder(order);
         Description = description;
        AssignedRoleId = assignedRoleId;
    }

    public void SetWorkflow(Guid serviceWorkflowId)
    {
        if (serviceWorkflowId == Guid.Empty) throw new ArgumentException("The identifier cannot be empty.", nameof(serviceWorkflowId));
        ServiceWorkflowId = serviceWorkflowId;
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
