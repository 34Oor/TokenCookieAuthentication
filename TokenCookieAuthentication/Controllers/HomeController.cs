using CookieAuthentication.DTO;
using CookieAuthentication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using CookieAuthentication.Services;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;

namespace CookieAuthentication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ApiService _apiService;

        public HomeController(ILogger<HomeController> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ApiService apiService)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _apiService = apiService;
        }

        [HttpGet("")]
        [HttpGet("index")]
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
   

        [HttpGet("hr-dashboard")]
        //[Authorize(Policy = "RequireBelongsToHrDeptAdmins")]
        public async Task<IActionResult> HrDashboard()
        {
            var WeatherForecasts = await _apiService.InvokeEndPointAsync<IEnumerable<WeatherDTO>>(
                clientName: _configuration.GetValue<string>("WeatherForecastApiHttpClientName"),
                Uri: "WeatherForecast/GetWeatherForecasts",
                sessionJwtName: "weather_jwt",
                jwtEndPointUri: "Auth/Authenticate",
                new WeatherApiCredentialDTO() { Password = "password", UserName = "admin" });

            return View(WeatherForecasts);
        }

        [HttpGet("hr-regular")]
        [Authorize(Policy = "RequireBelongsToHrDept")]
        public IActionResult HrRegular()
        {
            return View();
        }


        [HttpGet("privacy")]
        public IActionResult Privacy()
        {
            
            var smtpClient = new SmtpClient("smtp.gmail.com", 587) { 
                Credentials = new NetworkCredential("a.ashour.dev@gmail.com", "Ahmed&&Ashour01156923737"),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
            smtpClient.Send(
                new MailMessage(from: "a.ashour.dev@gmail.com", to: "ahmedashor461@gmail.com")
                {
                    Subject = "Test",
                    Body = $"PathBase: { base.HttpContext.Request.PathBase} \n Path: { base.HttpContext.Request.Path} \n Host: { base.HttpContext.Request.Host}"
           
                });
            return View();
        }
        [HttpGet("error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
