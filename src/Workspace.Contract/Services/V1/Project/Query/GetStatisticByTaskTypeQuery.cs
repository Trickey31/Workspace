namespace Workspace.Contract
{
    public class GetStatisticByTaskTypeQuery : IQuery<List<StatisticResponse>>
    {
        public Guid ProjectId { get; set; }
    }
}
