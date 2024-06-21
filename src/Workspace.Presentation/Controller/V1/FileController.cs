using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Workspace.Contract;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Workspace.Presentation
{
    [ApiVersion(1)]
    [Authorize]
    public class FileController : ApiController
    {
        public FileController(ISender sender) : base(sender)
        {
        }

        [HttpGet]
        [ProducesResponseType(typeof(TResult<IEnumerable<FileQueryResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFiles([FromQuery] GetFilesQuery request)
        {
            var result = await Sender.Send(request);

            return Ok(result);
        }

        [HttpGet]
        [Route("download-file")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Download(Guid id)
        {
            var result = await Sender.Send(new DownloadFileQuery { Id = id });

            return result.Value;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromForm] CreateFileCommand request)
        {
            var result = await Sender.Send(request);

            if (result.IsFailed)
            {
                return HandlerFailure(result);
            }

            return Ok(result);
        }

        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await Sender.Send(new DeleteFileCommand { Id = id });

            return Ok(result);

        }
    }
}
