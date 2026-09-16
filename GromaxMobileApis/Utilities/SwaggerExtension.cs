using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;

namespace GromaxMobileApis.Utilities
{
    public static class SwaggerExtension
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Gromax Portal API",
                    Version = "v1"
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Example: Bearer eyJhbGci..."
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference =
                                    new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "Bearer"
                                    }
                            },
                            Array.Empty<string>()
                        }
                    });
            });
            return services;
        }

        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app, IConfiguration configuration, IWebHostEnvironment env)
        {
            bool swaggerEnabled = configuration.GetSection("Swagger").GetValue<bool>("Enabled", false);

            if (swaggerEnabled)
            {
                app.UseSwagger();

                app.UseSwaggerUI(c =>
                {
                    if (env.IsDevelopment())
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gromax Portal API V1");
                    }
                    else
                    {
                        c.SwaggerEndpoint("/growmaxmobileapi/swagger/v1/swagger.json", "Gromax Portal API V1");
                    }

                    c.RoutePrefix = "swagger";
                });
            }

            return app;
        }
    }
}

