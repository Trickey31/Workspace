namespace Workspace.Contract
{
    public class GetStatisticByStatusQuery : IQuery<List<StatisticResponse>>
    {
        public Guid ProjectId { get; set; }
    }
}
