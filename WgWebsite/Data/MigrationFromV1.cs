using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace WgWebsite.Data.Migration
{
    public class MigrationFromV1
    {
        private KarmaDataContext dataContext;

        public MigrationFromV1()
        {
            dataContext = new KarmaDataContext();
        }

        public void MigrateUsers(string usersjson)
        {
            var mySHA256 = SHA256.Create();
            var users = JsonConvert.DeserializeObject<List<User>>(usersjson);
            var dbusers = dataContext.Users.ToList();
            foreach(var u in users)
            {
                if (dbusers.Any(dbu => dbu.Name == u.Name)) continue;
                dataContext.Users.Add(new Model.User
                {
                    Name = u.Name,
                    Email = u.Mail,
                    BrowsePosition = "/",
                    Language = Data.Translator.German,
                    Notifications = u.Notifications,
                    PassHash = mySHA256.ComputeHash(Encoding.ASCII.GetBytes("pizza")),
                    Role = Model.Roles.Guest,
                    Theme = u.Theme.Contains("dark") ? Model.Themes.Dark : Model.Themes.Light
                });
            }
            dataContext.SaveChanges();
        }
    }
    public class Balance
    {
        public int Id;
        public DateTime Date;
        public List<User> Users;
        public int Karma;
        public bool Balanced;

    }
    public class User
    {
        public int Id;
        public string Name;
        public string Token;
        public string Permissions;
        public string BrowsePosition;
        public string Theme;
        public string Notifications;
        public string Mail;
    }
    public class DrinksHistory
    {
        public int UserId;
        public int ProductId;

    }
    public class Product
    {
        public int Id;
        public string Name;
        public float Cost;
        public bool Expired;
    }
}
