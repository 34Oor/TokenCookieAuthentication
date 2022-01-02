using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WeatherAPI.Model;

namespace WeatherAPI.Services
{
    public class AuthControllerService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public AuthControllerService(IConfiguration configuration)
        {

            _configuration = configuration;
        }

        public JwtModel Authenticate (CredentialModel credential)
        {
            try
            {
                if (credential.UserName == "admin" && credential.Password == "password")
                {
                    // Get Claims Form Database
                    var claims = new List<Claim>(){

                        new Claim(ClaimTypes.Name, credential.UserName),
                        new Claim("password", credential.Password),
                        new Claim(ClaimTypes.Role, "admin"),
                        new Claim("department", "HR"),
                        new Claim("EmploymentDate", "2021-10-12")
                    };
                    var expiresAt = DateTime.UtcNow.AddMinutes(10);

                    var jwt = new JwtSecurityToken(
                        claims: claims,
                        notBefore: DateTime.UtcNow,
                        expires: expiresAt,
                        signingCredentials: new SigningCredentials(
                            key: new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration.GetValue<string>("SecurityKey"))),
                            algorithm: SecurityAlgorithms.HmacSha256Signature)
                    );

                    return new JwtModel() { 
                        AccessToken = new JwtSecurityTokenHandler().WriteToken(jwt),
                        ExpiresAt = expiresAt
                    };
                       
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
           
        }

    }
}
