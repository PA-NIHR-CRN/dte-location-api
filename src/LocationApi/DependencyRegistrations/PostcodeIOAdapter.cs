using Application.Contracts;
using Application.Models;
using MarkEmbling.PostcodesIO;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LocationApi.DependencyRegistrations
{
    public class PostcodeIOAdapter : IPostcodeLookup
    {
        private readonly IPostcodesIOClient postcodesIOClient;
        private readonly ILogger<PostcodeIOAdapter> logger;

        public PostcodeIOAdapter(IPostcodesIOClient postcodesIOClient, ILogger<PostcodeIOAdapter> logger)
        {
            this.postcodesIOClient = postcodesIOClient;
            this.logger = logger;
        }

        public Task<IEnumerable<PostcodeAddressModel>> GetAddressesByPostcodeAsync(string postcode)
        {
            logger.LogInformation("Postcode to address lookup is not supported by the postcodes.io API.");
            return Task.FromResult((IEnumerable<PostcodeAddressModel>)null);
        }

        public async Task<CoordinatesModel> GetCoordinatesByPostcodeAsync(string postcode, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var result = await postcodesIOClient.LookupAsync(postcode);

            if (result?.Latitude is not null && result?.Longitude is not null)
            {
                return new CoordinatesModel { Latitude = result.Latitude.Value, Longitude = result.Longitude.Value };
            }
            else
            {
                return null;
            }
        }
    }
}