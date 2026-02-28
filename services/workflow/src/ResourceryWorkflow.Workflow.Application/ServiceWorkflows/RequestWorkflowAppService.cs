using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResourceryWorkflow.Workflow.ServiceWorkflows;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;

namespace ResourceryWorkflow.Workflow.ServiceWorkflows;

[RemoteService(IsEnabled = false)]
public class RequestWorkflowAppService : ApplicationService, IRequestWorkflowAppService
{
    private readonly IRepository<RequestWorkflow, Guid> _requestWorkflowRepository;

    public RequestWorkflowAppService(
        IRepository<RequestWorkflow, Guid> requestWorkflowRepository)
    {
        _requestWorkflowRepository = requestWorkflowRepository;
    }

    public async Task<RequestWorkflowDto> CreateAsync(CreateUpdateRequestWorkflowDto input)
    {
        var entity = new RequestWorkflow(GuidGenerator.Create(), input.RequestId, input.ServiceWorkflowId);
        if (input.CurrentStepId != null)
        {
            entity.SetCurrentStep(input.CurrentStepId);
        }
        await _requestWorkflowRepository.InsertAsync(entity, autoSave: true);

        return new RequestWorkflowDto
        {
            Id = entity.Id,
            RequestId = entity.RequestId,
            ServiceWorkflowId = entity.ServiceWorkflowId,
            CurrentStepId = entity.CurrentStepId,
            Status = entity.Status
        };
    }

    public async Task<RequestWorkflowDto> UpdateAsync(Guid id, CreateUpdateRequestWorkflowDto input)
    {
        var entity = await _requestWorkflowRepository.GetAsync(id);
        entity.SetRequest(input.RequestId);
        entity.SetServiceWorkflow(input.ServiceWorkflowId);
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
            ServiceWorkflowId = entity.ServiceWorkflowId,
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
            ServiceWorkflowId = entity.ServiceWorkflowId,
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
            ServiceWorkflowId = entity.ServiceWorkflowId,
            CurrentStepId = entity.CurrentStepId,
            Status = entity.Status
        }).ToList();

        return new PagedResultDto<RequestWorkflowDto>(totalCount, dtos);
    }
}
