using System;
using Volo.Abp.Application.Dtos;

namespace ResourceryWorkflow.Workflow.Requests;

public class RequestDto : FullAuditedEntityDto<Guid>
{
    public Guid DepartmentId { get; set; }

    public string DepartmentName { get; set; }

    public Guid ServiceId { get; set; }

    public string ServiceName { get; set; }

    public Guid RequestedByUserId { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public RequestStatus Status { get; set; }

    public RequestPriority Priority { get; set; }

    public DateTime? TargetCompletionTime { get; set; }
}
