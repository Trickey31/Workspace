﻿//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using Workspace.Domain;
//using Action = Workspace.Domain.Action;

//namespace Workspace.Persistence
//{
//    public class ActionConfiguration : IEntityTypeConfiguration<Action>
//    {
//        public void Configure(EntityTypeBuilder<Action> builder)
//        {
//            builder.ToTable(Constants.Actions);

//            builder.HasKey(x => x.Id);

//            builder.Property(x => x.Id).HasMaxLength(50);
//            builder.Property(x => x.Name).HasMaxLength(200).IsRequired(true);
//            builder.Property(x => x.IsActive).HasDefaultValue(true);
//            builder.Property(x => x.SortOrder).HasDefaultValue(null);

//            // Each User can have many Permission
//            builder.HasMany(e => e.Permissions)
//                .WithOne()
//                .HasForeignKey(p => p.ActionId)
//                .IsRequired();

//            // Each User can have many ActionInFunction
//            builder.HasMany(e => e.ActionInFunctions)
//                .WithOne()
//                .HasForeignKey(aif => aif.ActionId)
//                .IsRequired();
//        }
//    }
//}