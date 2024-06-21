using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Workspace.Contract;

namespace Workspace.Presentation
{
    [ApiVersion(1)]
    [Authorize]
    public class ProjectController : ApiController
    {
        public ProjectController(ISender sender) : base(sender)
        {
        }

        [Authorize(Roles = "Super Admin")]
        [HttpGet("projects")]
        [ProducesResponseType(typeof(TResult<IEnumerable<ProjectResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetListProjects([FromQuery] GetProjectsQuery request)
        {
            var result = await Sender.Send(request);

            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(TResult<IEnumerable<ProjectResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetProjects([FromQuery] GetListProjectByUserIdQuery request)
        {
            var result = await Sender.Send(request);

            return Ok(result);
        }

        [HttpGet("menu")]
        [ProducesResponseType(typeof(TResult<IEnumerable<ProjectResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetMenu([FromQuery] GetMenuInProjectQuery request)
        {
            var result = await Sender.Send(request);

            return Ok(result);
        }

        [HttpGet("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await Sender.Send(new GetProjectByIdQuery(id));

            return Ok(result);
        }

        [HttpGet("get-by-slug/{slug}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var result = await Sender.Send(new GetProjectBySlugQuery(slug));

            return Ok(result);
        }

        [HttpGet("statistic-by-priority")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByPriority([FromQuery] GetStatisticByPriorityQuery request)
        {
            var result = await Sender.Send(request);

            return Ok(result);
        }

        [HttpGet("statistic-by-assignee")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByAssignee([FromQuery] GetStatisticByAssigneeQuery request)
        {
            var result = await Sender.Send(request);

            return Ok(result);
        }

        [HttpGet("statistic-by-status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByStatus([FromQuery] GetStatisticByStatusQuery request)
        {
            var result = await Sender.Send(request);

            return Ok(result);
        }

        [HttpGet("statistic-by-task-type")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByTaskType([FromQuery] GetStatisticByTaskTypeQuery request)
        {
            var result = await Sender.Send(request);

            return Ok(result);
        }

        [Authorize(Roles = "Super Admin")]
        [HttpGet("statistic-by-year")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByYear([FromQuery] GetStatisticByYearQuery request)
        {
            var result = await Sender.Send(request);

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateProjectCommand request)
        {
            var result = await Sender.Send(request);

            if (result.IsFailed)
            {
                return HandlerFailure(result);
            }

            return Ok(result);
        }

        [HttpPost("add-people")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] AccessProjectCommand request)
        {
            var result = await Sender.Send(request);

            if (result.IsFailed)
            {
                return HandlerFailure(result);
            }

            return Ok(result);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await Sender.Send(new DeleteProjectCommand(id));

            if (result.IsFailed)
            {
                return HandlerFailure(result);
            }

            return Ok(result);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromBody] UpdateProjectCommand request)
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
