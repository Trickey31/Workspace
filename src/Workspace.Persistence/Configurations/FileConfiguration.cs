using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Workspace.Domain;

namespace Workspace.Persistence
{
    public sealed class FileConfiguration : IEntityTypeConfiguration<Files>
    {
        public void Configure(EntityTypeBuilder<Files> builder)
        {
            builder.ToTable(Constants.Files);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).HasMaxLength(255).IsRequired(true);
            builder.Property(x => x.ObjId).IsRequired(false);
            builder.Property(x => x.ObjKey).IsRequired(true);
            builder.Property(x => x.Link).IsRequired(true);
            builder.Property(x => x.CreatedDate).HasPrecision(6).HasDefaultValueSql("current_timestamp");
            builder.Property(x => x.UpdatedDate).HasPrecision(6);
            builder.Property(x => x.IsDelete);
        }
    }
}
