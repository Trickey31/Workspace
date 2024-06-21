using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Workspace.Contract;

namespace Workspace.Presentation
{
    [ApiVersion(1)]
    public class CommentController : ApiController
    {
        public CommentController(ISender sender) : base(sender)
        {
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByTask([FromQuery] GetCommentByTaskQuery request)
        {
            var result = await Sender.Send(request);

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateCommentCommand request)
        {
            var result = await Sender.Send(request);

            if (result.IsFailed)
            {
                return HandlerFailure(result);
            }

            return Ok(result);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromBody] UpdateCommentCommand request)
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
            var result = await Sender.Send(new DeleteCommentCommand { Id = id });

            if (result.IsFailed)
            {
                return HandlerFailure(result);
            }

            return Ok(result);

        }
    }
}
