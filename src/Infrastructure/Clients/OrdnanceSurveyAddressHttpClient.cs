using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Mappings;
using Application.Models;
using Application.Responses.OrdnanceSurvey;
using Dte.Common.Authentication;
using Dte.Common.Http;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Clients
{
    public class OrdnanceSurveyAddressHttpClient : BaseHttpClient, IOrdnanceSurveyHttpClient
    {
        private readonly ILogger<OrdnanceSurveyAddressHttpClient> _logger;
        
        public OrdnanceSurveyAddressHttpClient(HttpClient httpClient, IHeaderService headerService, ILogger<OrdnanceSurveyAddressHttpClient> logger) 
            : base(httpClient, headerService, logger, ApiClientConfiguration.Default)
        {
            _logger = logger;
        }
        
        protected override string ServiceName => "OrdnanceSurveyService";

        public async Task<IEnumerable<PostcodeAddressModel>> GetAddressesByPostcodeAsync(string postcode)
        {
            var httpRequest = new HttpRequestMessage
            {
                RequestUri = new Uri($"search/places/v1/postcode?{BuildCountryCodeQueryParams(OrdnanceSurveyCountries.Countries)}&lr=EN&postcode={postcode}", UriKind.Relative),
                Method = HttpMethod.Get
            };
            
            var response = await SendAsync<OrdnanceSurveyAddressResponse>(httpRequest);

            if (response == null)
            {
                throw new Exception($"{nameof(OrdnanceSurveyAddressResponse)} is null");
            }

            return response.Results?.Where(x => x.Dpa != null).Select(x => OrdnanceSurveyResponseMapper.MapTo(x.Dpa)) ?? new List<PostcodeAddressModel>();
        }

        public async Task<CoordinatesModel> GetCoordinatesByPostcodeAsync(string postcode, CancellationToken cancellationToken)
        {
            var httpRequest = new HttpRequestMessage
            {
                RequestUri = new Uri($"search/places/v1/postcode?{BuildCountryCodeQueryParams(OrdnanceSurveyCountries.Countries)}&lr=EN&postcode={postcode}", UriKind.Relative),
                Method = HttpMethod.Get
            };
            
            var response = await SendAsync<OrdnanceSurveyAddressResponse>(httpRequest);

            if (response == null)
            {
                _logger.LogError("Failed to retrieve coordinates for postcode {Postcode}", postcode);
                throw new Exception($"{nameof(OrdnanceSurveyAddressResponse)} is null");
            }

            var result = response.Results?.FirstOrDefault(x => x.Dpa != null);

            if (result == null)
            {
                return null;
            }

            return new CoordinatesModel
            {
                Latitude = result.Dpa.Lat,
                Longitude = result.Dpa.Lng,
                SRID = 4326
            };
        }

        private static string BuildCountryCodeQueryParams(IEnumerable<CountryModel> countryModels)
        {
            var sb = new StringBuilder("fq=");

            foreach (var country in countryModels)
            {
                sb.Append($"COUNTRY_CODE:{country.CountryCode} ");
            }

            return sb.ToString();
        }
    }

    public class OrdnanceSurveyHttpMessageHandler : DelegatingHandler
    {
        private readonly IHeaderService _headerService;
        private readonly ClientsSettings _clientsSettings;
        private readonly ILogger<OrdnanceSurveyHttpMessageHandler> _logger;

        public OrdnanceSurveyHttpMessageHandler(IHeaderService headerService, ClientsSettings clientsSettings, ILogger<OrdnanceSurveyHttpMessageHandler> logger)
        {
            _headerService = headerService;
            _clientsSettings = clientsSettings;
            _logger = logger;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Calling http: {request.RequestUri}");

            _headerService.AddAllHeadersToRequest(request);

            request.Headers.Add("key", _clientsSettings.OrdnanceSurveyService.ApiKey);

            return base.SendAsync(request, cancellationToken);
        }
    }
}
