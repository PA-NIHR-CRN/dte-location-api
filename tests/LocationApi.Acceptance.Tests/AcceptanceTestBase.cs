using System;
using System.Net;
using Dte.Api.Acceptance.Test.Helpers.Clients;
using Dte.Api.Acceptance.Test.Helpers.Extensions;
using Dte.Common.Authentication;
using Dte.Common.Http;
using LocationApi.Acceptance.Tests.Clients;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace LocationApi.Acceptance.Tests
{
    public abstract class AcceptanceTestBase
    {
        private ApiClient _apiClient;
        
        // Override this method to provide a custom claims
        protected virtual void CreateApiWebApplicationFactory()
        {
            TestApi = new ApiWebApplicationFactory();
        }
        
        [SetUp]
        public void Setup()
        {
            CreateApiWebApplicationFactory();
            Scope = TestApi.Services.CreateScope();

            var httpClient = TestApi.CreateClient();
            _apiClient = ApiClientFactory.For(httpClient, "test");
            BaseAddress = httpClient.BaseAddress;
            
            LocationApiClient = new LocationApiClient(_apiClient);
            LocationApiClient.SetBasicAuthorisation(AuthenticationSettings.BasicAuthClients[0].ClientName, AuthenticationSettings.BasicAuthClients[0].ClientPassword);
            
            LocationApiClient = new LocationApiClient(_apiClient);
        }
        
        protected ApiWebApplicationFactory TestApi;
        protected IServiceScope Scope { get; private set; }
        protected Uri BaseAddress { get; private set; }
        protected LocationApiClient LocationApiClient { get; private set; }
        protected AuthenticationSettings AuthenticationSettings { get; private set; }

        protected static void AssertResponseStatusCode(IStubApiClient client, HttpStatusCode statusCode)
        {
            client.LastResponse().ShouldHaveStatusCode(statusCode);
        }

        protected static void AssertResponseContentType(IStubApiClient client, string contentType)
        {
            client.LastResponse().ShouldHaveContentType(contentType);
        }

        [TearDown]
        public void Dispose()
        {
            Scope.Dispose();
            TestApi?.Dispose();
            _apiClient?.Dispose();
        }
    }
}