using System;
namespace WgWebsite.Model
{
    public class KarmaTask
    {
        public long KarmaTaskId { get; set; }
        public DateTime? Highlighted { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public int Karma { get; set; }
        public string Description { get; set; }
        public string Categories { get; set; }
        public float Frequency { get; set; }
    }
}
