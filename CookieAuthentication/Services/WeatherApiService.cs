using CookieAuthentication.DTO;
using CookieAuthentication.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CookieAuthentication.Services
{
    public class WeatherApiService
    {
        public WeatherApiService(IHttpClientFactory httpClientFactory)
        {
            HttpClientFactory = httpClientFactory;
        }

        public IHttpClientFactory HttpClientFactory { get; }

        public async Task<T> InvokeEndPoint<T>(string clientName, string Uri)
        {

            var httpClient = HttpClientFactory.CreateClient(clientName);
            var respnse = await httpClient.PostAsJsonAsync(Uri,
                  new WeatherApiCredentialModel() { Password = "password", UserName = "admin" });
            respnse.EnsureSuccessStatusCode();
            var jwt = JsonConvert.DeserializeObject<TokenModel>(await respnse.Content.ReadAsStringAsync());
        }
    }
}
