namespace Workspace.Contract
{
    public class EntityExtensions
    {
        public static string GetSortProperty(string sortColumn)
        => sortColumn.ToLower() switch
        {
            "name" => "Name",
            "description" => "Description",
            _ => "CreatedDate"
        };
    }
}
