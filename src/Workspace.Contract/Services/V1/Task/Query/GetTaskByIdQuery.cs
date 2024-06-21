using System.ComponentModel.DataAnnotations;

namespace Workspace.Contract
{
    public class GetTaskByIdQuery : IQuery<TaskResponse>
    {
        [Required]
        public Guid Id { get; set; }

        public GetTaskByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
