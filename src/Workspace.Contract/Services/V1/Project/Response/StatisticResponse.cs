namespace Workspace.Contract
{
    public class StatisticResponse
    {
        public string Name { get; set; }

        public int Task { get; set; }

        public int Percentage { get; set; }
    }

    public class StatisticResponseByYear
    {
        public string Label { get; set; }

        public int Quantity { get; set; }
    }
}
