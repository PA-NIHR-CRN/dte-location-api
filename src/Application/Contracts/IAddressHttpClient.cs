using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Models;

namespace Application.Contracts
{
    public interface IAddressHttpClient
    {
        Task<IEnumerable<PostcodeAddressModel>> GetAddressesByPostcodeAsync(string postcode);
        Task<LatLngModel> GetLatLngByPostcodeAsync(string postcode);
    }
}
