using Workspace.Contract;

namespace Workspace.Contract
{
    public static class Commands
    {
        public record CreateTask(TaskCommand Task) : ICommand;

        public record UpdateTask(Guid Id, TaskCommand Task) : ICommand;

        public record DeleteTask(Guid Id) : ICommand;
    }
}
