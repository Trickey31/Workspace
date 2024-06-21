using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Workspace.Domain;

namespace Workspace.Persistence
{
    public class CTermConfiguration : IEntityTypeConfiguration<CTerm>
    {
        public void Configure(EntityTypeBuilder<CTerm> builder)
        {
            builder.ToTable(Constants.CTerms);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).HasMaxLength(255).IsRequired(true);
            builder.Property(x => x.Description).HasMaxLength(255).IsRequired(false);
            builder.Property(x => x.Type).HasMaxLength(255).IsRequired(false);
            builder.Property(x => x.CssClass).HasColumnType("text").IsRequired(false);
            builder.Property(x => x.CreatedDate).HasPrecision(6).HasDefaultValueSql("current_timestamp");
            builder.Property(x => x.UpdatedDate).HasPrecision(6);
            builder.Property(x => x.IsDelete);

            builder.HasMany(e => e.Project_CTerms)
                .WithOne()
                .HasForeignKey(p => p.TypeId)
                .IsRequired();
        }
    }
}
