using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Addresses.V1.Queries;
using Application.Models;
using LocationApi.Helpers;
using LocationApi.Requests;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LocationApi.Controllers.V1
{
    [ApiController]
    [ApiVersion("1")]
    [Route("api/[controller]")]
    public class AddressController : Controller
    {
        private readonly IMediator _mediator;

        public AddressController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        /// <summary>
        /// Get the address for a given postcode
        /// </summary>
        /// <response code="200">Addresses retrieved</response>
        /// <response code="500">Server side error</response>
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<PostcodeAddressModel>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = null)]
        [HttpGet("postcode/{postcode}")]
        public async Task<IActionResult> GetByPostcode([FromRoute] GetByPostcodeRequest request)
        {
            var postcode = PostcodeValidator.Sanitize(request.Postcode);

            return Ok(await _mediator.Send(new GetAddressesByPostcodeQuery(postcode)));
        }
        
        
        /// <summary>
        /// Get the latitude and longitude for a given postcode
        /// </summary>
        /// <response code="200">Lat and long retrieved</response>
        /// <response code="500">Server side error</response>
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(CoordinatesModel))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = null)]
        [HttpGet("coordinates/{postcode}")]
        public async Task<IActionResult> GetCoordinatesByPostcode([FromRoute] GetLatLngByPostcodeRequest request)
        {
            var postcode = PostcodeValidator.Sanitize(request.Postcode);

            return Ok(await _mediator.Send(new GetCoordinatesByPostcodeQuery(postcode)));
        }
    }
}
