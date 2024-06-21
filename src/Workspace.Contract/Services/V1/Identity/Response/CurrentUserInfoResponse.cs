namespace Workspace.Contract
{
    public class CurrentUserInfoResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string ImgLink { get; set; }

        public IList<string> Roles { get; set; }
    }
}
