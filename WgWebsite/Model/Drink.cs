using System;
namespace WgWebsite.Model
{
    public class Drink
    {
        public long DrinkId { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public bool Active { get; set; }
    }
}
