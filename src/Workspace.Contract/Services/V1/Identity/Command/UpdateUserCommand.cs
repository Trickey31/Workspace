namespace Workspace.Contract
{
    public class UpdateUserCommand : ICommand
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string UserName { get; set; }

        public string PhoneNumber { get; set; }

        public string ImgLink { get; set; }
    }
}
