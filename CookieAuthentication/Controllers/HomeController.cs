using CookieAuthentication.DTO;
using CookieAuthentication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace CookieAuthentication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
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

            var httpClient = _httpClientFactory.CreateClient("WeatherForecastApiHttpClient");
            var strJwt = HttpContext.Session.GetString("weather_jwt");
            //if(string.IsNullOrWhiteSpace(strJwt))

            var jwt = JsonConvert.DeserializeObject<TokenModel>(HttpContext.Session.GetString("weather_jwt"));
            if (jwt == null ||
                string.IsNullOrWhiteSpace(jwt.AccessToken) ||
                DateTime.UtcNow > jwt.ExpiresAt)
            {
                var respnse = await httpClient.PostAsJsonAsync("Authenticate",
                   new WeatherApiCredentialModel() { Password = "password", UserName = "admin" });
                respnse.EnsureSuccessStatusCode();
                jwt = JsonConvert.DeserializeObject<TokenModel>(await respnse.Content.ReadAsStringAsync());

            }
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt.AccessToken);
            var WeatherForecasts = await httpClient.GetFromJsonAsync<List<WeatherDTO>>("GetWeatherForecasts");
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
