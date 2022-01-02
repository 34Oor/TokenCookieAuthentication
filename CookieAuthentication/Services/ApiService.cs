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
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CookieAuthentication.Services
{
    public class ApiService
    {
        // this service assuming that the authentication provider is pinded together with resources.
        public ApiService(IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            ILogger logger)
        {
            HttpClientFactory = httpClientFactory;
            HttpContext = httpContextAccessor.HttpContext;
            Logger = logger;
        }

        private IHttpClientFactory HttpClientFactory { get; }
        private HttpContext HttpContext { get; }
        private ILogger Logger { get; }

        public async Task<T> InvokeEndPointAsync<T>(string clientName, string Uri, string sessionJwtName, string jwtEndPointUri, object apiCredential)
        {
            try
            {
                JwtModel jwt = null;
                var strJwt = HttpContext.Session.GetString(sessionJwtName);
                // if no jwt in session, get one from auth provider. 
                if (string.IsNullOrWhiteSpace(strJwt))
                    jwt = await GetJwt(clientName, jwtEndPointUri, apiCredential);
                // if we have jwt in session
                else
                {
                    jwt = JsonConvert.DeserializeObject<JwtModel>(strJwt);
                    // if the jwt in the session is not valid any more, get one from auth provider.
                    if (jwt == null ||
                        string.IsNullOrWhiteSpace(jwt.AccessToken) ||
                        DateTime.UtcNow > jwt.ExpiresAt)
                        jwt = await GetJwt(clientName, jwtEndPointUri, apiCredential);
                }
                // if credential provided is not authenticated
                if (jwt == null)
                    return JsonConvert.DeserializeObject<T>("");

                var httpClient = HttpClientFactory.CreateClient(clientName);
                return await httpClient.GetFromJsonAsync<T>(Uri);
            }catch(Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return JsonConvert.DeserializeObject<T>("");
            }
           
        }


        public async Task<JwtModel> GetJwt(string clientName, string uri, object apiCredential)
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
                return JsonConvert.DeserializeObject<JwtModel>(strJwt);
            }catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return null;
            }
           
        }
    }
}
