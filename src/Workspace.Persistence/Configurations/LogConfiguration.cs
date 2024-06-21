using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Workspace.Domain;

namespace Workspace.Persistence
{
    public class LogConfiguration : IEntityTypeConfiguration<Logs>
    {
        public void Configure(EntityTypeBuilder<Logs> builder)
        {
            builder.ToTable(Constants.Log);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserName).HasMaxLength(255).IsRequired(true);
            builder.Property(x => x.FullName).HasMaxLength(255).IsRequired(true);
            builder.Property(x => x.FunctionType).HasMaxLength(255).IsRequired(false);
            builder.Property(x => x.FunctionName).IsRequired(false);
            builder.Property(x => x.Application).IsRequired(false);
            builder.Property(x => x.BeforeValue).HasColumnType("text").IsRequired(false);
            builder.Property(x => x.AfterValue).HasColumnType("text").IsRequired(false);
            builder.Property(x => x.ObjId).IsRequired(false);
            builder.Property(x => x.CreatedDate).HasPrecision(6).HasDefaultValueSql("current_timestamp");
        }
    }
}
