using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResourceryWorkflow.Workflow.Departments;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace ResourceryWorkflow.Workflow;

public class WorkflowDepartmentDataSeedContributor(
    ICurrentTenant currentTenant,
    IRepository<Department, System.Guid> departmentRepository,
    IGuidGenerator guidGenerator
) : IDataSeedContributor, ITransientDependency
{
    private readonly ICurrentTenant _currentTenant = currentTenant;
    private readonly IRepository<Department, System.Guid> _departmentRepository = departmentRepository;
    private readonly IGuidGenerator _guidGenerator = guidGenerator;

    public async Task SeedAsync(DataSeedContext context)
    {
        using (_currentTenant.Change(context?.TenantId))
        {
            var seedItems = new List<(string Name, string Code)>
            {
                ("Office of the Director", "OFD"),
                ("Translation", "TRANS"),
                ("Interpretation", "INT"),
                ("conference", "CONF"),
                ("protocol", "PROT"),
                ("transcription", "TRANC")
            };

            var names = seedItems.Select(item => item.Name).ToList();
            var codes = seedItems.Select(item => item.Code).ToList();

            var existing = await _departmentRepository.GetListAsync(
                department => names.Contains(department.Name) || codes.Contains(department.Code)
            );
            var existingNames = new HashSet<string>(existing.Select(department => department.Name));
            var existingCodes = new HashSet<string>(existing.Select(department => department.Code));

            var newDepartments = seedItems
                .Where(item => !existingNames.Contains(item.Name) && !existingCodes.Contains(item.Code))
                .Select(item => new Department(
                    _guidGenerator.Create(),
                    item.Name,
                    item.Code,
                    item.Name,
                    isActive: true
                ))
                .ToList();

            if (newDepartments.Count == 0)
            {
                return;
            }

            await _departmentRepository.InsertManyAsync(newDepartments, autoSave: true);
        }
    }
}
