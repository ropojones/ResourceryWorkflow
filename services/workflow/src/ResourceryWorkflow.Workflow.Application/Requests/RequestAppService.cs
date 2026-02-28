using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResourceryWorkflow.Workflow.Requests;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp;

namespace ResourceryWorkflow.Workflow.Application.Requests;

[RemoteService(IsEnabled = false)]
public class RequestAppService : WorkflowAppService, IRequestAppService
{
    private readonly IRepository<Request, Guid> _requestRepository;
    private readonly IRepository<RequestType, Guid> _requestTypeRepository;

    public RequestAppService(
        IRepository<Request, Guid> requestRepository,
        IRepository<RequestType, Guid> requestTypeRepository)
    {
        _requestRepository = requestRepository;
        _requestTypeRepository = requestTypeRepository;
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
            input.RequestTypeId,
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

        request.SetRequestType(input.RequestTypeId);
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

        var typeIds = dtos.Select(dto => dto.RequestTypeId).Distinct().ToList();
        var types = await _requestTypeRepository.GetListAsync(x => typeIds.Contains(x.Id));
        var lookup = types.ToDictionary(x => x.Id, x => x.Name);

        foreach (var dto in dtos)
        {
            if (lookup.TryGetValue(dto.RequestTypeId, out var name))
            {
                dto.RequestTypeName = name;
            }
        }
    }
}