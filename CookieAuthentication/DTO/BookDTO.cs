using System;

namespace CookieAuthentication.DTO
{
    public class BookDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public string Description { get; set; }
        public string CoverUrl { get; set; }
        public DateTime Created { get; set; }
        public float Rate { get; set; }

    }
}
