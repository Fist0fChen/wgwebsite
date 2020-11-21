using System;
namespace WgWebsite.Model
{
    public class User
    {
        public long UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public byte[] PassHash { get; set; }
        public string Role { get; set; }
        public string BrowsePosition { get; set; }
        public string Theme { get; set; }
        public string Notifications { get; set; }
        public string Language { get; set; }
    }
}
