using AutoMapper;
using ResourceryWorkflow.Workflow.Departments;
using ResourceryWorkflow.Workflow.Requests;
using ResourceryWorkflow.Workflow.Services;
using ResourceryWorkflow.Workflow.ServiceWorkflows;

namespace ResourceryWorkflow.Workflow;

public class WorkflowApplicationAutoMapperProfile : Profile
{
	public WorkflowApplicationAutoMapperProfile()
	{
		CreateMap<Department, DepartmentDto>();
		CreateMap<Service, ServiceDto>()
			.ForMember(dest => dest.DepartmentName, opt => opt.Ignore());
		CreateMap<Request, RequestDto>();
		CreateMap<RequestType, RequestTypeDto>();
		CreateMap<CreateUpdateRequestTypeDto, RequestType>()
			.ForMember(dest => dest.Id, opt => opt.Ignore())
			.ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
			.ForMember(dest => dest.DeleterId, opt => opt.Ignore())
			.ForMember(dest => dest.DeletionTime, opt => opt.Ignore())
			.ForMember(dest => dest.LastModificationTime, opt => opt.Ignore())
			.ForMember(dest => dest.LastModifierId, opt => opt.Ignore())
			.ForMember(dest => dest.CreationTime, opt => opt.Ignore())
			.ForMember(dest => dest.CreatorId, opt => opt.Ignore())
			.ForMember(dest => dest.ExtraProperties, opt => opt.Ignore())
			.ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore());
		
		// ServiceWorkflow mappings
		CreateMap<ServiceWorkflow, ServiceWorkflowDto>();
		CreateMap<CreateUpdateServiceWorkflowDto, ServiceWorkflow>()
			.ForMember(dest => dest.Id, opt => opt.Ignore())
			.ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
			.ForMember(dest => dest.DeleterId, opt => opt.Ignore())
			.ForMember(dest => dest.DeletionTime, opt => opt.Ignore())
			.ForMember(dest => dest.LastModificationTime, opt => opt.Ignore())
			.ForMember(dest => dest.LastModifierId, opt => opt.Ignore())
			.ForMember(dest => dest.CreationTime, opt => opt.Ignore())
			.ForMember(dest => dest.CreatorId, opt => opt.Ignore())
			.ForMember(dest => dest.ExtraProperties, opt => opt.Ignore())
			.ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
			.ForMember(dest => dest.Steps, opt => opt.Ignore());
		CreateMap<ServiceWorkflowStep, ServiceWorkflowStepDto>();
		CreateMap<CreateServiceWorkflowStepDto, ServiceWorkflowStep>()
			.ForMember(dest => dest.Id, opt => opt.Ignore())
			.ForMember(dest => dest.ServiceWorkflowId, opt => opt.Ignore())
			.ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
			.ForMember(dest => dest.DeleterId, opt => opt.Ignore())
			.ForMember(dest => dest.DeletionTime, opt => opt.Ignore())
			.ForMember(dest => dest.LastModificationTime, opt => opt.Ignore())
			.ForMember(dest => dest.LastModifierId, opt => opt.Ignore())
			.ForMember(dest => dest.CreationTime, opt => opt.Ignore())
			.ForMember(dest => dest.CreatorId, opt => opt.Ignore())
			.ForMember(dest => dest.ExtraProperties, opt => opt.Ignore())
			.ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore());
		
		// RequestWorkflow mappings
		CreateMap<RequestWorkflow, RequestWorkflowDto>();
		CreateMap<CreateUpdateRequestWorkflowDto, RequestWorkflow>()
			.ForMember(dest => dest.Id, opt => opt.Ignore())
			.ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
			.ForMember(dest => dest.DeleterId, opt => opt.Ignore())
			.ForMember(dest => dest.DeletionTime, opt => opt.Ignore())
			.ForMember(dest => dest.LastModificationTime, opt => opt.Ignore())
			.ForMember(dest => dest.LastModifierId, opt => opt.Ignore())
			.ForMember(dest => dest.CreationTime, opt => opt.Ignore())
			.ForMember(dest => dest.CreatorId, opt => opt.Ignore())
			.ForMember(dest => dest.ExtraProperties, opt => opt.Ignore())
			.ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore());
		CreateMap<RequestWorkflowStep, RequestWorkflowStepDto>();
		CreateMap<CreateUpdateRequestWorkflowStepDto, RequestWorkflowStep>()
			.ForMember(dest => dest.Id, opt => opt.Ignore())
			.ForMember(dest => dest.CompletedAt, opt => opt.Ignore())
			.ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
			.ForMember(dest => dest.DeleterId, opt => opt.Ignore())
			.ForMember(dest => dest.DeletionTime, opt => opt.Ignore())
			.ForMember(dest => dest.LastModificationTime, opt => opt.Ignore())
			.ForMember(dest => dest.LastModifierId, opt => opt.Ignore())
			.ForMember(dest => dest.CreationTime, opt => opt.Ignore())
			.ForMember(dest => dest.CreatorId, opt => opt.Ignore())
			.ForMember(dest => dest.ExtraProperties, opt => opt.Ignore())
			.ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore());
	}
}
