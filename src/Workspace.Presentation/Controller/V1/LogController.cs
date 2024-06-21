using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Workspace.Contract;

namespace Workspace.Presentation
{
    [ApiVersion(1)]
    [Authorize]
    public class LogController : ApiController
    {
        public LogController(ISender sender) : base(sender)
        {
        }

        [HttpGet]
        [ProducesResponseType(typeof(TResult<IEnumerable<LogResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFiles([FromQuery] GetLogByObjIdQuery request)
        {
            var result = await Sender.Send(request);

            return Ok(result);
        }

        [Authorize(Roles = "Super Admin")]
        [HttpGet("logs")]
        [ProducesResponseType(typeof(TResult<IEnumerable<LogResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLogs([FromQuery] GetLogsQuery request)
        {
            var result = await Sender.Send(request);

            return Ok(result);
        }
    }
}
