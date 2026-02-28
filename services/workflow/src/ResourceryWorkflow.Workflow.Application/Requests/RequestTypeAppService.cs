using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp;

namespace ResourceryWorkflow.Workflow.Requests;

[RemoteService(IsEnabled = false)]
public class RequestTypeAppService : WorkflowAppService, IRequestTypeAppService
{
    private readonly IRepository<RequestType, Guid> _requestTypeRepository;

    public RequestTypeAppService(IRepository<RequestType, Guid> requestTypeRepository)
    {
        _requestTypeRepository = requestTypeRepository;
    }

    public async Task<RequestTypeDto> GetAsync(Guid id)
    {
        var entity = await _requestTypeRepository.GetAsync(id);
        return ObjectMapper.Map<RequestType, RequestTypeDto>(entity);
    }

    public async Task<PagedResultDto<RequestTypeDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var queryable = await _requestTypeRepository.GetQueryableAsync();
        var totalCount = await _requestTypeRepository.GetCountAsync();

        queryable = queryable
            .OrderBy(x => x.Name)
            .PageBy(input.SkipCount, input.MaxResultCount);

        var items = await AsyncExecuter.ToListAsync(queryable);
        var dtos = items.Select(x => ObjectMapper.Map<RequestType, RequestTypeDto>(x)).ToList();

        return new PagedResultDto<RequestTypeDto>(totalCount, dtos);
    }

    public async Task<RequestTypeDto> CreateAsync(CreateUpdateRequestTypeDto input)
    {
        var entity = new RequestType(
            GuidGenerator.Create(),
            input.Name,
            input.Description);

        await _requestTypeRepository.InsertAsync(entity, autoSave: true);
        return ObjectMapper.Map<RequestType, RequestTypeDto>(entity);
    }

    public async Task<RequestTypeDto> UpdateAsync(Guid id, CreateUpdateRequestTypeDto input)
    {
        var entity = await _requestTypeRepository.GetAsync(id);

        entity.SetName(input.Name);
        entity.SetDescription(input.Description);

        await _requestTypeRepository.UpdateAsync(entity, autoSave: true);
        return ObjectMapper.Map<RequestType, RequestTypeDto>(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _requestTypeRepository.DeleteAsync(id);
    }
}
