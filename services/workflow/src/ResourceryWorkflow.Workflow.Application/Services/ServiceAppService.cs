using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResourceryWorkflow.Workflow.Departments;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;
using Volo.Abp;

namespace ResourceryWorkflow.Workflow.Services;

[RemoteService(IsEnabled = false)]
public class ServiceAppService : WorkflowAppService, IServiceAppService
{
    private readonly IRepository<Service, Guid> _serviceRepository;
    private readonly IRepository<Department, Guid> _departmentRepository;

    public ServiceAppService(
        IRepository<Service, Guid> serviceRepository,
        IRepository<Department, Guid> departmentRepository)
    {
        _serviceRepository = serviceRepository;
        _departmentRepository = departmentRepository;
    }

    public async Task<ServiceDto> GetAsync(Guid id)
    {
        var service = await _serviceRepository.GetAsync(id);
        var dto = ObjectMapper.Map<Service, ServiceDto>(service);

        var department = await _departmentRepository.FindAsync(service.DepartmentId);
        dto.DepartmentName = department?.Name;

        return dto;
    }

    public async Task<PagedResultDto<ServiceDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var query = await _serviceRepository.GetQueryableAsync();
        var totalCount = await _serviceRepository.GetCountAsync();

        query = query.OrderBy(x => x.Name)
            .PageBy(input.SkipCount, input.MaxResultCount);

        var items = await AsyncExecuter.ToListAsync(query);
        var dtos = items.Select(item => ObjectMapper.Map<Service, ServiceDto>(item)).ToList();
        await PopulateDepartmentNamesAsync(dtos);

        return new PagedResultDto<ServiceDto>(totalCount, dtos);
    }

    public async Task<ServiceDto> CreateAsync(CreateUpdateServiceDto input)
    {
        var service = new Service(
            GuidGenerator.Create(),
            input.DepartmentId,
            input.Name,
            input.Code,
            input.Description,
            input.Activities,
            input.Outcomes,
            input.Details,
            input.HasChecklist,
            input.DefaultSlaHours,
            input.IsActive
        );

        await _serviceRepository.InsertAsync(service, autoSave: true);
        var dto = ObjectMapper.Map<Service, ServiceDto>(service);
        await PopulateDepartmentNamesAsync(new List<ServiceDto> { dto });
        return dto;
    }

    public async Task<ServiceDto> UpdateAsync(Guid id, CreateUpdateServiceDto input)
    {
        var service = await _serviceRepository.GetAsync(id);

        service.SetDepartment(input.DepartmentId);
        service.SetName(input.Name);
        service.SetCode(input.Code);
        service.SetDescription(input.Description);
        service.SetActivities(input.Activities);
        service.SetOutcomes(input.Outcomes);
        service.SetDetails(input.Details);
        service.SetHasChecklist(input.HasChecklist);
        service.SetDefaultSlaHours(input.DefaultSlaHours);

        if (input.IsActive)
        {
            service.Activate();
        }
        else
        {
            service.Deactivate();
        }

        await _serviceRepository.UpdateAsync(service, autoSave: true);
        var dto = ObjectMapper.Map<Service, ServiceDto>(service);
        await PopulateDepartmentNamesAsync(new List<ServiceDto> { dto });
        return dto;
    }

    public async Task DeleteAsync(Guid id)
    {
        await _serviceRepository.DeleteAsync(id);
    }

    private async Task PopulateDepartmentNamesAsync(List<ServiceDto> dtos)
    {
        if (dtos.Count == 0)
        {
            return;
        }

        var departmentIds = dtos.Select(dto => dto.DepartmentId).Distinct().ToList();
        var departments = await _departmentRepository.GetListAsync(x => departmentIds.Contains(x.Id));
        var lookup = departments.ToDictionary(department => department.Id, department => department.Name);

        foreach (var dto in dtos)
        {
            if (lookup.TryGetValue(dto.DepartmentId, out var name))
            {
                dto.DepartmentName = name;
            }
        }
    }
}
