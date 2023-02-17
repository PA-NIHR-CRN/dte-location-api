using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace LocationApi.DependencyRegistrations
{
    public static class ApplicationRegistration
    {
        private const string ApplicationAssemblyName = "Application";

        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.Load(ApplicationAssemblyName));
            
            return services;
        }
    }
}