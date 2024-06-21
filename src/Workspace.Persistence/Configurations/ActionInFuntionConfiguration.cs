//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using Workspace.Domain;

//namespace Workspace.Persistence
//{
//    public class ActionInFuntionConfiguration : IEntityTypeConfiguration<ActionInFunction>
//    {
//        public void Configure(EntityTypeBuilder<ActionInFunction> builder)
//        {
//            builder.ToTable(Constants.ActionInFunctions);

//            builder.HasKey(x => new { x.ActionId, x.FunctionId });
//        }
//    }
//}
