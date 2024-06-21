using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Workspace.Contract;

namespace Workspace.Presentation
{
    [Authorize]
    [ApiVersion(1)]
    public class AuthController : ApiController
    {
        public AuthController(ISender sender) : base(sender)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetUserInfo()
        {
            var result = await Sender.Send(new GetCurrentUserInfoQuery());

            return Ok(result);
        }

        [HttpGet("{int:Guid}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var result = await Sender.Send(new GetUserByIdQuery { Id = id});

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("check-exist")]
        public async Task<IActionResult> CheckExist([FromQuery] GetUserQuery request)
        {
            var result = await Sender.Send(request);

            return Ok(result);
        }

        [Authorize(Roles = "Super Admin")]
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery]GetListUserQuery request)
        {
            var result = await Sender.Send(request);

            return Ok(result);
        }

        [HttpGet("assignee")]
        public async Task<IActionResult> GetAssigneeInfo([FromQuery] GetListAssigneeByUserQuery request)
        {
            var result = await Sender.Send(request);

            return Ok(result);
        }

        [HttpGet("user-in-project")]
        public async Task<IActionResult> GetUserInProject([FromQuery] GetUserInProjectQuery request)
        {
            var result = await Sender.Send(request);

            return Ok(result);
        }

        [Authorize(Roles = "Super Admin")]
        [HttpGet("statistic-by-year")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByYear([FromQuery] GetUserStatisticByYearQuery request)
        {
            var result = await Sender.Send(request);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Login([FromBody] AuthenCommand.Login login)
        {
            var result = await Sender.Send(login);

            if (result.IsFailed)
            {
                return HandlerFailure(result);
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("send-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailCommand request)
        {
            var result = await Sender.Send(request);

            if (result.IsFailed)
            {
                return HandlerFailure(result);
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("verify-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailCommand request)
        {
            var result = await Sender.Send(request);

            if (result.IsFailed)
            {
                return HandlerFailure(result);
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Register([FromBody] CreateUserCommand request)
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
        public async Task<IActionResult> Update([FromBody] UpdateUserCommand request)
        {
            var result = await Sender.Send(request);

            if (result.IsFailed)
            {
                return HandlerFailure(result);
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPut("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand request)
        {
            var result = await Sender.Send(request);

            if (result.IsFailed)
            {
                return HandlerFailure(result);
            }

            return Ok(result);
        }

        [HttpPut("change-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Change([FromBody] ChangePasswordCommand request)
        {
            var result = await Sender.Send(request);

            if (result.IsFailed)
            {
                return HandlerFailure(result);
            }

            return Ok(result);
        }

        [Authorize(Roles = "Super Admin")]
        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await Sender.Send(new DeleteUserCommand(id));

            if (result.IsFailed)
            {
                return HandlerFailure(result);
            }

            return Ok(result);
        }

        [HttpDelete("delete-user-in-project")]
        public async Task<IActionResult> DeleteInProject([FromQuery] DeleteUserInProjectCommand request)
        {
            var result = await Sender.Send(request);

            if (result.IsFailed)
            {
                return HandlerFailure(result);
            }

            return Ok(result);
        }
    }
}
