using Microsoft.AspNetCore.Identity;

namespace Workspace.Domain
{
    public class User : IdentityUser<Guid>
    {
        public string Name { get; set; }

        public string ImgLink { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual ICollection<IdentityUserClaim<Guid>> Claims { get; set; }
        public virtual ICollection<IdentityUserLogin<Guid>> Logins { get; set; }
        public virtual ICollection<IdentityUserToken<Guid>> Tokens { get; set; }
        public virtual ICollection<IdentityUserRole<Guid>> UserRoles { get; set; }
        public virtual ICollection<Tasks> Tasks { get; set; }
        public virtual ICollection<Users_Projects> Users_Projects { get; set; }
    }
}
