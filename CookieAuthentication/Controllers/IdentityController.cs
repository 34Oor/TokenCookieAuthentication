using CookieAuthentication.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CookieAuthentication.Controllers
{
   
    public class IdentityController : Controller
    {
       
        [HttpGet("login")]
        public IActionResult Login(string ReturnUrl)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View("login", new LoginModel() { ReturnUrl= ReturnUrl });
        }

        [HttpPost("login")]
        public IActionResult Login(LoginModel loginModel)
        {
            // check model state
            if (!ModelState.IsValid)
                return View(loginModel);
            // verify user credential 
            if(loginModel.UserName == "admin" && loginModel.Password == "password")
            {
                // create security context (claims principal)
                
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, loginModel.UserName),
                    new Claim("password", loginModel.Password),
                    new Claim(ClaimTypes.Role, "admin"),
                    new Claim("department", "HR"),
                    new Claim("EmploymentDate", "2021-10-12")
                };
                var claimsIdentity = new ClaimsIdentity(claims, "cookieAuth");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                var authenticationProperties = new AuthenticationProperties {
                    IsPersistent = loginModel.RememberMe
                };

                // serialise the security context into a cookie
                HttpContext.SignInAsync("cookieAuth", claimsPrincipal, authenticationProperties);

                if(!string.IsNullOrEmpty(loginModel.ReturnUrl))
                    return Redirect(loginModel.ReturnUrl);
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("validation-summary", "faild login attempt");
            return View(loginModel);

        }
        [HttpPost("/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("cookieAuth");
            return RedirectToAction("login", "Identity");
        }

        [HttpGet("/access-denied")]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
