namespace Workspace.Contract
{
    public class UserResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public string ImgLink { get; set; }
    }
}
