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
    public class TaskController : ApiController
    {
        public TaskController(ISender sender) : base(sender)
        {
        }

        [HttpGet]
        [ProducesResponseType(typeof(TResult<List<TaskResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTasks([FromQuery] GetTasksQuery request)
        {
            var result = await Sender.Send(request);

            return Ok(result);
        }

        [HttpGet("export-word")]
        [ProducesResponseType(typeof(TResult<List<TaskResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ExportWord([FromQuery] ExportWordQuery request)
        {
            var result = await Sender.Send(request);

            return File(result.Value, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "ExportedData.docx");
        }

        [HttpGet("export-excel")]
        [ProducesResponseType(typeof(TResult<List<TaskResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ExportExcel([FromQuery] ExportExcelQuery request)
        {
            var result = await Sender.Send(request);

            return File(result.Value, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExportedData.xlsx");
        }

        [HttpGet("get-by-project")]
        [ProducesResponseType(typeof(TResult<List<TaskResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTasks([FromQuery] GetTaskByProjectQuery request)
        {
            var result = await Sender.Send(request);

            return Ok(result);
        }

        [HttpGet("get-by-jql")]
        [ProducesResponseType(typeof(TResult<List<TaskResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTasks([FromQuery] GetTaskByJQLQuery request)
        {
            var result = await Sender.Send(request);

            return Ok(result);
        }

        [HttpPost("get-advanced")]
        [ProducesResponseType(typeof(TResult<List<TaskResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTasksAdvanced([FromBody] GetTaskAdvancedQuery request)
        {
            var result = await Sender.Send(request);

            return Ok(result);
        }

        [HttpGet("userId")]
        [ProducesResponseType(typeof(TResult<List<TaskResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTasksByUserId([FromQuery] GetTaskByUserIdQuery request)
        {
            var result = await Sender.Send(request);

            return Ok(result);
        }

        [HttpGet("personal-task")]
        [ProducesResponseType(typeof(TResult<List<TaskResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPersonalTasks()
        {
            var result = await Sender.Send(new GetPersonalTaskQuery());

            return Ok(result);
        }

        [HttpGet("{id:Guid}")]
        [ProducesResponseType(typeof(TResult<TaskResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTask(Guid id)
        {
            var result = await Sender.Send(new GetTaskByIdQuery(id));

            return Ok(result);
        }

        [Authorize(Roles = "Super Admin")]
        [HttpGet("statistic-by-year")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByYear([FromQuery] GetTaskStatisticByYearQuery request)
        {
            var result = await Sender.Send(request);

            return Ok(result);
        }

        [HttpGet("suggestion")]
        [ProducesResponseType(typeof(TResult<List<TaskResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSuggestions([FromQuery] GetSuggestionQuery request)
        {
            var result = await Sender.Send(request);

            return Ok(result);
        }

        [HttpGet("get-task-due-soon")]
        [ProducesResponseType(typeof(TResult<List<TaskResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTaskDueSoon()
        {
            var result = await Sender.Send(new GetTaskDueSoonQuery());

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskCommand createTask)
        {
            var result = await Sender.Send(createTask);

            if(result.IsFailed)
            {
                return HandlerFailure(result);
            }

            return Ok(result);
        }

        [HttpPost("create-personal-task")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateTask([FromBody] CreatePersonalTaskCommand createTask)
        {
            var result = await Sender.Send(createTask);

            if (result.IsFailed)
            {
                return HandlerFailure(result);
            }

            return Ok(result);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var result = await Sender.Send(new DeleteTaskCommand(id));

            if (result.IsFailed)
            {
                return HandlerFailure(result);
            }

            return Ok(result);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTask([FromBody] UpdateTaskCommand updateTask)
        {
            var result = await Sender.Send(updateTask);

            if (result.IsFailed)
            {
                return HandlerFailure(result);
            }

            return Ok(result);
        }

        [HttpPut("update-personal-task")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePersonalTask([FromBody] UpdatePersonalTaskCommand updateTask)
        {
            var result = await Sender.Send(updateTask);

            if (result.IsFailed)
            {
                return HandlerFailure(result);
            }

            return Ok(result);
        }

        [HttpPut("update-status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusTaskCommand updateTask)
        {
            var result = await Sender.Send(updateTask);

            if (result.IsFailed)
            {
                return HandlerFailure(result);
            }

            return Ok(result);
        }
    }
}
