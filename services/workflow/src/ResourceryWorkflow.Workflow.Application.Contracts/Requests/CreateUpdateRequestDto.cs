using System;

namespace ResourceryWorkflow.Workflow.Requests;

public class CreateUpdateRequestDto
{
    public Guid RequestTypeId { get; set; }

    public Guid RequestedByUserId { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public RequestPriority Priority { get; set; } = RequestPriority.Normal;

    public DateTime? TargetCompletionTime { get; set; }
}
