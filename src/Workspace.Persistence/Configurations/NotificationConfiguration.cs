using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Workspace.Domain;

namespace Workspace.Persistence
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable(Constants.Notification);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.FromUser).IsRequired(true);
            builder.Property(x => x.ToUser).IsRequired(true);
            builder.Property(x => x.FunctionType).HasMaxLength(255).IsRequired(false);
            builder.Property(x => x.FunctionName).IsRequired(false);
            builder.Property(x => x.ObjId).IsRequired(true);
            builder.Property(x => x.CreatedDate).HasPrecision(6).HasDefaultValueSql("current_timestamp");
            builder.Property(x => x.Type).IsRequired(true);
            builder.Property(x => x.HaveSeen).HasDefaultValueSql("false");
            builder.Property(x => x.IsNew);
        }
    }
}
