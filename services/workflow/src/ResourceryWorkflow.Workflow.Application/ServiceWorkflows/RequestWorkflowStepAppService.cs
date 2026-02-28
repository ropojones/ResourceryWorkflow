using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using ResourceryWorkflow.Workflow.ServiceWorkflows;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;

namespace ResourceryWorkflow.Workflow.ServiceWorkflows;

[RemoteService(IsEnabled = false)]
public class RequestWorkflowStepAppService : ApplicationService, IRequestWorkflowStepAppService
{
    private readonly IRepository<RequestWorkflowStep, Guid> _stepRepository;

    public RequestWorkflowStepAppService(
        IRepository<RequestWorkflowStep, Guid> stepRepository)
    {
        _stepRepository = stepRepository;
    }

    public async Task<RequestWorkflowStepDto> CreateAsync(CreateUpdateRequestWorkflowStepDto input)
    {
        var entity = new RequestWorkflowStep(GuidGenerator.Create(), input.RequestWorkflowId, input.ServiceWorkflowStepId, input.AssignedToUserId);
        await _stepRepository.InsertAsync(entity, autoSave: true);

        return new RequestWorkflowStepDto
        {
            Id = entity.Id,
            RequestWorkflowId = entity.RequestWorkflowId,
            ServiceWorkflowStepId = entity.ServiceWorkflowStepId,
            AssignedToUserId = entity.AssignedToUserId,
            Status = entity.Status,
            CompletedAt = entity.CompletedAt,
            Comments = entity.Comments
        };
    }

    public async Task<RequestWorkflowStepDto> UpdateAsync(Guid id, CreateUpdateRequestWorkflowStepDto input)
    {
        var entity = await _stepRepository.GetAsync(id);
        entity.SetRequestWorkflow(input.RequestWorkflowId);
        entity.SetServiceWorkflowStep(input.ServiceWorkflowStepId);
        if (input.AssignedToUserId != null)
        {
            entity.AssignTo(input.AssignedToUserId.Value);
        }

        if (input.Status == RequestWorkflowStepStatus.Completed)
        {
            entity.Complete(input.Comments);
        }
        else if (input.Status == RequestWorkflowStepStatus.Skipped)
        {
            entity.Skip(input.Comments);
        }

        await _stepRepository.UpdateAsync(entity, autoSave: true);

        return new RequestWorkflowStepDto
        {
            Id = entity.Id,
            RequestWorkflowId = entity.RequestWorkflowId,
            ServiceWorkflowStepId = entity.ServiceWorkflowStepId,
            AssignedToUserId = entity.AssignedToUserId,
            Status = entity.Status,
            CompletedAt = entity.CompletedAt,
            Comments = entity.Comments
        };
    }

    public async Task DeleteAsync(Guid id)
    {
        await _stepRepository.DeleteAsync(id);
    }

    public async Task<RequestWorkflowStepDto> GetAsync(Guid id)
    {
        var entity = await _stepRepository.GetAsync(id);
        return new RequestWorkflowStepDto
        {
            Id = entity.Id,
            RequestWorkflowId = entity.RequestWorkflowId,
            ServiceWorkflowStepId = entity.ServiceWorkflowStepId,
            AssignedToUserId = entity.AssignedToUserId,
            Status = entity.Status,
            CompletedAt = entity.CompletedAt,
            Comments = entity.Comments
        };
    }

    public async Task<PagedResultDto<RequestWorkflowStepDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var query = await _stepRepository.GetQueryableAsync();
        var totalCount = await _stepRepository.GetCountAsync();

        query = query.OrderBy(x => x.Id)
            .PageBy(input.SkipCount, input.MaxResultCount);

        var items = await AsyncExecuter.ToListAsync(query);

        var dtos = items.Select(entity => new RequestWorkflowStepDto
        {
            Id = entity.Id,
            RequestWorkflowId = entity.RequestWorkflowId,
            ServiceWorkflowStepId = entity.ServiceWorkflowStepId,
            AssignedToUserId = entity.AssignedToUserId,
            Status = entity.Status,
            CompletedAt = entity.CompletedAt,
            Comments = entity.Comments
        }).ToList();

        return new PagedResultDto<RequestWorkflowStepDto>(totalCount, dtos);
    }
}
