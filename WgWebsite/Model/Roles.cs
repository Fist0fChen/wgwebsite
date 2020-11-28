using System;
using System.Collections.Generic;
namespace WgWebsite.Model
{
    public static class Roles
    {
        public static string Admin { get; } = "Admin";
        public static string Guest { get; } = "Guest";
        public static string Karma { get; } = "Karma";
        public static string Drinks { get; } = "Drinks";
        public static string DrinksAdmin { get; } = "DrinksAdmin";
        public static string KarmaGuest { get; } = "KarmaGuest";

        public static List<string> getAll()
        {
            return new List<string>() { Admin, Guest, Karma, Drinks, DrinksAdmin, KarmaGuest };
        }
    }
}
