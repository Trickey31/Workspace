namespace Workspace.Contract
{
    public class GetUserStatisticByYearQuery : IQuery<List<UserStatisticByYearResponse>>
    {
        public int? Year { get; set; }
    }
}
