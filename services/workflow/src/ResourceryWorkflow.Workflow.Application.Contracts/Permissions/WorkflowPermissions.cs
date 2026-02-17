using Volo.Abp.Reflection;

namespace ResourceryWorkflow.Workflow.Permissions;

public class WorkflowPermissions
{
    public const string GroupName = "Workflow";


    public static class Departments
    {
        public const string Default = GroupName + ".Departments";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class Services
    {
        public const string Default = GroupName + ".Services";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class Requests
    {
        public const string Default = GroupName + ".Requests";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(WorkflowPermissions));
    }
}
