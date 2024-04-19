using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Models;
using Dte.Common.Exceptions;
using Dte.Common.Http;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Addresses.V1.Queries
{
    public class GetLatLngByPostcodeQuery : IRequest<LatLngModel>
    {
        public string Postcode { get; }
        public GetLatLngByPostcodeQuery(string postcode)
        {
            Postcode = postcode;
        }
        
        public class GetLatLngByPostcodeQueryHandler : IRequestHandler<GetLatLngByPostcodeQuery, LatLngModel>
        {
            private readonly ILogger<GetLatLngByPostcodeQueryHandler> _logger;
            private readonly IEnumerable<IAddressHttpClient> _postcodeServiceHttpClients;

            public GetLatLngByPostcodeQueryHandler(IOrdnanceSurveyHttpClient ordnanceSurveyHttpClient, IPostCoderHttpClient postCoderHttpClient, IHeaderService headerService, ILogger<GetLatLngByPostcodeQueryHandler> logger)
            {
                _logger = logger;
                _postcodeServiceHttpClients = new List<IAddressHttpClient>
                {
                    ordnanceSurveyHttpClient,
                    // postCoderHttpClient
                };
            }

            public async Task<LatLngModel> Handle(GetLatLngByPostcodeQuery request, CancellationToken cancellationToken)
            {
                Exception lastException = null;
                
                foreach (var client in _postcodeServiceHttpClients)
                {
                    try
                    {
                        var response = await client.GetLatLngByPostcodeAsync(request.Postcode);

                        if (response != null)
                        {
                            _logger.LogInformation($"Returning SUCCESS response for postcode: {request.Postcode} - using external service: {client.GetType().Name}");
                            return response;
                        }
                        
                        _logger.LogError($"Error getting latlng for postcode: {request.Postcode} - using external service: {client.GetType().Name}");
                    }
                    catch (HttpServiceException ex)
                    {
                        lastException = ex;
                        _logger.LogError(ex, $"HTTP Request Error ({ex.GetType().Name}) getting latlng for postcode: {request.Postcode} - using external service: {client.GetType().Name}: ({ex.HttpStatusCode}) : {ex.Message}");
                        throw;
                    }
                    catch (HttpRequestException ex)
                    {
                        lastException = ex;
                        _logger.LogError(ex, $"HTTP Request Error ({ex.GetType().Name}) getting latlng for postcode: {request.Postcode} - using external service: {client.GetType().Name} : {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        lastException = ex;
                        _logger.LogError(ex, $"Error getting latlng for postcode: {request.Postcode} - using external service: {client.GetType().Name}");
                    }
                }

                _logger.LogError(lastException, $"Error getting latlng for postcode: {request.Postcode}");
                throw new Exception($"Error getting latlng for postcode: {request.Postcode}");
            }
        }
    }
}
