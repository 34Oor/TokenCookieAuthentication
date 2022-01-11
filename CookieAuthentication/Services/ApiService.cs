using CookieAuthentication.DTO;
using CookieAuthentication.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CookieAuthentication.Services
{
    public class ApiService
    {
        // this service assuming that the authentication provider is pinded together with resources.
        public ApiService(IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ApiService> logger)
        {
            HttpClientFactory = httpClientFactory;
            HttpContextAccessor = httpContextAccessor;
            Logger = logger;
        }

        private IHttpClientFactory HttpClientFactory { get; }
        private IHttpContextAccessor HttpContextAccessor { get; }
        private ILogger<ApiService> Logger { get; }

        public async Task<T> InvokeEndPointAsync<T>(string clientName, string Uri, string sessionJwtName, string jwtEndPointUri, object apiCredential)
        {
            try
            {
                
                var strJwt = HttpContextAccessor.HttpContext.Session.GetString(sessionJwtName);
                JwtModel jwt = ValidateSessionJwt(strJwt);
                // if not valid session JWT, get one from auth provider. 
                if (jwt == null)
                    jwt = await GetJwt(clientName, jwtEndPointUri, apiCredential, sessionJwtName);

                // if credential provided is not authenticated
                if (jwt == null)
                    return default;

                var httpClient = HttpClientFactory.CreateClient(clientName);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",jwt.AccessToken);
                return await httpClient.GetFromJsonAsync<T>(Uri);
            }catch(Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return default;
            }
        }

        private static JwtModel ValidateSessionJwt(string strJwt)
        {
            if (string.IsNullOrWhiteSpace(strJwt))
                return null;

            var jwt = JsonConvert.DeserializeObject<JwtModel>(strJwt);
            if (jwt == null ||
                string.IsNullOrWhiteSpace(jwt.AccessToken) ||
                DateTime.UtcNow > jwt.ExpiresAt)
                return null;
            return jwt;
        }
        private async Task<JwtModel> GetJwt(string clientName, string uri, object apiCredential, string sessionJwtName)
        {
            // this function get authentication jwt from the authentication provider
            try
            {
                var httpClient = HttpClientFactory.CreateClient(clientName);
                var response = await httpClient.PostAsJsonAsync(uri, apiCredential);
                response.EnsureSuccessStatusCode();
                var strJwt = await response.Content.ReadAsStringAsync();

                if (strJwt == null || string.IsNullOrWhiteSpace(strJwt))
                    return null;
                HttpContextAccessor.HttpContext.Session.SetString(sessionJwtName, strJwt);
                return JsonConvert.DeserializeObject<JwtModel>(strJwt);
            }catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return null;
            }
           
        }
    }
}
