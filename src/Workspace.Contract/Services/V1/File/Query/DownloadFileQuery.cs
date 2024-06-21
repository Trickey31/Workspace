using Microsoft.AspNetCore.Mvc;

namespace Workspace.Contract
{
    public class DownloadFileQuery : IQuery<IActionResult>
    {
        public Guid Id { get; set; }
    }
}
