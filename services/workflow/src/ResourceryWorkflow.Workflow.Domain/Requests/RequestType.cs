using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace ResourceryWorkflow.Workflow.Requests;

public class RequestType : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; private set; }

    public string Description { get; private set; }

    private RequestType()
    {
    }

    public RequestType(Guid id, string name, string description)
        : base(EnsureNotEmpty(id, nameof(id)))
    {
        SetName(name);
        SetDescription(description);
    }

    public void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), RequestTypeConsts.MaxNameLength);
    }

    public void SetDescription(string description)
    {
        Description = Check.Length(description, nameof(description), RequestTypeConsts.MaxDescriptionLength);
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
