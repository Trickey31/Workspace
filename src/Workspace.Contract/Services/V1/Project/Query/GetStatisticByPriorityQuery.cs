namespace Workspace.Contract
{
    public class GetStatisticByPriorityQuery : IQuery<List<StatisticResponse>>
    {
        public Guid ProjectId { get; set; }
    }
}
