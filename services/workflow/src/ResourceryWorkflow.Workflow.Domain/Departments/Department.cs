using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace ResourceryWorkflow.Workflow.Departments;

public class Department : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; private set; }

    public string Code { get; private set; }

    public string Description { get; private set; }

    public bool IsActive { get; private set; }

    private Department()
    {
    }

    public Department(Guid id, string name, string code, string description, bool isActive = true)
        : base(EnsureNotEmpty(id, nameof(id)))
    {
        SetName(name);
        SetCode(code);
        SetDescription(description);
        IsActive = isActive;
    }

    public void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), DepartmentConsts.MaxNameLength);
    }

    public void SetCode(string code)
    {
        Code = Check.NotNullOrWhiteSpace(code, nameof(code), DepartmentConsts.MaxCodeLength);
    }

    public void SetDescription(string description)
    {
        Description = Check.Length(description, nameof(description), DepartmentConsts.MaxDescriptionLength);
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
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
