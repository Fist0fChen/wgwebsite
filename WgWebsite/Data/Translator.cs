using System;
namespace WgWebsite.Data
{
    public class Translator
    {
        public Translator()
        {
        }
        public string WordFor(string something)
        {
            return something;
        }
        public string[] Languages()
        {
            return new[] { "Deutsch", "English", "Italiano" };
        }
        public static string German => "Deutsch";
        public static string English => "English";
        public static string Italian => "Italiano";
    }
}
