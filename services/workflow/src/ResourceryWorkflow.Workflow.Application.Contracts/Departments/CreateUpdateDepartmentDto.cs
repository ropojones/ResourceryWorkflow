namespace ResourceryWorkflow.Workflow.Departments;

public class CreateUpdateDepartmentDto
{
    public string Name { get; set; }

    public string Code { get; set; }

    public string Description { get; set; }

    public bool IsActive { get; set; } = true;
}
