using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ResourceryWorkflow.Workflow.Services;
using ResourceryWorkflow.Workflow.Workflows;
using ResourceryWorkflow.Workflow.Workflows.Repositories;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace ResourceryWorkflow.Workflow;

public class WorkflowDataSeedContributor(
    ICurrentTenant currentTenant,
    IRepository<Service, Guid> serviceRepository,
    Volo.Abp.Domain.Repositories.IRepository<global::ResourceryWorkflow.Workflow.Workflows.Workflow, Guid> workflowRepository,
    Volo.Abp.Domain.Repositories.IRepository<global::ResourceryWorkflow.Workflow.Workflows.WorkflowStep, Guid> workflowStepRepository,
    IGuidGenerator guidGenerator
) : IDataSeedContributor, ITransientDependency
{
    private readonly ICurrentTenant _currentTenant = currentTenant;
    private readonly IRepository<Service, Guid> _serviceRepository = serviceRepository;
    private readonly Volo.Abp.Domain.Repositories.IRepository<global::ResourceryWorkflow.Workflow.Workflows.Workflow, Guid> _workflowRepository = workflowRepository;
    private readonly Volo.Abp.Domain.Repositories.IRepository<global::ResourceryWorkflow.Workflow.Workflows.WorkflowStep, Guid> _workflowStepRepository = workflowStepRepository;
    private readonly IGuidGenerator _guidGenerator = guidGenerator;

    public async Task SeedAsync(DataSeedContext context)
    {
        using (_currentTenant.Change(context?.TenantId))
        {
            var items = await LoadSeedItemsAsync();
            if (items.Count == 0) return;

            var existingWorkflows = await _workflowRepository.GetListAsync();
            var existingServiceIds = existingWorkflows.Select(w => w.ServiceId).ToHashSet();

            foreach (var item in items)
            {
                // Find service by Code
                var service = (await _serviceRepository.GetListAsync(s => s.Code == item.ServiceCode)).FirstOrDefault();
                if (service is null) continue;

                // Skip if workflow exists for this service and name
                if (existingWorkflows.Any(w => w.ServiceId == service.Id && w.Name == item.Name))
                {
                    continue;
                }

                var workflow = new ResourceryWorkflow.Workflow.Workflows.Workflow(
                    _guidGenerator.Create(),
                    service.Id,
                    item.Name,
                    item.Description,
                    isActive: item.IsActive
                );

                await _workflowRepository.InsertAsync(workflow, autoSave: true);

                var stepsToInsert = new List<ResourceryWorkflow.Workflow.Workflows.WorkflowStep>();
                var order = 0;
                foreach (var s in item.Steps ?? new List<SeedStep>())
                {
                    order = s.Order >= 0 ? s.Order : order + 1;
                    var step = new ResourceryWorkflow.Workflow.Workflows.WorkflowStep(
                        _guidGenerator.Create(),
                        workflow.Id,
                        s.Title,
                        order,
                        s.StepType,
                        s.Description,
                        assignedRoleId: null
                    );
                    stepsToInsert.Add(step);
                }

                if (stepsToInsert.Count > 0)
                {
                    await _workflowStepRepository.InsertManyAsync(stepsToInsert, autoSave: true);
                }
            }
        }
    }

    private static async Task<List<SeedWorkflowItem>> LoadSeedItemsAsync()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "Data", "json", "workflows.json");
        if (!File.Exists(filePath)) return new List<SeedWorkflowItem>();

        var json = await File.ReadAllTextAsync(filePath);
        var items = JsonSerializer.Deserialize<List<SeedWorkflowItem>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return items ?? new List<SeedWorkflowItem>();
    }

    private sealed class SeedWorkflowItem
    {
        public string ServiceCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public List<SeedStep> Steps { get; set; }
    }

    private sealed class SeedStep
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public WorkflowStepType StepType { get; set; } = WorkflowStepType.Approval;
    }
}
