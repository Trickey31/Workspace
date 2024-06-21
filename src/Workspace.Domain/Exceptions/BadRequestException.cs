using Workspace.Domain;

namespace Workspace.Domain
{
    public abstract class BadRequestException : DomainException
    {
        protected BadRequestException(string message)
            : base("Bad Request", message)
        {
        }
    }
}
