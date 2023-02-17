using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Dte.Common.Authentication;
using FluentValidation.AspNetCore;
using LocationApi.Behaviours;
using LocationApi.Common;
using LocationApi.DependencyRegistrations;
using LocationApi.Extensions;
using LocationApi.HealthChecks;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;

namespace LocationApi
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Configuration
            var authenticationSettings = Configuration.GetSection(AuthenticationSettings.SectionName).Get<AuthenticationSettings>();
            if (authenticationSettings == null) throw new Exception("Could not bind the authentication settings, please check configuration");
            var clientsSettings = Configuration.GetSection(ClientsSettings.SectionName).Get<ClientsSettings>();
            if (clientsSettings == null) throw new Exception("Could not bind the Clients Settings, please check configuration");

            services.AddSingleton(authenticationSettings);
            services.AddSingleton(clientsSettings);

            services.AddApiVersioning(opts =>
            {
                opts.AssumeDefaultVersionWhenUnspecified = true;
                opts.DefaultApiVersion = ApiVersion.Parse("1");
                opts.ApiVersionReader = ApiVersionReader.Combine
                (
                    new MediaTypeApiVersionReader("version"),
                    new HeaderApiVersionReader("x-dte-version")
                );
                opts.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(
                options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });

            services.AddSwaggerGen(c =>
            {
                // Include 'SecurityScheme' to use JWT Authentication
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Scheme = "basic",
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement { { jwtSecurityScheme, Array.Empty<string>() } });

                var versionProvider = services.BuildServiceProvider().GetService<IApiVersionDescriptionProvider>();
                foreach (var description in versionProvider.ApiVersionDescriptions)
                {
                    c.SwaggerDoc(description.GroupName, new OpenApiInfo { Title = "Dte.Location.Api", Version = description.GroupName });
                }
                
                var filePath = Path.Combine(AppContext.BaseDirectory, "LocationApi.xml");
                c.IncludeXmlComments(filePath);
            });

            // Register service to service basic authentication
            services.AddAuthentication("Basic").AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", null);
            services.AddAuthorization(options => options.AddPolicy("Basic", builder => builder.RequireAuthenticatedUser().AddAuthenticationSchemes("Basic")));

            // Applications / Features
            services.AddApplication();
            services.AddInfrastructure(Configuration, Environment.EnvironmentName);

            // All others
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

            // ASP.NET Core setup
            services.AddControllers(x =>
                {
                    x.Filters.Add(typeof(RequestModelValidatorFilter));
                    x.Filters.Add(new AuthorizeFilter("Basic"));
                })
                .AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<Startup>(lifetime: ServiceLifetime.Transient))
                .AddFluentValidation(x => x.RegisterValidatorsFromAssembly(Assembly.Load("Application"), lifetime: ServiceLifetime.Transient));
            
            var build = System.Environment.GetEnvironmentVariable("DTE_BUILD_STRING") ?? "Unknown";
            services.AddHealthChecks()
                .AddCheck("LocationService", () => HealthCheckResult.Healthy($"Build: {build}"))
                .AddCheck<PostcodeLookupHealthCheck>("PostcodeLookupService", timeout: clientsSettings.OrdnanceSurveyService.DefaultTimeout, tags: new List<string> { "services" });
        }

        public void Configure(IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
            app.UseCustomExceptionHandler();
            app.UseCustomHeaderForwarderHandler();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    c.SwaggerEndpoint($"./{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app
                .UseHsts()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapCustomHealthCheck();
                    endpoints.MapControllers();
                    endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Location API V0.0.2"); });
                });
        }
    }
}