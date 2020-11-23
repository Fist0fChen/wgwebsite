using System;
namespace WgWebsite.Model
{
    public class KarmaBalanceEntry
    {
        public long KarmaBalanceEntryId { get; set; }
        public long KarmaBalanceId { get; set; }
        public KarmaBalance KarmaBalance { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }
        public long Karma { get; set; }
    }
}
