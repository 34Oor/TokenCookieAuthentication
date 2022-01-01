using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using WeatherAPI.Model;
using WeatherAPI.Services;

namespace WeatherAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly AuthControllerService _authControllerService;

        public AuthController(ILogger<WeatherForecastController> logger,
            AuthControllerService authControllerService)
        {
            _logger = logger;
            _authControllerService = authControllerService;
        }


        [HttpPost("Authenticate")]
        public IActionResult Authenticate(CredentialModel credential)
        {
            var jwt = _authControllerService.Authenticate(credential);
            if (jwt == null)
                return Unauthorized();
            return Ok(jwt);
        }
    }
}
