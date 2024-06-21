using System.ComponentModel.DataAnnotations;

namespace Workspace.Contract
{
    public class GetListCTermByTypeQuery : IQuery<List<CTermResponse>>
    {
        [Required]
        public string Type { get; set; }
    }
}
