using System.ComponentModel.DataAnnotations;

namespace Workspace.Contract
{
    public class GetByIdQuery : IQuery<CTermResponse>
    {
        [Required]
        public Guid Id { get; set; }
    }
}
