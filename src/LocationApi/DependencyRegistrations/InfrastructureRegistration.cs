using System;
using System.Linq;
using Application.Contracts;
using Dte.Common;
using Dte.Common.Authentication;
using Dte.Common.Contracts;
using Dte.Common.Extensions;
using Dte.Common.Http;
using Infrastructure.Clients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LocationApi.DependencyRegistrations
{
    public static class InfrastructureRegistration
    {
        private static readonly string[] ProdEnvironmentNames = { "production", "prod", "live" };

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, string environmentName)
        {
            // Infrastructure dependencies
            services.AddSingleton<IClock, Clock>();
            services.AddTransient<PostCoderHttpMessageHandler>();
            services.AddTransient<OrdnanceSurveyHttpMessageHandler>();
            services.AddSingleton<IHeaderService, HeaderService>();

            var clientsSettings = configuration.GetSection(ClientsSettings.SectionName).Get<ClientsSettings>();
            var logger = services.BuildServiceProvider().GetService<ILoggerFactory>().CreateLogger("LocationApi.DependencyRegistrations.InfrastructureRegistration");
            services
                .AddHttpClientWithRetry<IPostCoderHttpClient, PostCoderAddressHttpClient, PostCoderHttpMessageHandler>(clientsSettings.PostCoderService, 2, logger)
                .AddHttpClientWithRetry<IOrdnanceSurveyHttpClient, OrdnanceSurveyAddressHttpClient, OrdnanceSurveyHttpMessageHandler>(clientsSettings.OrdnanceSurveyService, 2, logger);

            // If not Prod, then enable stubs
            if (!ProdEnvironmentNames.Any(x => string.Equals(x, environmentName, StringComparison.OrdinalIgnoreCase)))
            {
                // Enable local stubs
            }

            return services;
        }
    }
}