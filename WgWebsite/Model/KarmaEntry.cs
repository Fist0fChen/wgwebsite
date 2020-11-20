using System;
namespace WgWebsite.Model
{
    public class KarmaEntry
    {
        public long KarmaEntryId { get; set; }
        public DateTime Timestamp { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }
        public string Comment { get; set; }
        public int Karma { get; set; }
        public long? KarmaTaskId { get; set; }
        public KarmaTask KarmaTask { get; set; }
    }
}
