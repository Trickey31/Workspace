﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Workspace.Persistence;

#nullable disable

namespace Workspace.Persistence.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240523023909_InitialMigration_20240523_v1")]
    partial class InitialMigration_20240523_v1
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.HasKey("RoleId");

                    b.ToTable("RoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.HasKey("UserId");

                    b.ToTable("UserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<string>("LoginProvider")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text");

                    b.Property<string>("ProviderKey")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("UserId");

                    b.ToTable("UserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("RoleId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("UserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<string>("LoginProvider")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("UserId");

                    b.ToTable("UserTokens", (string)null);
                });

            modelBuilder.Entity("Workspace.Domain.CTerm", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasPrecision(6)
                        .HasColumnType("timestamp(6) with time zone")
                        .HasDefaultValueSql("current_timestamp");

                    b.Property<string>("CssClass")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<int>("IsDelete")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Type")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasPrecision(6)
                        .HasColumnType("timestamp(6) with time zone");

                    b.HasKey("Id");

                    b.ToTable("CTerms", (string)null);
                });

            modelBuilder.Entity("Workspace.Domain.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasPrecision(6)
                        .HasColumnType("timestamp(6) with time zone")
                        .HasDefaultValueSql("current_timestamp");

                    b.Property<Guid>("TaskId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasPrecision(6)
                        .HasColumnType("timestamp(6) with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("Comment", (string)null);
                });

            modelBuilder.Entity("Workspace.Domain.Files", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasPrecision(6)
                        .HasColumnType("timestamp(6) with time zone")
                        .HasDefaultValueSql("current_timestamp");

                    b.Property<int>("IsDelete")
                        .HasColumnType("integer");

                    b.Property<string>("Link")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid?>("ObjId")
                        .HasColumnType("uuid");

                    b.Property<int>("ObjKey")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasPrecision(6)
                        .HasColumnType("timestamp(6) with time zone");

                    b.HasKey("Id");

                    b.ToTable("Files", (string)null);
                });

            modelBuilder.Entity("Workspace.Domain.Logs", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("AfterValue")
                        .HasColumnType("text");

                    b.Property<string>("Application")
                        .HasColumnType("text");

                    b.Property<string>("BeforeValue")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasPrecision(6)
                        .HasColumnType("timestamp(6) with time zone")
                        .HasDefaultValueSql("current_timestamp");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("FunctionName")
                        .HasColumnType("text");

                    b.Property<string>("FunctionType")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid?>("ObjId")
                        .HasColumnType("uuid");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.HasKey("Id");

                    b.ToTable("Log", (string)null);
                });

            modelBuilder.Entity("Workspace.Domain.Notification", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasPrecision(6)
                        .HasColumnType("timestamp(6) with time zone")
                        .HasDefaultValueSql("current_timestamp");

                    b.Property<Guid>("FromUser")
                        .HasColumnType("uuid");

                    b.Property<string>("FunctionName")
                        .HasColumnType("text");

                    b.Property<string>("FunctionType")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<bool>("HaveSeen")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValueSql("false");

                    b.Property<bool>("IsNew")
                        .HasColumnType("boolean");

                    b.Property<Guid>("ObjId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ToUser")
                        .HasColumnType("uuid");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Notification", (string)null);
                });

            modelBuilder.Entity("Workspace.Domain.Parent_CTerm", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("IsDelete")
                        .HasColumnType("integer");

                    b.Property<Guid>("ParentId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("TypeId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("TypeId");

                    b.ToTable("Parent_CTerms", (string)null);
                });

            modelBuilder.Entity("Workspace.Domain.Project", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasPrecision(6)
                        .HasColumnType("timestamp(6) with time zone")
                        .HasDefaultValueSql("current_timestamp");

                    b.Property<string>("Description")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("ImgLink")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("IsDelete")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<int?>("Status")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasPrecision(6)
                        .HasColumnType("timestamp(6) with time zone");

                    b.HasKey("Id");

                    b.ToTable("Project", (string)null);
                });

            modelBuilder.Entity("Workspace.Domain.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("NormalizedName")
                        .HasColumnType("text");

                    b.Property<string>("RoleCode")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.ToTable("Roles", (string)null);
                });

            modelBuilder.Entity("Workspace.Domain.Tasks", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasPrecision(6)
                        .HasColumnType("timestamp(6) with time zone")
                        .HasDefaultValueSql("current_timestamp");

                    b.Property<string>("Description")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("EndDate")
                        .HasPrecision(6)
                        .HasColumnType("timestamp(6) with time zone");

                    b.Property<int>("IsDelete")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<int?>("Priority")
                        .HasColumnType("integer");

                    b.Property<Guid?>("ProjectId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ReporterId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("StartDate")
                        .HasPrecision(6)
                        .HasColumnType("timestamp(6) with time zone");

                    b.Property<int?>("Status")
                        .HasColumnType("integer");

                    b.Property<Guid?>("Type")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasPrecision(6)
                        .HasColumnType("timestamp(6) with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.HasIndex("UserId");

                    b.ToTable("Task", (string)null);
                });

            modelBuilder.Entity("Workspace.Domain.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasPrecision(6)
                        .HasColumnType("timestamp(6) with time zone")
                        .HasDefaultValueSql("current_timestamp");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("ImgLink")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("text");

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("Workspace.Domain.Users_Projects", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("IsDelete")
                        .HasColumnType("integer");

                    b.Property<Guid>("ProjectId")
                        .HasColumnType("uuid");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasPrecision(6)
                        .HasColumnType("timestamp(6) with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.HasIndex("UserId");

                    b.ToTable("Users_Projects", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("Workspace.Domain.Role", null)
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("Workspace.Domain.User", null)
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("Workspace.Domain.User", null)
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("Workspace.Domain.Role", null)
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Workspace.Domain.User", null)
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.HasOne("Workspace.Domain.User", null)
                        .WithMany("Tokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Workspace.Domain.Parent_CTerm", b =>
                {
                    b.HasOne("Workspace.Domain.CTerm", null)
                        .WithMany("Project_CTerms")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Workspace.Domain.Tasks", b =>
                {
                    b.HasOne("Workspace.Domain.Project", null)
                        .WithMany("Tasks")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Workspace.Domain.User", null)
                        .WithMany("Tasks")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Workspace.Domain.Users_Projects", b =>
                {
                    b.HasOne("Workspace.Domain.Project", null)
                        .WithMany("Users_Projects")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Workspace.Domain.User", null)
                        .WithMany("Users_Projects")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Workspace.Domain.CTerm", b =>
                {
                    b.Navigation("Project_CTerms");
                });

            modelBuilder.Entity("Workspace.Domain.Project", b =>
                {
                    b.Navigation("Tasks");

                    b.Navigation("Users_Projects");
                });

            modelBuilder.Entity("Workspace.Domain.Role", b =>
                {
                    b.Navigation("Claims");

                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("Workspace.Domain.User", b =>
                {
                    b.Navigation("Claims");

                    b.Navigation("Logins");

                    b.Navigation("Tasks");

                    b.Navigation("Tokens");

                    b.Navigation("UserRoles");

                    b.Navigation("Users_Projects");
                });
#pragma warning restore 612, 618
        }
    }
}