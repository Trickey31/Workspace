using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Workspace.Application;
using StackExchange.Redis;
using Quartz;

namespace Workspace.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IJwtTokenService, JwtTokenService>();
            services.AddTransient<IEmailService, EmailService>();

            var redis = new RedisOptions();
            configuration.GetSection("RedisOptions").Bind(redis);
            services.AddSingleton(redis);

            if (!redis.Enabled)
                return;

            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redis.ConnectionString));
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redis.ConnectionString;
            });
            services.AddSingleton<IDatabase>(provider =>
            {
                var multiplexer = provider.GetRequiredService<IConnectionMultiplexer>();
                return multiplexer.GetDatabase();
            });
            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory(); // Phương thức mở rộng sẽ được tìm thấy

                var jobKey = new JobKey("TaskReminderJob");
                q.AddJob<TaskReminderJob>(opts => opts.WithIdentity(jobKey));

                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity("TaskReminderJob-trigger")
                    .WithCronSchedule("0 * * * * ?"));
            });
            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        }
    }
}
