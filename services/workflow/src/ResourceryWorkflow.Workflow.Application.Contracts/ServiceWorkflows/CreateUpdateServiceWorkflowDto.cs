using System;
using System.Collections.Generic;

namespace ResourceryWorkflow.Workflow.ServiceWorkflows;

public class CreateUpdateServiceWorkflowDto
{
    public Guid ServiceId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Activities { get; set; }
    public string Outcomes { get; set; }
    public string Details { get; set; }
    public bool HasChecklist { get; set; }
    public int? DefaultSlaHours { get; set; }
    public bool IsActive { get; set; } = true;
    public List<CreateServiceWorkflowStepDto> Steps { get; set; }
}

public class CreateServiceWorkflowStepDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public int Order { get; set; }
    public Guid? AssignedRoleId { get; set; }
}
