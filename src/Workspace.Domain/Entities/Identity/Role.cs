using Microsoft.AspNetCore.Identity;

namespace Workspace.Domain
{
    public class Role : IdentityRole<Guid>
    {
        public string Description { get; set; }
        public string RoleCode { get; set; }

        public virtual ICollection<IdentityUserRole<Guid>> UserRoles { get; set; }
        public virtual ICollection<IdentityRoleClaim<Guid>> Claims { get; set; }
        //public virtual ICollection<Permission> Permissions { get; set; }
    }
}
