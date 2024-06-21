using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Workspace.Contract;

namespace Workspace.Presentation
{
    [Authorize]
    public class NotificationController : ApiController
    {
        public NotificationController(ISender sender) : base(sender)
        {
        }

        [HttpGet]
        [ProducesResponseType(typeof(TResult<IEnumerable<NotificationResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetNotification()
        {
            var result = await Sender.Send(new GetNotificationsByUserIdQuery());

            return Ok(result);
        }

        [HttpPut("is-new")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update()
        {
            var result = await Sender.Send(new UpdateIsNewCommand());

            if (result.IsFailed)
            {
                return HandlerFailure(result);
            }

            return Ok(result);
        }
    }
}
