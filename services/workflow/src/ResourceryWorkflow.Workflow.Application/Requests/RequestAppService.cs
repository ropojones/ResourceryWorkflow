using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResourceryWorkflow.Workflow.Departments;
using ResourceryWorkflow.Workflow.Services;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;
using Volo.Abp;

namespace ResourceryWorkflow.Workflow.Requests;

[RemoteService(IsEnabled = false)]
public class RequestAppService : WorkflowAppService, IRequestAppService
{
    private readonly IRepository<Request, Guid> _requestRepository;
    private readonly IRepository<Department, Guid> _departmentRepository;
    private readonly IRepository<Service, Guid> _serviceRepository;

    public RequestAppService(
        IRepository<Request, Guid> requestRepository,
        IRepository<Department, Guid> departmentRepository,
        IRepository<Service, Guid> serviceRepository)
    {
        _requestRepository = requestRepository;
        _departmentRepository = departmentRepository;
        _serviceRepository = serviceRepository;
    }

    public async Task<RequestDto> GetAsync(Guid id)
    {
        var request = await _requestRepository.GetAsync(id);
        var dto = ObjectMapper.Map<Request, RequestDto>(request);
        await PopulateDisplayNamesAsync(new List<RequestDto> { dto });

        return dto;
    }

    public async Task<PagedResultDto<RequestDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var query = await _requestRepository.GetQueryableAsync();
        var totalCount = await _requestRepository.GetCountAsync();

        query = query.OrderByDescending(x => x.CreationTime)
            .PageBy(input.SkipCount, input.MaxResultCount);

        var items = await AsyncExecuter.ToListAsync(query);
        var dtos = items.Select(item => ObjectMapper.Map<Request, RequestDto>(item)).ToList();
        await PopulateDisplayNamesAsync(dtos);

        return new PagedResultDto<RequestDto>(totalCount, dtos);
    }

    public async Task<RequestDto> CreateAsync(CreateUpdateRequestDto input)
    {
        var request = new Request(
            GuidGenerator.Create(),
            input.DepartmentId,
            input.ServiceId,
            input.RequestedByUserId,
            input.Title,
            input.Description,
            input.Priority,
            input.TargetCompletionTime
        );

        await _requestRepository.InsertAsync(request, autoSave: true);
        var dto = ObjectMapper.Map<Request, RequestDto>(request);
        await PopulateDisplayNamesAsync(new List<RequestDto> { dto });

        return dto;
    }

    public async Task<RequestDto> UpdateAsync(Guid id, CreateUpdateRequestDto input)
    {
        var request = await _requestRepository.GetAsync(id);

        request.SetDepartment(input.DepartmentId);
        request.SetService(input.ServiceId);
        request.SetRequestedByUserId(input.RequestedByUserId);
        request.SetTitle(input.Title);
        request.SetDescription(input.Description);
        request.SetPriority(input.Priority);
        request.SetTargetCompletionTime(input.TargetCompletionTime);

        await _requestRepository.UpdateAsync(request, autoSave: true);
        var dto = ObjectMapper.Map<Request, RequestDto>(request);
        await PopulateDisplayNamesAsync(new List<RequestDto> { dto });

        return dto;
    }

    public async Task DeleteAsync(Guid id)
    {
        await _requestRepository.DeleteAsync(id);
    }

    private async Task PopulateDisplayNamesAsync(List<RequestDto> dtos)
    {
        if (dtos.Count == 0)
        {
            return;
        }

        var departmentIds = dtos.Select(dto => dto.DepartmentId).Distinct().ToList();
        var serviceIds = dtos.Select(dto => dto.ServiceId).Distinct().ToList();

        var departments = await _departmentRepository.GetListAsync(x => departmentIds.Contains(x.Id));
        var services = await _serviceRepository.GetListAsync(x => serviceIds.Contains(x.Id));

        var departmentLookup = departments.ToDictionary(department => department.Id, department => department.Name);
        var serviceLookup = services.ToDictionary(service => service.Id, service => service.Name);

        foreach (var dto in dtos)
        {
            if (departmentLookup.TryGetValue(dto.DepartmentId, out var departmentName))
            {
                dto.DepartmentName = departmentName;
            }

            if (serviceLookup.TryGetValue(dto.ServiceId, out var serviceName))
            {
                dto.ServiceName = serviceName;
            }
        }
    }
}
