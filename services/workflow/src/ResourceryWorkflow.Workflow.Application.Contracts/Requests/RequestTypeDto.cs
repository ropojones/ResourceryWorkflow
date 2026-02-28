using System;
using Volo.Abp.Application.Dtos;

namespace ResourceryWorkflow.Workflow.Requests;

public class RequestTypeDto : FullAuditedEntityDto<Guid>
{
    public string Name { get; set; }

    public string Description { get; set; }
}
