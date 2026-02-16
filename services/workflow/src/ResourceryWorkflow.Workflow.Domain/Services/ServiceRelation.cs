using System;
using Volo.Abp.Domain.Entities;

namespace ResourceryWorkflow.Workflow.Services;

public class ServiceRelation : Entity<Guid>
{
    public Guid ServiceId { get; private set; }

    public Guid RelatedServiceId { get; private set; }

    private ServiceRelation()
    {
    }

    public ServiceRelation(Guid id, Guid serviceId, Guid relatedServiceId)
        : base(Service.EnsureNotEmpty(id, nameof(id)))
    {
        ServiceId = Service.EnsureNotEmpty(serviceId, nameof(serviceId));
        RelatedServiceId = Service.EnsureNotEmpty(relatedServiceId, nameof(relatedServiceId));
    }
}
