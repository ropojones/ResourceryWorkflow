using System;
using Volo.Abp.Application.Dtos;

namespace ResourceryWorkflow.Workflow.Requests;

public class RequestDto : FullAuditedEntityDto<Guid>
{
    public Guid RequestTypeId { get; set; }

    public string RequestTypeName { get; set; }

    public Guid RequestedByUserId { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public RequestStatus Status { get; set; }

    public RequestPriority Priority { get; set; }

    public DateTime? TargetCompletionTime { get; set; }
}
