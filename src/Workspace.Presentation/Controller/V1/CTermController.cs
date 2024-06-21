using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Workspace.Contract;

namespace Workspace.Presentation
{
    [ApiVersion(1)]
    public class CTermController : ApiController
    {
        public CTermController(ISender sender) : base(sender)
        {
        }

        [HttpGet("type-project")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByTypeAndProject([FromQuery] GetListCTermByTypeAndProjectQuery request)
        {
            var result = await Sender.Send(request);

            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByType([FromQuery] GetListCTermByTypeQuery request)
        {
            var result = await Sender.Send(request);

            return Ok(result);
        }

        [HttpGet("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await Sender.Send(new GetByIdQuery { Id = id});

            return Ok(result);
        }

        [HttpGet("get-by-current-user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByCurrentUser([FromQuery] GetListCTermByCurrentUserQuery request)
        {
            var result = await Sender.Send(request);

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateCTermCommand request)
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
        public async Task<IActionResult> Update([FromBody] UpdateCTermCommand request)
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
            var result = await Sender.Send(new DeleteCTermCommand { Id = id });

            if (result.IsFailed)
            {
                return HandlerFailure(result);
            }

            return Ok(result);

        }
    }
}
