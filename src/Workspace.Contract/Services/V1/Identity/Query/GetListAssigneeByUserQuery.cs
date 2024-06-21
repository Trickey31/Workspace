using System.ComponentModel.DataAnnotations;

namespace Workspace.Contract
{
    public class GetListAssigneeByUserQuery : IQuery<List<CurrentUserInfoResponse>>
    {
        [Required]
        public Guid ProjectId { get; set; }
    }
}
