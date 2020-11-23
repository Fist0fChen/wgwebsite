using System;
using System.Collections;
using System.Collections.Generic;

namespace WgWebsite.Model
{
    public class KarmaBalance
    {
        public long KarmaBalanceId { get; set; }
        public IEnumerable<KarmaBalanceEntry> Entries { get; set; }
        public DateTime BalanceFrom { get; set; }
        public DateTime BalanceTo { get; set; }
        public bool Acknowledged { get; set; }
    }
}
