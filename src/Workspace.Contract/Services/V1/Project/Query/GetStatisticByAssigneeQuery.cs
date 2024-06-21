namespace Workspace.Contract
{
    public class GetStatisticByAssigneeQuery : IQuery<List<StatisticResponse>>
    {
        public Guid ProjectId { get; set; }
    }
}
