using System;

namespace ResourceryWorkflow.Workflow.Requests;

public class CreateUpdateRequestDto
{
    public Guid DepartmentId { get; set; }

    public Guid ServiceId { get; set; }

    public Guid RequestedByUserId { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public RequestPriority Priority { get; set; } = RequestPriority.Normal;

    public DateTime? TargetCompletionTime { get; set; }
}
