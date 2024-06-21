using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Workspace.Domain;

namespace Workspace.Persistence
{
    public class ProjectCTermConfiguration : IEntityTypeConfiguration<Parent_CTerm>
    {
        public void Configure(EntityTypeBuilder<Parent_CTerm> builder)
        {
            builder.ToTable(Constants.Parent_CTerms);

            builder.HasKey(x => x.Id);
            builder.Property(x => x.TypeId).IsRequired(true);
            builder.Property(x => x.ParentId).IsRequired(true);
        }
    }
}
