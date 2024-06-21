using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Workspace.Domain;

namespace Workspace.Persistence
{
    public sealed class ApplicationDbContext : IdentityDbContext<User, Role, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
            => builder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);

        public DbSet<User> Users { get; set; }

        //public DbSet<Action> Actions { get; set; }

        //public DbSet<Function> Functions { get; set; }

        //public DbSet<ActionInFunction> ActionInFunctions { get; set; }

        //public DbSet<Permission> Permissions { get; set; }

        public DbSet<Tasks> Tasks { get; set; }

        public DbSet<Users_Projects> UsersProjects { get; set; }

        public DbSet<CTerm> CTerms { get; set; }

        public DbSet<Parent_CTerm> Parent_CTerms { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<Comment> Comments { get; set; }
    }
}