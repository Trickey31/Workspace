using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Workspace.Domain;

namespace Workspace.Persistence
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable(Constants.Comment);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId).IsRequired(true);
            builder.Property(x => x.TaskId).IsRequired(true);
            builder.Property(x => x.Content).HasColumnType("text").IsRequired(true);
            builder.Property(x => x.CreatedDate).HasPrecision(6).HasDefaultValueSql("current_timestamp");
            builder.Property(x => x.UpdatedDate).HasPrecision(6).IsRequired(false);
        }
    }
}
