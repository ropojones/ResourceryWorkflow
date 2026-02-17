using AutoMapper;
using ResourceryWorkflow.Workflow.Departments;
using ResourceryWorkflow.Workflow.Requests;
using ResourceryWorkflow.Workflow.Services;

namespace ResourceryWorkflow.Workflow;

public class WorkflowApplicationAutoMapperProfile : Profile
{
	public WorkflowApplicationAutoMapperProfile()
	{
		CreateMap<Department, DepartmentDto>();
		CreateMap<Service, ServiceDto>();
		CreateMap<Request, RequestDto>();
	}
}
