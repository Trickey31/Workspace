namespace Workspace.Contract
{
    public class GetStatisticByYearQuery : IQuery<List<StatisticResponseByYear>>
    {
        public int? Year { get; set; }
    }
}
