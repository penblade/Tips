using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Tips.Swagger.Extensions
{
    public static class SwaggerExtensions
    {
        public static void AddSwaggerWithApiKeySecurity(this IServiceCollection services, IConfiguration configuration, string assemblyName)
        {
            var apiKeyConfiguration = new ApiKeyConfiguration();
            configuration.Bind(nameof(ApiKeyConfiguration), apiKeyConfiguration);

            if (string.IsNullOrEmpty(apiKeyConfiguration?.ApiHeader)) throw new InvalidOperationException("ApiKeyConfiguration.ApiHeader is null or empty.");

            // https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-5.0&tabs=visual-studio
            services.AddSwaggerGen(c =>
            {
                const string securityDefinition = "ApiKey";

                // https://stackoverflow.com/questions/36975389/api-key-in-header-with-swashbuckle
                c.AddSecurityDefinition(securityDefinition, new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = apiKeyConfiguration.ApiHeader,
                    Type = SecuritySchemeType.ApiKey
                });
                // https://stackoverflow.com/questions/57227912/swaggerui-not-adding-apikey-to-header-with-swashbuckle-5-x
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Name = apiKeyConfiguration.ApiHeader,
                            Type = SecuritySchemeType.ApiKey,
                            In = ParameterLocation.Header,
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = securityDefinition }
                        },
                        new List<string>()
                    }
                });

                var xmlFile = $"{assemblyName}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }
    }
}
