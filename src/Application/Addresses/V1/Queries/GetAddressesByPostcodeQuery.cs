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
    public class GetAddressesByPostcodeQuery : IRequest<IEnumerable<PostcodeAddressModel>>
    {
        public string Postcode { get; }

        public GetAddressesByPostcodeQuery(string postcode)
        {
            Postcode = postcode;
        }

        public class GetAddressesByPostcodeQueryHandler : IRequestHandler<GetAddressesByPostcodeQuery, IEnumerable<PostcodeAddressModel>>
        {
            private readonly ILogger<GetAddressesByPostcodeQueryHandler> _logger;
            private readonly IEnumerable<IPostcodeLookup> _postcodeLookups;

            public GetAddressesByPostcodeQueryHandler(IEnumerable<IPostcodeLookup> postcodeLookups, IHeaderService headerService, ILogger<GetAddressesByPostcodeQueryHandler> logger)
            {
                _logger = logger;
                _postcodeLookups = postcodeLookups;
            }

            public async Task<IEnumerable<PostcodeAddressModel>> Handle(GetAddressesByPostcodeQuery request, CancellationToken cancellationToken)
            {
                Exception lastException = null;
                
                foreach (var client in _postcodeLookups)
                {
                    try
                    {
                        var response = await client.GetAddressesByPostcodeAsync(request.Postcode);

                        if (response != null)
                        {
                            _logger.LogInformation($"Returning SUCCESS response for postcode: {request.Postcode} - using external service: {client.GetType().Name}");
                            return response;
                        }
                        
                        _logger.LogError($"Error getting addresses for postcode: {request.Postcode} - using external service: {client.GetType().Name}");
                    }
                    catch (HttpServiceException ex)
                    {
                        lastException = ex;
                        _logger.LogError(ex, $"HTTP Request Error ({ex.GetType().Name}) getting addresses for postcode: {request.Postcode} - using external service: {client.GetType().Name}: ({ex.HttpStatusCode}) : {ex.Message}");
                    }
                    catch (HttpRequestException ex)
                    {
                        lastException = ex;
                        _logger.LogError(ex, $"HTTP Request Error ({ex.GetType().Name}) getting addresses for postcode: {request.Postcode} - using external service: {client.GetType().Name} : {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        lastException = ex;
                        _logger.LogError(ex, $"Error ({ex.GetType().Name}) getting addresses for postcode: {request.Postcode} - using external service: {client.GetType().Name} : {ex.Message}");
                    }
                }

                throw new Exception($"Unable to get address for postcode: {request.Postcode} from any service", lastException);
            }
        }
    }
}