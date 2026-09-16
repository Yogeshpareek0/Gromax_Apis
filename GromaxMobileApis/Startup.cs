using Azure.Storage.Blobs;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using GromaxMobileApis.Interfaces;
using GromaxMobileApis.Models;
using GromaxMobileApis.Models.Services;
using GromaxMobileApis.Services;
using GromaxMobileApis.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace GromaxMobileApis
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("GromaxPortalPolicy", builder =>
                {
                    builder
                       .WithOrigins(
                            "https://loadcrm.com",
                            "http://localhost:4200",
                            "https://localhost:4200"
                        )
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });
            //Firebase
            FirebaseApp.Create(new AppOptions()
            {
                //Credential = GoogleCredential.FromFile("gromax-c2226-firebase-adminsdk-fbsvc-6eda969af4.json")
                Credential = GoogleCredential.FromFile("gromax-c2226-firebase-adminsdk-fbsvc-d952eb8487.json")
            });
            services.AddHttpContextAccessor();
            //services.AddControllers();

            services
       .AddControllers()
       .ConfigureApiBehaviorOptions(options =>
       {
           options.InvalidModelStateResponseFactory = context =>
           {
               var message = context.ModelState
                   .Values
                   .SelectMany(v => v.Errors)
                   .Select(e => e.ErrorMessage)
                   .FirstOrDefault()
                   ?? "Invalid request.";

               return new BadRequestObjectResult(
                   ApiResponse<string>.BadRequest(message)
               );
           };
       });
            //        services.AddSwaggerGen(c =>
            //        {
            //            c.SwaggerDoc("v1", new OpenApiInfo
            //            {
            //                Title = "GromaxMobileApis",
            //                Version = "v1"
            //            });

            //            // Add JWT Bearer token support
            //            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            //            {
            //                Description = "Enter 'Bearer' [space] and then your valid token.\r\nExample: \"Bearer abcdef12345\"",
            //                Name = "Authorization",
            //                In = ParameterLocation.Header,
            //                Type = SecuritySchemeType.Http,
            //                Scheme = "bearer",
            //                BearerFormat = "JWT"
            //            });

            //            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            //{
            //    {
            //        new OpenApiSecurityScheme
            //        {
            //            Reference = new OpenApiReference
            //            {
            //                Type = ReferenceType.SecurityScheme,
            //                Id = "Bearer"
            //            },
            //            Scheme = "bearer",
            //            Name = "Bearer",
            //            In = ParameterLocation.Header
            //        },
            //        Array.Empty<string>()
            //    }
            //});
            //        });

            services.AddScoped<IDbConnection>(sp =>
 new SqlConnection(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<IDatabaseService, DatabaseService>();
            services.AddScoped<IDatabaseServicesweb, DatabaseServicesweb>();
            services.AddScoped<ResponseClass>();
            services.AddScoped<IAzureStorageService, AzureStorageService>();
            services.AddScoped<IReportMaster, ReportMaster>();
            services.AddScoped<DynamicExcel>();
            services.AddScoped<StagingService>();
            services.AddScoped<getFileName>();
            services.AddScoped<DapperParameterHelper>();
            services.AddScoped<ServiceInvoicePdfGenerator>();
            services.AddScoped<IUser, User>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddSwaggerDocumentation();


            var jwtSection = Configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSection["Key"]);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = jwtSection["Issuer"],
                    ValidAudience = jwtSection["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                // Token validation logic
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = async context =>
                    {
                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            // Extract token string from the Authorization header
                            var token = context.Request.Headers["Authorization"]
                                .FirstOrDefault()?.Replace("Bearer ", "");

                            if (!string.IsNullOrEmpty(token))
                            {
                                // Decode token manually (no validation)
                                var handler = new JwtSecurityTokenHandler();
                                var jwtToken = handler.ReadJwtToken(token);

                                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
                                var mobileNo = jwtToken.Claims.FirstOrDefault(c => c.Type == "mobilenumber")?.Value;
                                var id = jwtToken.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

                                // Safely resolve your database service
                                var validator = context.HttpContext.RequestServices.GetRequiredService<IDatabaseService>();

                                // Call your stored procedure
                                await validator.BlankFCMToken(mobileNo);
                            }

                            // Set custom failure reason
                            context.Fail("Token expired.");
                        }
                    },


                    OnTokenValidated = async context =>
                    {
                        var userId = context.Principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                        var jti = context.Principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                        var mobileNo = context.Principal.FindFirst("mobilenumber")?.Value;
                        var id = context.Principal.FindFirst("id")?.Value;

                        // Get your service from the DI container
                        var validator = context.HttpContext.RequestServices.GetRequiredService<IDatabaseService>();

                        // Call your method to get the latest valid JTI
                        var dbJti = await validator.ValidateToken(mobileNo, id); // Add Mobileno if required

                        if (!dbJti)
                        {
                            context.Fail("This token has been replaced due to a new login.");
                        }
                    }
                };
            });


        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("Referrer-Policy", "no-referrer");
                context.Response.Headers.Add("Permissions-Policy", "camera=(), microphone=()");
                context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");

                bool isSwaggerPath = context.Request.Path.StartsWithSegments("/swagger") ||
                                    context.Request.Path.StartsWithSegments("/growmaxmobileapi/swagger");

                if (!isSwaggerPath && !env.IsDevelopment())
                {
                    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'");
                }

                await next();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseSwagger();
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GromaxMobileApis v1"));
            }
            else
            {
                app.UseHsts();
            }

            app.UseSwaggerDocumentation(Configuration, env);
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("GromaxPortalPolicy");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
