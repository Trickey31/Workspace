using System.ComponentModel.DataAnnotations;

namespace Workspace.Persistence
{
    public record class PostgreSQLRetryOptions
    {
        [Required, Range(5, 20)]
        public int MaxRetryCount { get; set; }

        [Required, Timestamp]
        public TimeSpan MaxRetryDelay { get; set; }

        public string[]? ErrorCodesToAdd { get; set; }
    }
}
