using System;
using System.Collections.Generic;

namespace ResourceryWorkflow.Workflow.ServiceWorkflows;

public class ServiceWorkflowDto
{
    public Guid Id { get; set; }
    public Guid ServiceId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Activities { get; set; }
    public string Outcomes { get; set; }
    public string Details { get; set; }
    public bool HasChecklist { get; set; }
    public int? DefaultSlaHours { get; set; }
    public bool IsActive { get; set; }
    public List<ServiceWorkflowStepDto> Steps { get; set; }
}

public class ServiceWorkflowStepDto
{
    public Guid Id { get; set; }
    public Guid ServiceWorkflowId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Order { get; set; }
    public Guid? AssignedRoleId { get; set; }
}
