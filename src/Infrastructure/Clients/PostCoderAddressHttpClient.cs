using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Models;
using Dte.Common.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Infrastructure.Clients
{
    public class PostCoderAddressHttpClient : BaseHttpClient, IPostCoderHttpClient
    {
        private readonly ILogger<PostCoderAddressHttpClient> _logger;

        public PostCoderAddressHttpClient(HttpClient httpClient, IHeaderService headerService, ILogger<PostCoderAddressHttpClient> logger) 
            : base(httpClient, headerService, logger, ApiClientConfiguration.Default)
        {
            _logger = logger;
        }
        
        protected override string ServiceName => "PostCoderService";

        public async Task<IEnumerable<PostcodeAddressModel>> GetAddressesByPostcodeAsync(string postcode)
        {
            var httpRequest = new HttpRequestMessage
            {
                RequestUri = new Uri($"search/SomeUrl/v1/postcode?postcode={postcode}", UriKind.Relative),
                Method = HttpMethod.Get
            };
            
            var response = await SendAsync<IEnumerable<PostcodeAddressModel>>(httpRequest);

            return response;
        }

        public async Task<CoordinatesModel> GetCoordinatesByPostcodeAsync(string postcode, CancellationToken cancellationToken)
        {
            var httpRequest = new HttpRequestMessage
            {
                RequestUri = new Uri($"search/SomeUrl/v1/postcode?postcode={postcode}", UriKind.Relative),
                Method = HttpMethod.Get
            };
            
            var response = await SendAsync<CoordinatesModel>(httpRequest);

            return response;
        }
    }
    
    public class PostCoderHttpMessageHandler : DelegatingHandler
    {
        private readonly IHeaderService _headerService;
        private readonly ILogger<PostCoderHttpMessageHandler> _logger;

        public PostCoderHttpMessageHandler(IHeaderService headerService, ILogger<PostCoderHttpMessageHandler> logger)
        {
            _headerService = headerService;
            _logger = logger;
        }
        
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Calling http: {request.RequestUri}");
            
            _headerService.AddAllHeadersToRequest(request);
            
            // request.Headers.Add("UserName", _awsSettings.UserName);
            // request.Headers.Add("Password", _awsSettings.Password);
            
            var addresses = new List<PostcodeAddressModel>();
            
            // return base.SendAsync(request, cancellationToken);
            return Task.FromResult(new HttpResponseMessage {StatusCode = HttpStatusCode.OK, Content = new StringContent(JsonConvert.SerializeObject(addresses))});
        }
    }
}
