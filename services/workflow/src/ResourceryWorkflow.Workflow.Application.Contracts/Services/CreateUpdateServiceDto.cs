using System;

namespace ResourceryWorkflow.Workflow.Services;

public class CreateUpdateServiceDto
{
    public Guid DepartmentId { get; set; }

    public string Name { get; set; }

    public string Code { get; set; }

    public string Description { get; set; }
}
