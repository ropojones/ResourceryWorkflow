using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResourceryWorkflow.Workflow.Workflows.Repositories;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Linq;
using Volo.Abp;

namespace ResourceryWorkflow.Workflow.Workflows;

[RemoteService(IsEnabled = false)]
public class RequestWorkflowAppService : WorkflowAppService, IRequestWorkflowAppService
{
    private readonly IRequestWorkflowRepository _requestWorkflowRepository;

    public RequestWorkflowAppService(
        IWorkflowRepository workflowRepository,
        IWorkflowStepRepository workflowStepRepository,
        IRequestWorkflowRepository requestWorkflowRepository)
        : base(workflowRepository, workflowStepRepository)
    {
        _requestWorkflowRepository = requestWorkflowRepository;
    }

    public async Task<RequestWorkflowDto> CreateAsync(CreateUpdateRequestWorkflowDto input)
    {
        var entity = new RequestWorkflow(GuidGenerator.Create(), input.RequestId, input.WorkflowId);
        if (input.CurrentStepId != null)
        {
            entity.SetCurrentStep(input.CurrentStepId);
        }
        await _requestWorkflowRepository.InsertAsync(entity, autoSave: true);

        return new RequestWorkflowDto
        {
            Id = entity.Id,
            RequestId = entity.RequestId,
            WorkflowId = entity.WorkflowId,
            CurrentStepId = entity.CurrentStepId,
            Status = entity.Status
        };
    }

    public async Task<RequestWorkflowDto> UpdateAsync(Guid id, CreateUpdateRequestWorkflowDto input)
    {
        var entity = await _requestWorkflowRepository.GetAsync(id);
        entity.SetRequest(input.RequestId);
        entity.SetWorkflow(input.WorkflowId);
        entity.SetCurrentStep(input.CurrentStepId);
        // map status
        switch (input.Status)
        {
            case RequestWorkflowStatus.InProgress:
                entity.Start();
                break;
            case RequestWorkflowStatus.Completed:
                entity.Complete();
                break;
            case RequestWorkflowStatus.Cancelled:
                entity.Cancel();
                break;
            default:
                break;
        }

        await _requestWorkflowRepository.UpdateAsync(entity, autoSave: true);

        return new RequestWorkflowDto
        {
            Id = entity.Id,
            RequestId = entity.RequestId,
            WorkflowId = entity.WorkflowId,
            CurrentStepId = entity.CurrentStepId,
            Status = entity.Status
        };
    }

    public async Task DeleteAsync(Guid id)
    {
        await _requestWorkflowRepository.DeleteAsync(id);
    }

    public async Task<RequestWorkflowDto> GetAsync(Guid id)
    {
        var entity = await _requestWorkflowRepository.GetAsync(id);
        return new RequestWorkflowDto
        {
            Id = entity.Id,
            RequestId = entity.RequestId,
            WorkflowId = entity.WorkflowId,
            CurrentStepId = entity.CurrentStepId,
            Status = entity.Status
        };
    }

    public async Task<PagedResultDto<RequestWorkflowDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var query = await _requestWorkflowRepository.GetQueryableAsync();
        var totalCount = await _requestWorkflowRepository.GetCountAsync();

        query = query.OrderBy(x => x.Id)
            .PageBy(input.SkipCount, input.MaxResultCount);

        var items = await AsyncExecuter.ToListAsync(query);

        var dtos = items.Select(entity => new RequestWorkflowDto
        {
            Id = entity.Id,
            RequestId = entity.RequestId,
            WorkflowId = entity.WorkflowId,
            CurrentStepId = entity.CurrentStepId,
            Status = entity.Status
        }).ToList();

        return new PagedResultDto<RequestWorkflowDto>(totalCount, dtos);
    }
}
