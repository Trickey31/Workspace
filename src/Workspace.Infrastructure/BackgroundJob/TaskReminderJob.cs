using Quartz;
using Workspace.Application;

namespace Workspace.Infrastructure
{
    public class TaskReminderJob : IJob
    {
        private readonly IEmailService _emailService;

        public TaskReminderJob(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _emailService.SendEmailReminderAsync();
        }
    }
}
