﻿using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using Workspace.API.DependencyInjection.Options;

namespace Workspace.API
{
    public static class SwaggerExtension
    {
        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen();
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOption>();
        }

        public static void ConfigureSwagger(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var version in app.DescribeApiVersions().Select(version => version.GroupName))
                    options.SwaggerEndpoint($"/swagger/{version}/swagger.json", version);

                options.DisplayRequestDuration();
                options.EnableTryItOutByDefault();
                options.DocExpansion(DocExpansion.None);
            });

            app.MapGet("/", () => Results.Redirect("/swagger/index.html"))
                .WithTags(string.Empty);
        }
    }
}
