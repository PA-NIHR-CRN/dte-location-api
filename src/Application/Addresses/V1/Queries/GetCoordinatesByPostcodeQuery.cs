using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Models;
using Dte.Common.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Addresses.V1.Queries
{
    public class GetCoordinatesByPostcodeQuery : IRequest<CoordinatesModel>
    {
        private string Postcode { get; }

        public GetCoordinatesByPostcodeQuery(string postcode)
        {
            Postcode = postcode;
        }

        public class
            GetCoordinatesByPostcodeQueryHandler : IRequestHandler<GetCoordinatesByPostcodeQuery, CoordinatesModel>
        {
            private readonly ILogger<GetCoordinatesByPostcodeQueryHandler> _logger;
            private readonly IEnumerable<IAddressHttpClient> _postcodeServiceHttpClients;

            public GetCoordinatesByPostcodeQueryHandler(IOrdnanceSurveyHttpClient ordnanceSurveyHttpClient,
                ILogger<GetCoordinatesByPostcodeQueryHandler> logger)
            {
                _logger = logger;
                _postcodeServiceHttpClients = new List<IAddressHttpClient>
                {
                    ordnanceSurveyHttpClient,
                    // postCoderHttpClient
                };
            }

            public async Task<CoordinatesModel> Handle(GetCoordinatesByPostcodeQuery request,
                CancellationToken cancellationToken)
            {
                Exception lastException = null;

                foreach (var client in _postcodeServiceHttpClients)
                {
                    try
                    {
                        var response = await client.GetCoordinatesByPostcodeAsync(request.Postcode, cancellationToken);

                        if (response != null)
                        {
                            _logger.LogInformation("Returning SUCCESS response for postcode: {RequestPostcode} - using external service: {Name}", request.Postcode, client.GetType().Name);
                            return response;
                        }

                        _logger.LogError("Error getting coordinates for postcode: {RequestPostcode} - using external service: {Name}", request.Postcode, client.GetType().Name);
                    }
                    catch (HttpServiceException ex)
                    {
                        lastException = ex;
                        _logger.LogError(ex, "HTTP Request Error ({Name}) getting coordinates for postcode: {RequestPostcode} - using external service: {Name}: ({ExHttpStatusCode}) : {ExMessage}", ex.GetType().Name, request.Postcode, client.GetType().Name, ex.HttpStatusCode, ex.Message);
                        throw;
                    }
                    catch (HttpRequestException ex)
                    {
                        lastException = ex;
                        _logger.LogError(ex, "HTTP Request Error ({Name}) getting coordinates for postcode: {RequestPostcode} - using external service: {Name} : {ExMessage}", ex.GetType().Name, request.Postcode, client.GetType().Name, ex.Message);
                    }
                    catch (Exception ex)
                    {
                        lastException = ex;
                        _logger.LogError(ex, "Error getting coordinates for postcode: {RequestPostcode} - using external service: {Name}", request.Postcode, client.GetType().Name);
                    }
                }

                _logger.LogError(lastException, "Error getting coordinates for postcode: {RequestPostcode}", request.Postcode);
                throw new Exception($"Error getting coordinates for postcode: {request.Postcode}");
            }
        }
    }
}
