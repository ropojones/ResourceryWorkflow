using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace ResourceryWorkflow.Workflow.Services;

public class Service : FullAuditedAggregateRoot<Guid>
{
    private readonly List<ServiceRelation> _relatedServices = new();

    public Guid DepartmentId { get; private set; }

    public string Name { get; private set; }

    public string Code { get; private set; }

    public string Description { get; private set; }

    public string Activities { get; private set; }

    public string Outcomes { get; private set; }

    public string Details { get; private set; }

    public bool HasChecklist { get; private set; }

    public bool IsActive { get; private set; }

    public int? DefaultSlaHours { get; private set; }

    public IReadOnlyCollection<ServiceRelation> RelatedServices => new ReadOnlyCollection<ServiceRelation>(_relatedServices);

    private Service()
    {
    }

    public Service(
        Guid id,
        Guid departmentId,
        string name,
        string code,
        string description,
        string activities,
        string outcomes,
        string details,
        bool hasChecklist = false,
        int? defaultSlaHours = null,
        bool isActive = true)
        : base(EnsureNotEmpty(id, nameof(id)))
    {
        SetDepartment(departmentId);
        SetName(name);
        SetCode(code);
        SetDescription(description);
        SetActivities(activities);
        SetOutcomes(outcomes);
        SetDetails(details);
        SetHasChecklist(hasChecklist);
        SetDefaultSlaHours(defaultSlaHours);
        IsActive = isActive;
    }

    public void SetDepartment(Guid departmentId)
    {
        DepartmentId = EnsureNotEmpty(departmentId, nameof(departmentId));
    }

    public void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), ServiceConsts.MaxNameLength);
    }

    public void SetCode(string code)
    {
        Code = Check.NotNullOrWhiteSpace(code, nameof(code), ServiceConsts.MaxCodeLength);
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

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void AddRelatedService(Guid relatedServiceId)
    {
        EnsureNotEmpty(relatedServiceId, nameof(relatedServiceId));

        if (relatedServiceId == Id)
        {
            throw new ArgumentException("Related service cannot be the same as the current service.", nameof(relatedServiceId));
        }

        if (_relatedServices.Any(item => item.RelatedServiceId == relatedServiceId))
        {
            return;
        }

        _relatedServices.Add(new ServiceRelation(Guid.NewGuid(), Id, relatedServiceId));
    }

    public void RemoveRelatedService(Guid relatedServiceId)
    {
        var existing = _relatedServices.FirstOrDefault(item => item.RelatedServiceId == relatedServiceId);
        if (existing is null)
        {
            return;
        }

        _relatedServices.Remove(existing);
    }

    public static Guid EnsureNotEmpty(Guid id, string paramName)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("The identifier cannot be empty.", paramName);
        }

        return id;
    }
}
