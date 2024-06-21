using Workspace.Application;
using Microsoft.Extensions.Configuration;
using Mailjet.Client;
using Mailjet.Client.TransactionalEmails;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using MediatR;
using Workspace.Contract;

namespace Workspace.Infrastructure
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _cache;
        private readonly IDatabase _redis;
        private readonly ISender _sender;

        public EmailService(IConfiguration configuration, IDistributedCache cache, IDatabase redis, ISender sender)
        {
            _configuration = configuration;
            _cache = cache;
            _redis = redis;
            _sender = sender;
        }

        public async Task SendEmailAsync(string toEmail)
        {
            var client = new MailjetClient(
                _configuration["Mailjet:ApiKey"],
                _configuration["Mailjet:ApiSecret"]
            );

            var random = new Random();

            int otp = random.Next(100000, 999999);

            var request = new TransactionalEmailBuilder()
                .WithFrom(new SendContact("tienthanh888555@gmail.com", "Thành Tiến"))
                .WithSubject("OTP Verification")
                .WithHtmlPart($@"
                    <html>
                    <head>
                        <style>
                            body {{
                                font-family: Arial, sans-serif;
                                line-height: 1.6;
                            }}
                            .container {{
                                max-width: 600px;
                                margin: 0 auto;
                                padding: 20px;
                                border: 1px solid #e0e0e0;
                                border-radius: 5px;
                            }}
                            .header {{
                                background-color: #4CAF50;
                                color: white;
                                padding: 10px 20px;
                                text-align: center;
                            }}
                            .content {{
                                padding: 20px;
                            }}
                            .footer {{
                                margin-top: 20px;
                                font-size: 12px;
                                color: #888;
                                text-align: center;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>
                                <h1>OTP Verification</h1>
                            </div>
                            <div class='content'>
                                <p>Dear User,</p>
                                <p>Your OTP code is: <strong>{otp}</strong></p>
                                <p>Please use this code to complete your verification process. The code is valid for the next 1 minutes.</p>
                            </div>
                            <div class='footer'>
                                <p>&copy; 2024 Thành Tiến. All rights reserved.</p>
                            </div>
                        </div>
                    </body>
                    </html>")
                .WithTo(new SendContact(toEmail))
                .Build();

            await client.SendTransactionalEmailAsync(request);

            await _cache.SetStringAsync(toEmail, otp.ToString(), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            });
        }

        public async Task<bool> VerifyEmailAsync(string email, string otp)
        {
            var storeOtp = await _redis.HashGetAsync(email, "data");

            if(string.IsNullOrEmpty(storeOtp))
            {
                return false;
            }

            if(otp != storeOtp)
            {
                return false;
            }

            await _redis.KeyDeleteAsync(email);

            return true;
        }

        public async Task SendEmailV2Async(string toEmail, Guid id, string action)
        {
            var client = new MailjetClient(
                _configuration["Mailjet:ApiKey"],
                _configuration["Mailjet:ApiSecret"]
            );

            var task = await _sender.Send(new GetTaskByIdQuery(id));

            var project = task.Value.ProjectId != null ? await _sender.Send(new GetProjectByIdQuery((Guid)task.Value.ProjectId)) : null;

            var request = new TransactionalEmailBuilder()
                .WithFrom(new SendContact("tienthanh888555@gmail.com", "Thành Tiến"))
                .WithSubject("[WORKSPACE] TASK")
                .WithHtmlPart($@"
                    <html>
                    <head>
                        <style>
                            body {{
                                font-family: Arial, sans-serif;
                                line-height: 1.6;
                                color: #333;
                            }}
                            .container {{
                                max-width: 600px;
                                margin: 0 auto;
                                padding: 20px;
                                border: 1px solid #e0e0e0;
                                border-radius: 5px;
                            }}
                            .header {{
                                font-size: 14px;
                                color: #555;
                            }}
                            .content {{
                                padding: 20px 0;
                            }}
                            .issue-details {{
                                margin: 20px 0;
                                padding: 20px;
                                background-color: #f4f5f7;
                                border-left: 5px solid #0052cc;
                            }}
                            .issue-details p {{
                                margin: 5px 0;
                            }}
                            .issue-details .description,
                            .issue-details .name,
                            .issue-details .issue-type,
                            .issue-details .assignee,
                            .issue-details .priority,
                            .issue-details .created,
                            .issue-details .reporter {{
                                font-size: 14px;
                            }}
                            .button {{
                                display: inline-block;
                                padding: 10px 20px;
                                margin: 20px 0;
                                font-size: 14px;
                                color: white;
                                background-color: #0052cc;
                                text-decoration: none;
                                border-radius: 5px;
                            }}
                            .footer {{
                                font-size: 12px;
                                color: #888;
                                text-align: center;
                                margin-top: 20px;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>
                                <p>{task.Value.ReporterName} {action} a task</p>
                            </div>
                            <div class='content'>
                                <div class='issue-details'>
                                    <p><strong>{task.Value.ReporterName}</strong> {task.Value.CreatedDate}</p>
                                    <p class='name'>Name: {task.Value.Name}</p>
                                    <p class='issue-type'>Task Type: {task.Value.TypeName}</p>
                                    <p class='assignee'>Assignee: {task.Value.AssigneeName}</p>
                                    <p class='priority'>Priority: {task.Value.PriorityName}</p>
                                    <p class='created'>Created: {task.Value.CreatedDate}</p>
                                    <p class='reporter'>Reporter: {task.Value.ReporterName}</p>
                                    <p class='description'>Description: {task.Value.Description}</p>
                                </div>
                                <a href=""http://localhost:4200/project/{project?.Value.Slug}/task/{task.Value.Id}"" class='button' style=""color: white; text-decoration: none; background-color: #0052cc; padding: 10px 20px; border-radius: 5px;"">View task</a>
                            </div>
                            <div class='footer'>
                                <p>&copy; 2024 Thành Tiến. All rights reserved.</p>
                            </div>
                        </div>
                    </body>
                    </html>")
                .WithTo(new SendContact(toEmail))
                .Build();

            await client.SendTransactionalEmailAsync(request);
        }

        public async Task SendEmailResetPasswordAsync(string email, string password)
        {
            var client = new MailjetClient(
                _configuration["Mailjet:ApiKey"],
                _configuration["Mailjet:ApiSecret"]
            );

            var request = new TransactionalEmailBuilder()
                .WithFrom(new SendContact("tienthanh888555@gmail.com", "Thành Tiến"))
                .WithSubject("Reset password")
                .WithHtmlPart($@"
                    <html>
                    <head>
                        <style>
                            body {{
                                font-family: Arial, sans-serif;
                                line-height: 1.6;
                            }}
                            .container {{
                                max-width: 600px;
                                margin: 0 auto;
                                padding: 20px;
                                border: 1px solid #e0e0e0;
                                border-radius: 5px;
                            }}
                            .header {{
                                background-color: #4CAF50;
                                color: white;
                                padding: 10px 20px;
                                text-align: center;
                            }}
                            .content {{
                                padding: 20px;
                            }}
                            .footer {{
                                margin-top: 20px;
                                font-size: 12px;
                                color: #888;
                                text-align: center;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>
                                <h1>Reset Password</h1>
                            </div>
                            <div class='content'>
                                <p>Dear User,</p>
                                <p>Your new password is: <strong>{password}</strong></p>
                            </div>
                            <div class='footer'>
                                <p>&copy; 2024 Thành Tiến. All rights reserved.</p>
                            </div>
                        </div>
                    </body>
                    </html>")
                .WithTo(new SendContact(email))
                .Build();

            await client.SendTransactionalEmailAsync(request);
        }

        public async Task SendEmailReminderAsync()
        {
            var client = new MailjetClient(
                _configuration["Mailjet:ApiKey"],
                _configuration["Mailjet:ApiSecret"]
            );

            var tasks = await _sender.Send(new GetTaskDueSoonQuery());

            foreach (var task in tasks.Value)
            {
                var project = task.ProjectId != null ? await _sender.Send(new GetProjectByIdQuery((Guid)task.ProjectId)) : null;

                var user = await _sender.Send(new GetUserByIdQuery { Id = task.UserId });

                var request = new TransactionalEmailBuilder()
                .WithFrom(new SendContact("tienthanh888555@gmail.com", "Thành Tiến"))
                .WithSubject("Reminder: Your task is due soon!")
                .WithHtmlPart($@"
                    <html>
                    <head>
                        <style>
                            body {{
                                font-family: Arial, sans-serif;
                                line-height: 1.6;
                            }}
                            .container {{
                                max-width: 600px;
                                margin: 0 auto;
                                padding: 20px;
                                border: 1px solid #e0e0e0;
                                border-radius: 5px;
                            }}
                            .header {{
                                background-color: #4CAF50;
                                color: white;
                                padding: 10px 20px;
                                text-align: center;
                            }}
                            .content {{
                                padding: 20px;
                            }}
                            .footer {{
                                margin-top: 20px;
                                font-size: 12px;
                                color: #888;
                                text-align: center;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>
                                <h1>Deadline</h1>
                            </div>
                            <div class='content'>
                                <p>Dear User,</p>
                                <p>Your task <strong>{task.Name}</strong> in project <strong>{project.Value.Name}</strong> is due on {task.EndDate.Date.AddDays(1)}. Please complete it on time.</p>
                            </div>
                            <div class='footer'>
                                <p>&copy; 2024 Thành Tiến. All rights reserved.</p>
                            </div>
                        </div>
                    </body>
                    </html>")
                .WithTo(new SendContact(user.Value.Email))
                .Build();

                await client.SendTransactionalEmailAsync(request);
            }

        }
    }
}
