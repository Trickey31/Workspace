namespace Workspace.Contract
{
    public class GetTaskStatisticByYearQuery : IQuery<List<TaskStatisticResponse>>
    {
        public int? Year { get; set; }
    }
}
