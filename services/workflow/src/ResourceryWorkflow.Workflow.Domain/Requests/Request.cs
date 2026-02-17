using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace ResourceryWorkflow.Workflow.Requests;

public class Request : FullAuditedAggregateRoot<Guid>
{
    public Guid DepartmentId { get; private set; }

    public Guid ServiceId { get; private set; }

    public Guid RequestedByUserId { get; private set; }

    public string Title { get; private set; }

    public string Description { get; private set; }

    public RequestStatus Status { get; private set; }

    public RequestPriority Priority { get; private set; }

    public DateTime? TargetCompletionTime { get; private set; }

    private Request()
    {
    }

    public Request(
        Guid id,
        Guid departmentId,
        Guid serviceId,
        Guid requestedByUserId,
        string title,
        string description,
        RequestPriority priority = RequestPriority.Normal,
        DateTime? targetCompletionTime = null)
        : base(EnsureNotEmpty(id, nameof(id)))
    {
        SetDepartment(departmentId);
        SetService(serviceId);
        SetRequestedByUserId(requestedByUserId);
        SetTitle(title);
        SetDescription(description);
        SetPriority(priority);
        SetTargetCompletionTime(targetCompletionTime);
        Status = RequestStatus.Submitted;
    }

    public void SetDepartment(Guid departmentId)
    {
        DepartmentId = EnsureNotEmpty(departmentId, nameof(departmentId));
    }

    public void SetService(Guid serviceId)
    {
        ServiceId = EnsureNotEmpty(serviceId, nameof(serviceId));
    }

    public void SetRequestedByUserId(Guid requestedByUserId)
    {
        RequestedByUserId = EnsureNotEmpty(requestedByUserId, nameof(requestedByUserId));
    }

    public void SetTitle(string title)
    {
        Title = Check.NotNullOrWhiteSpace(title, nameof(title), RequestConsts.MaxTitleLength);
    }

    public void SetDescription(string description)
    {
        Description = Check.Length(description, nameof(description), RequestConsts.MaxDescriptionLength);
    }

    public void SetPriority(RequestPriority priority)
    {
        Priority = priority;
    }

    public void SetTargetCompletionTime(DateTime? targetCompletionTime)
    {
        TargetCompletionTime = targetCompletionTime;
    }

    public void Submit()
    {
        Status = RequestStatus.Submitted;
    }

    public void Approve()
    {
        Status = RequestStatus.Approved;
    }

    public void Reject()
    {
        Status = RequestStatus.Rejected;
    }

    public void Start()
    {
        Status = RequestStatus.InProgress;
    }

    public void Complete()
    {
        Status = RequestStatus.Completed;
    }

    public void Cancel()
    {
        Status = RequestStatus.Cancelled;
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
