using System;
namespace WgWebsite.Model
{
    public class TodoTask
    {
        public long TodoTaskId { get; set; }
        public string Name { get; set; }
        public ulong? UserId { get; set; }
        public User User { get; set; }
        public bool Done { get; set; }
        public int Karma { get; set; }
    }
}
