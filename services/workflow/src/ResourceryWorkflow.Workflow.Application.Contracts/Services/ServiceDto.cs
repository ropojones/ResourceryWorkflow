using System;
using Volo.Abp.Application.Dtos;

namespace ResourceryWorkflow.Workflow.Services;

public class ServiceDto : FullAuditedEntityDto<Guid>
{
    public Guid DepartmentId { get; set; }

    public string DepartmentName { get; set; }

    public string Name { get; set; }

    public string Code { get; set; }

    public string Description { get; set; }
}
