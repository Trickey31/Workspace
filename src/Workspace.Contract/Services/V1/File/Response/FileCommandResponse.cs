namespace Workspace.Contract
{
    public class FileCommandResponse
    {
        public string Link { get; set; }

        public FileCommandResponse(string link)
        {
            Link = link;
        }
    }
}
