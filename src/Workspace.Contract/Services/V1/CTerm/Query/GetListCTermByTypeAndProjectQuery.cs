using System.ComponentModel.DataAnnotations;

namespace Workspace.Contract
{
    public class GetListCTermByTypeAndProjectQuery : IQuery<List<CTermResponse>>
    {
        [Required]
        public string Type { get; set; }

        [Required]
        public Guid ProjectId { get; set; }
    }
}
