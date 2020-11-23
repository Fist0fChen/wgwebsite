using System;
namespace WgWebsite.Model
{
    public static class Themes
    {
        public static string Light => "Light";
        public static string Dark => "Dark";
        public static string MeillerKiffer => "MeillerKiffer";
        public static string Aldi => "Aldi";
        public static string Icon(string name)
        {
            return "icons/" + name + ".svg";
        }
    }
}
