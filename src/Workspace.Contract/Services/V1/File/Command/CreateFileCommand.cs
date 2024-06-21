using Microsoft.AspNetCore.Http;

namespace Workspace.Contract
{
    public class CreateFileCommand : ICommand<FileCommandResponse>
    {
        public IFormFile File { get; set; }

        public Guid? ObjId { get; set; }

        public int ObjKey { get; set; }
    }
}
