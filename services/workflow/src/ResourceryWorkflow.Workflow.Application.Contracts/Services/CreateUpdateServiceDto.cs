using System;

namespace ResourceryWorkflow.Workflow.Services;

public class CreateUpdateServiceDto
{
    public Guid DepartmentId { get; set; }

    public string Name { get; set; }

    public string Code { get; set; }

    public string Description { get; set; }

    public string Activities { get; set; }

    public string Outcomes { get; set; }

    public string Details { get; set; }

    public bool HasChecklist { get; set; }

    public bool IsActive { get; set; } = true;

    public int? DefaultSlaHours { get; set; }
}
