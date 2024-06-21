using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Workspace.Domain;

namespace Workspace.Persistence
{
    public class UsersProjectsConfiguration : IEntityTypeConfiguration<Users_Projects>
    {
        public void Configure(EntityTypeBuilder<Users_Projects> builder)
        {
            builder.ToTable(Constants.Users_Projects);

            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserId).IsRequired(true);
            builder.Property(x => x.ProjectId).IsRequired(true);
            builder.Property(x => x.RoleId);
            builder.Property(x => x.CreatedDate).HasColumnType("timestamp without time zone");
            builder.Property(x => x.UpdatedDate).HasPrecision(6);
        }
    }
}
