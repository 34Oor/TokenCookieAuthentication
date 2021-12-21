using CookieAuthentication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CookieAuthentication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet("")]
        [HttpGet("index")]
        [Authorize]
        public IActionResult Index()
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

        [HttpGet("hr-dashboard")]
        [Authorize(Policy = "RequireBelongsToHrDeptAdmins")]
        public IActionResult HrDashboard()
        {
            return View();
        }
        [HttpGet("hr-regular")]
        [Authorize(Policy = "RequireBelongsToHrDept")]
        public IActionResult HrRegular()
        {
            return View();
        }

    }
}
