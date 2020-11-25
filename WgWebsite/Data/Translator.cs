using System.Collections.Generic;
using System.Linq;

namespace WgWebsite.Data
{
    public class Translator
    {
        private Dictionary<string, string> EnDe = new Dictionary<string, string>
        {
            { "Done", "Gemacht" },
            { "Send", "Senden" },
            { "Search Karma Tasks", "Karmatasks durchsuchen" },
            { "NavbarTitle", "R.I.C.H." }
        };
        private Dictionary<string, string> EnIt = new Dictionary<string, string>
        {
            { "Done", "Fatto" },
            { "Send", "Inviare" },
            { "NavbarTitle", "R.I.C.H." }
        };
        private Dictionary<string, string> EnEn = new Dictionary<string, string>
        {
            { "NavbarTitle", "R.I.C.H." }
        };
        private string _setLanguage;
        public Translator()
        {
        }
        public string WordFor(string something)
        {
            switch (_setLanguage)
            {
                case "Deutsch":
                    if (EnDe.TryGetValue(something, out var resde)) return resde;
                    return something;
                case "Italiano":
                    if (EnIt.TryGetValue(something, out var resit)) return resit;
                    return something;
                case "English":
                    if (EnEn.TryGetValue(something, out var resen)) return resen;
                    return something;
                default:
                    return something;
            }
        }
        public string[] Languages()
        {
            return new[] { "Deutsch", "English", "Italiano" };
        }
        public void SetLanguage(string lang)
        {
            if (Languages().ToList().Any(l => l == lang)) _setLanguage = lang;
        }
        public static string German => "Deutsch";
        public static string English => "English";
        public static string Italian => "Italiano";
    }
}
