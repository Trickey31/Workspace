using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Workspace.Domain;

namespace Workspace.Persistence
{
    public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable(Constants.Project);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).HasMaxLength(255).IsRequired(true);
            builder.Property(x => x.ImgLink);
            builder.Property(x => x.Description).HasMaxLength(255).IsRequired(false);
            builder.Property(x => x.Status).IsRequired(false);
            builder.Property(x => x.Slug).HasMaxLength(255);
            builder.Property(x => x.CreatedDate).HasPrecision(6).HasDefaultValueSql("current_timestamp");
            builder.Property(x => x.UpdatedDate).HasPrecision(6);
            builder.Property(x => x.IsDelete);

            builder.HasMany(e => e.Users_Projects)
                .WithOne()
                .HasForeignKey(p => p.ProjectId)
                .IsRequired();

            builder.HasMany(e => e.Tasks)
                .WithOne()
                .HasForeignKey(p => p.ProjectId)
                .IsRequired();
        }
    }
}
