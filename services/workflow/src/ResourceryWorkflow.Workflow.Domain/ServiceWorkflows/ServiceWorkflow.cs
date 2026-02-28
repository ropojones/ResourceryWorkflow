using System;
using System.Collections.Generic;
using ResourceryWorkflow.Workflow.Services;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp;

namespace ResourceryWorkflow.Workflow.ServiceWorkflows;

public class ServiceWorkflow : FullAuditedAggregateRoot<Guid>
{
    public Guid ServiceId { get; private set; }

    public string Title { get; private set; }

    public string Description { get; private set; }

    public string Activities { get; private set; }

    public string Outcomes { get; private set; }

    public string Details { get; private set; }

    public bool HasChecklist { get; private set; }

    public bool IsActive { get; private set; }

    public int? DefaultSlaHours { get; private set; }



    // Navigation property (optional)
    public virtual ICollection<ServiceWorkflowStep> Steps { get; private set; }

    private ServiceWorkflow()
    {
        Steps = new List<ServiceWorkflowStep>();
    }

    public ServiceWorkflow(
        Guid id,
        Guid serviceId,
        string title,
        string description,
        string activities,
        string outcomes,
        string details,
        bool hasChecklist = false,
        int? defaultSlaHours = null,
        bool isActive = true)
        : base(id)
    {
        SetService(serviceId);
        SetTitle(title);
        SetDescription(description);
        SetActivities(activities);
        SetOutcomes(outcomes);
        SetDetails(details);
        SetHasChecklist(hasChecklist);
        SetDefaultSlaHours(defaultSlaHours);
        IsActive = isActive;
        Steps = new List<ServiceWorkflowStep>();
    }

    public void SetService(Guid serviceId)
    {
        if (serviceId == Guid.Empty) throw new ArgumentException("The identifier cannot be empty.", nameof(serviceId));
        ServiceId = serviceId;
    }

    public void SetTitle(string title)
    {
        Title = Check.NotNullOrWhiteSpace(title, nameof(title), ServiceConsts.MaxNameLength);
    }

    public void SetDescription(string description)
    {
        Description = Check.Length(description, nameof(description), ServiceConsts.MaxDescriptionLength);
    }

    public void SetActivities(string activities)
    {
        Activities = Check.Length(activities, nameof(activities), ServiceConsts.MaxActivitiesLength);
    }

    public void SetOutcomes(string outcomes)
    {
        Outcomes = Check.Length(outcomes, nameof(outcomes), ServiceConsts.MaxOutcomesLength);
    }

    public void SetDetails(string details)
    {
        Details = Check.Length(details, nameof(details), ServiceConsts.MaxDetailsLength);
    }

    public void SetHasChecklist(bool hasChecklist)
    {
        HasChecklist = hasChecklist;
    }

    public void SetDefaultSlaHours(int? defaultSlaHours)
    {
        if (defaultSlaHours.HasValue)
        {
            Check.Range(defaultSlaHours.Value, nameof(defaultSlaHours), 1, 24 * 30);
        }

        DefaultSlaHours = defaultSlaHours;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}
