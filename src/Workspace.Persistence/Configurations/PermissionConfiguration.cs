//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using Workspace.Domain;

//namespace Workspace.Persistence
//{
//    public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
//    {
//        public void Configure(EntityTypeBuilder<Permission> builder)
//        {
//            builder.ToTable(Constants.Permissions);

//            builder.HasKey(x => new { x.RoleId, x.FunctionId, x.ActionId });
//        }
//    }
//}
