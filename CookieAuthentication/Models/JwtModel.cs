using Newtonsoft.Json;
using System;

namespace CookieAuthentication.DTO
{
    public class JwtModel
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("expires_at")]
        public DateTime ExpiresAt { get; set; }
    }
}
