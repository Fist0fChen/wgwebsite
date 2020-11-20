using System;
namespace WgWebsite.Model
{
    public class DrinkPurchase
    {
        public long DrinkPurchaseId { get; set; }
        public DateTime Timestamp { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }
        public string Comment { get; set; }
        public long Cost { get; set; }
        public long? DrinkId { get; set; }
        public Drink Drink { get; set; }
        public bool Challenged { get; set; }
    }
}
