using Microsoft.EntityFrameworkCore;
using ResourceryWorkflow.Workflow.Departments;
using ResourceryWorkflow.Workflow.Requests;
using ResourceryWorkflow.Workflow.Services;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace ResourceryWorkflow.Workflow.EntityFrameworkCore;

public static class WorkflowDbContextModelCreatingExtensions
{
    public static void ConfigureWorkflow(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        builder.Entity<Department>(b =>
        {
            b.ToTable(WorkflowDbProperties.DbTablePrefix + "Departments", WorkflowDbProperties.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(DepartmentConsts.MaxNameLength);
            b.Property(x => x.Code).IsRequired().HasMaxLength(DepartmentConsts.MaxCodeLength);
            b.Property(x => x.Description).HasMaxLength(DepartmentConsts.MaxDescriptionLength);

            b.HasIndex(x => x.Code).IsUnique();
            b.HasIndex(x => x.Name);
        });

        builder.Entity<Service>(b =>
        {
            b.ToTable(WorkflowDbProperties.DbTablePrefix + "Services", WorkflowDbProperties.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.DepartmentId).IsRequired();
            b.Property(x => x.Name).IsRequired().HasMaxLength(ServiceConsts.MaxNameLength);
            b.Property(x => x.Code).IsRequired().HasMaxLength(ServiceConsts.MaxCodeLength);
            b.Property(x => x.Description).HasMaxLength(ServiceConsts.MaxDescriptionLength);
            b.Property(x => x.Activities).HasMaxLength(ServiceConsts.MaxActivitiesLength);
            b.Property(x => x.Outcomes).HasMaxLength(ServiceConsts.MaxOutcomesLength);
            b.Property(x => x.Details).HasMaxLength(ServiceConsts.MaxDetailsLength);
            b.Property(x => x.HasChecklist).HasDefaultValue(false);

            b.HasIndex(x => x.Code).IsUnique();
            b.HasIndex(x => x.DepartmentId);
            b.HasIndex(x => x.Name);
        });

        builder.Entity<ServiceRelation>(b =>
        {
            b.ToTable(WorkflowDbProperties.DbTablePrefix + "ServiceRelations", WorkflowDbProperties.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.ServiceId).IsRequired();
            b.Property(x => x.RelatedServiceId).IsRequired();

            b.HasIndex(x => new { x.ServiceId, x.RelatedServiceId }).IsUnique();

            b.HasOne<Service>()
                .WithMany(x => x.RelatedServices)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne<Service>()
                .WithMany()
                .HasForeignKey(x => x.RelatedServiceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Request>(b =>
        {
            b.ToTable(WorkflowDbProperties.DbTablePrefix + "Requests", WorkflowDbProperties.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.DepartmentId).IsRequired();
            b.Property(x => x.ServiceId).IsRequired();
            b.Property(x => x.RequestedByUserId).IsRequired();
            b.Property(x => x.Title).IsRequired().HasMaxLength(RequestConsts.MaxTitleLength);
            b.Property(x => x.Description).HasMaxLength(RequestConsts.MaxDescriptionLength);
            b.Property(x => x.Status).IsRequired();
            b.Property(x => x.Priority).IsRequired();

            b.HasIndex(x => x.ServiceId);
            b.HasIndex(x => x.Status);
            b.HasIndex(x => x.CreationTime);
        });
    }
}
