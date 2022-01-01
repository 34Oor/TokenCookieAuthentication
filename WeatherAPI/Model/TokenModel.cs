using System;

namespace WeatherAPI.Model
{
    public class TokenModel
    {
        public string AccessToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
