using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tasks = Workspace.Domain.Tasks;

namespace Workspace.Persistence
{
    public sealed class TaskConfiguration : IEntityTypeConfiguration<Tasks>
    {
        public void Configure(EntityTypeBuilder<Tasks> builder)
        {
            builder.ToTable(Constants.Task);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).HasMaxLength(255).IsRequired(true);
            builder.Property(x => x.Type).IsRequired(false);
            builder.Property(x => x.Description).HasMaxLength(255).IsRequired(false);
            builder.Property(x => x.Status).IsRequired(false);
            builder.Property(x => x.StartDate).HasPrecision(6);
            builder.Property(x => x.EndDate).HasPrecision(6);
            builder.Property(x => x.ProjectId).IsRequired(false);
            builder.Property(x => x.UserId).IsRequired(true);
            builder.Property(x => x.ReporterId).IsRequired(true);
            builder.Property(x => x.Priority);
            builder.Property(x => x.CreatedDate).HasPrecision(6).HasDefaultValueSql("current_timestamp");
            builder.Property(x => x.UpdatedDate).HasPrecision(6);
            builder.Property(x => x.IsDelete);
        }
    }
}
