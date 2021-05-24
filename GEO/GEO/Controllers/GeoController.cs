using GEO.Models;
using GEO.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace GEO.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GeoController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IAddressService _addressService;

        public GeoController(IMessageService messageService, IAddressService addressService)
        {
            _messageService = messageService;
            _addressService = addressService;
        }

        [HttpGet("Geocodificar/{id}")]
        public async Task<IActionResult> GetGeolocalizar([FromRoute] int id)
        {
            var direccion = await _addressService.GetAddress(id);
            return Ok(new { direccion.Id, direccion.Latitud, direccion.Longitud, direccion.Estado});
        }

        [HttpPost("Geolocalizar")]
        public async Task<IActionResult> PostGeolocalizar([FromBody] Direccion direccion)
        {
            direccion.Estado = "PROCESANDO";
            await _addressService.PostAddress(direccion);
            _messageService.SendGeocodificar(direccion);
            return StatusCode((int)HttpStatusCode.Accepted, new {direccion.Id});
        }
    }
}
