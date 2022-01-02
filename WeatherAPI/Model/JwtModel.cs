using System;

namespace WeatherAPI.Model
{
    public class JwtModel
    {
        public string AccessToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
