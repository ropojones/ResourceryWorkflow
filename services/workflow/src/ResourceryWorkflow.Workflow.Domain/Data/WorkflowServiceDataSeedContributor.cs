using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ResourceryWorkflow.Workflow.Departments;
using ResourceryWorkflow.Workflow.Services;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace ResourceryWorkflow.Workflow;

public class WorkflowServiceDataSeedContributor(
    ICurrentTenant currentTenant,
    IRepository<Department, System.Guid> departmentRepository,
    IRepository<Service, System.Guid> serviceRepository,
    IGuidGenerator guidGenerator
) : IDataSeedContributor, ITransientDependency
{
    private readonly ICurrentTenant _currentTenant = currentTenant;
    private readonly IRepository<Department, System.Guid> _departmentRepository = departmentRepository;
    private readonly IRepository<Service, System.Guid> _serviceRepository = serviceRepository;
    private readonly IGuidGenerator _guidGenerator = guidGenerator;

    public async Task SeedAsync(DataSeedContext context)
    {
        using (_currentTenant.Change(context?.TenantId))
        {
            var seedItems = await LoadSeedItemsAsync();
            if (seedItems.Count == 0)
            {
                return;
            }

            var existingCodes = new HashSet<string>(
                (await _serviceRepository.GetListAsync()).Select(item => item.Code)
            );

            var servicesToInsert = new List<Service>();

            foreach (var item in seedItems)
            {
                if (existingCodes.Contains(item.Code))
                {
                    continue;
                }

                var department = await _departmentRepository.FirstOrDefaultAsync(
                    departmentItem => departmentItem.Code == item.DeptCode
                );

                if (department is null)
                {
                    continue;
                }

                servicesToInsert.Add(new Service(
                    _guidGenerator.Create(),
                    department.Id,
                    item.Name,
                    item.Code,
                    item.Details ?? string.Empty,
                    item.Activities ?? string.Empty,
                    item.Outcomes ?? string.Empty,
                    item.Details ?? string.Empty,
                    item.HasChecklist,
                    defaultSlaHours: null,
                    isActive: true
                ));

                existingCodes.Add(item.Code);
            }

            if (servicesToInsert.Count == 0)
            {
                return;
            }

            await _serviceRepository.InsertManyAsync(servicesToInsert, autoSave: true);
        }
    }

    private static async Task<List<ServiceSeedItem>> LoadSeedItemsAsync()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "Data", "workflow.json");
        if (!File.Exists(filePath))
        {
            return new List<ServiceSeedItem>();
        }

        var json = await File.ReadAllTextAsync(filePath);
        var items = JsonSerializer.Deserialize<List<ServiceSeedItem>>(
            json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        return items ?? new List<ServiceSeedItem>();
    }

    private sealed class ServiceSeedItem
    {
        public string DeptCode { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        [JsonPropertyName("Actvities")]
        public string Activities { get; set; }

        [JsonPropertyName("Outcome")]
        public string Outcomes { get; set; }

        public string Details { get; set; }

        public bool HasChecklist { get; set; }
    }
}
