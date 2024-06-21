using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Workspace.Domain;

namespace Workspace.Persistence
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSqlConfiguration(this IServiceCollection services)
        {
            services.AddDbContextPool<DbContext, ApplicationDbContext>((provider, builder) =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var options = provider.GetRequiredService<IOptionsMonitor<PostgreSQLRetryOptions>>();

                #region ============== SQL-SERVER-STRATEGY-1 ==============

                builder
                .EnableDetailedErrors(true)
                .EnableSensitiveDataLogging(true)
                .UseLazyLoadingProxies(true) // => If UseLazyLoadingProxies, all of the navigation fields should be VIRTUAL
                .UseNpgsql(
                    connectionString: configuration.GetConnectionString("ConnectionStrings"),
                    npgsqlOptionsAction: optionsBuilder
                            => optionsBuilder.ExecutionStrategy(
                                    dependencies => new NpgsqlRetryingExecutionStrategy(
                                        dependencies: dependencies,
                                        maxRetryCount: options.CurrentValue.MaxRetryCount,
                                        maxRetryDelay: options.CurrentValue.MaxRetryDelay,
                                        errorCodesToAdd: options.CurrentValue.ErrorCodesToAdd))
                                .MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name));

                #endregion ============== SQL-SERVER-STRATEGY-1 ==============

                #region ============== SQL-SERVER-STRATEGY-2 ==============

                //builder
                //.EnableDetailedErrors(true)
                //.EnableSensitiveDataLogging(true)
                //.UseLazyLoadingProxies(true) // => If UseLazyLoadingProxies, all of the navigation fields should be VIRTUAL
                //.UseSqlServer(
                //    connectionString: configuration.GetConnectionString("ConnectionStrings"),
                //        sqlServerOptionsAction: optionsBuilder
                //            => optionsBuilder
                //            .MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name));

                #endregion ============== SQL-SERVER-STRATEGY-2 ==============
            });

            services.AddIdentityCore<User>(opt =>
            {
                opt.Lockout.AllowedForNewUsers = true; // Default true
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2); // Default 5
                opt.Lockout.MaxFailedAccessAttempts = 3; // Default 5
            })
                .AddRoles<Role>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.AllowedForNewUsers = true; // Default true
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2); // Default 5
                options.Lockout.MaxFailedAccessAttempts = 3; // Default 5
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
                options.Lockout.AllowedForNewUsers = true;
            });
        }

        public static OptionsBuilder<PostgreSQLRetryOptions> ConfigurePostgreSQLRetryOptions(this IServiceCollection services, IConfigurationSection section)
        => services
            .AddOptions<PostgreSQLRetryOptions>()
            .Bind(section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        public static void AddRepositoryBaseConfiguration(this IServiceCollection services)
        {
            services.AddTransient(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddTransient(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>));
        }
    }
}
