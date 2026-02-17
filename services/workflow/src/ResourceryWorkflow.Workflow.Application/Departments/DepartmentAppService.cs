using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;
using Volo.Abp;

namespace ResourceryWorkflow.Workflow.Departments;

[RemoteService(IsEnabled = false)]
public class DepartmentAppService : WorkflowAppService, IDepartmentAppService
{
    private readonly IRepository<Department, Guid> _departmentRepository;

    public DepartmentAppService(IRepository<Department, Guid> departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public async Task<DepartmentDto> GetAsync(Guid id)
    {
        var department = await _departmentRepository.GetAsync(id);
        return ObjectMapper.Map<Department, DepartmentDto>(department);
    }

    public async Task<PagedResultDto<DepartmentDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var query = await _departmentRepository.GetQueryableAsync();
        var totalCount = await _departmentRepository.GetCountAsync();

        query = query.OrderBy(x => x.Name)
            .PageBy(input.SkipCount, input.MaxResultCount);

        var items = await AsyncExecuter.ToListAsync(query);
        var dtos = items.Select(item => ObjectMapper.Map<Department, DepartmentDto>(item)).ToList();

        return new PagedResultDto<DepartmentDto>(totalCount, dtos);
    }

    public async Task<DepartmentDto> CreateAsync(CreateUpdateDepartmentDto input)
    {
        var department = new Department(
            GuidGenerator.Create(),
            input.Name,
            input.Code,
            input.Description,
            input.IsActive
        );

        await _departmentRepository.InsertAsync(department, autoSave: true);
        return ObjectMapper.Map<Department, DepartmentDto>(department);
    }

    public async Task<DepartmentDto> UpdateAsync(Guid id, CreateUpdateDepartmentDto input)
    {
        var department = await _departmentRepository.GetAsync(id);

        department.SetName(input.Name);
        department.SetCode(input.Code);
        department.SetDescription(input.Description);

        if (input.IsActive)
        {
            department.Activate();
        }
        else
        {
            department.Deactivate();
        }

        await _departmentRepository.UpdateAsync(department, autoSave: true);
        return ObjectMapper.Map<Department, DepartmentDto>(department);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _departmentRepository.DeleteAsync(id);
    }

}
