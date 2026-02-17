using ResourceryWorkflow.Workflow.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace ResourceryWorkflow.Workflow.Permissions;

public class WorkflowPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var workflowGroup = context.AddGroup(
            WorkflowPermissions.GroupName,
            L("Permission:Workflow")
        );
        var departments = workflowGroup.AddPermission(
            WorkflowPermissions.Departments.Default,
            L("Permission:Workflow:Departments")
        );
        departments.AddChild(
            WorkflowPermissions.Departments.Create,
            L("Permission:Workflow:Departments:Create")
        );
        departments.AddChild(
            WorkflowPermissions.Departments.Update,
            L("Permission:Workflow:Departments:Update")
        );
        departments.AddChild(
            WorkflowPermissions.Departments.Delete,
            L("Permission:Workflow:Departments:Delete")
        );

        var services = workflowGroup.AddPermission(
            WorkflowPermissions.Services.Default,
            L("Permission:Workflow:Services")
        );
        services.AddChild(
            WorkflowPermissions.Services.Create,
            L("Permission:Workflow:Services:Create")
        );
        services.AddChild(
            WorkflowPermissions.Services.Update,
            L("Permission:Workflow:Services:Update")
        );
        services.AddChild(
            WorkflowPermissions.Services.Delete,
            L("Permission:Workflow:Services:Delete")
        );

        var requests = workflowGroup.AddPermission(
            WorkflowPermissions.Requests.Default,
            L("Permission:Workflow:Requests")
        );
        requests.AddChild(
            WorkflowPermissions.Requests.Create,
            L("Permission:Workflow:Requests:Create")
        );
        requests.AddChild(
            WorkflowPermissions.Requests.Update,
            L("Permission:Workflow:Requests:Update")
        );
        requests.AddChild(
            WorkflowPermissions.Requests.Delete,
            L("Permission:Workflow:Requests:Delete")
        );
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<WorkflowResource>(name);
    }
}
