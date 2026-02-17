using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp;

namespace ResourceryWorkflow.Workflow.Workflows;

public class Workflow : FullAuditedAggregateRoot<Guid>
{
    public Guid ServiceId { get; private set; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public bool IsActive { get; private set; }

    // Navigation property (optional)
    public virtual ICollection<WorkflowStep> Steps { get; private set; }

    private Workflow()
    {
        Steps = new List<WorkflowStep>();
    }

    public Workflow(Guid id, Guid serviceId, string name, string description = null, bool isActive = true)
        : base(id)
    {
        SetService(serviceId);
        SetName(name);
        SetDescription(description);
        IsActive = isActive;
        Steps = new List<WorkflowStep>();
    }

    public void SetService(Guid serviceId)
    {
        if (serviceId == Guid.Empty) throw new ArgumentException("The identifier cannot be empty.", nameof(serviceId));
        ServiceId = serviceId;
    }

    public void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name));
    }

    public void SetDescription(string description)
    {
        Description = description;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}
