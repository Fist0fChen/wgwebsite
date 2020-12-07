using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.IO;
using WgWebsite;
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
            try
            {
                var mySHA256 = SHA256.Create();
                var users = JsonConvert.DeserializeObject<List<User>>(usersjson);
                var dbusers = dataContext.Users.ToList();
                foreach (var u in users)
                {
                    if (dbusers.Any(dbu => dbu.Name == u.Name)) continue;
                    var theme = Model.Themes.Light;
                    if (u.Theme != null)
                        theme = u.Theme.Contains("dark") ? Model.Themes.Dark : Model.Themes.Light;
                    var newuser = new Model.User
                    {
                        Name = u.Name,
                        Email = u.Mail ?? "",
                        BrowsePosition = "/",
                        Language = Translator.German,
                        Notifications = u.Notifications ?? "",
                        PassHash = mySHA256.ComputeHash(Encoding.ASCII.GetBytes("pizza")),
                        Role = Model.Roles.Guest,
                        Theme = theme
                    };
                    dataContext.Users.Add(newuser);
                }
                dataContext.SaveChanges();
            }
            catch(Exception ex)
            {

            }
        }
        public void MigrateTasks(string tasksjson)
        {
            try
            {
                var tasks = JsonConvert.DeserializeObject<List<KarmaTask>>(tasksjson);
                var dbtasks = dataContext.Tasks.ToList();
                foreach (var t in tasks)
                {
                    if (t.Name == null) continue;
                    if (t.Categories == null) continue;
                    if (dbtasks.Any(dbt => dbt.Name == t.Name)) continue;
                    dataContext.Tasks.Add(new Model.KarmaTask
                    {
                        Name = t.Name.Replace("_", " "),
                        Active = true,
                        Categories = t.Categories.Aggregate((a, b) => a + " " + b),
                        Description = t.Name,
                        Frequency = t.Frequency,
                        Highlighted = null,
                        Karma = t.Karma
                    });
                }
                dataContext.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }
        public void MigrateTaskEntries(string entries, string users, string tasks)
        {
            try
            {
                var extentries = JsonConvert.DeserializeObject<List<KarmaTaskEntry>>(entries);
                var extusers = JsonConvert.DeserializeObject<List<User>>(users);
                var exttasks = JsonConvert.DeserializeObject<List<KarmaTask>>(tasks);

                var dbentries = dataContext.TasksDone.ToList();
                var dbusers = dataContext.Users.ToList();
                var dbtasks = dataContext.Tasks.ToList();
                foreach (var entry in extentries)
                {
                    if (dbentries.Any(e => e.Timestamp == DateFromTimestamp(entry.Date))) continue;
                    var extuser = extusers.FirstOrDefault(u => u.Id == entry.UserId);
                    var exttask = exttasks.FirstOrDefault(t => t.Id == entry.KarmaTaskId);
                    if (extuser == null || exttask == null) throw new Exception("Entry could not be linked to external task or user");
                    var dbuser = dbusers.FirstOrDefault(u => u.Name == extuser.Name);
                    var dbtask = dbtasks.FirstOrDefault(t => t.Name == exttask.Name.Replace("_", " "));
                    if (dbuser == null || dbtask == null) throw new Exception("Entry coud not be linked to task or user in database");
                    dataContext.TasksDone.Add(new Model.KarmaEntry
                    {
                        Approved = true,
                        Comment = "Migrated from Version 1.0 on " + DateTime.Now,
                        Karma = dbtask.Karma,
                        KarmaTaskId = dbtask.KarmaTaskId,
                        Timestamp = DateFromTimestamp(entry.Date),
                        UserId = dbuser.UserId
                    });
                }
                dataContext.SaveChanges();
            }
            catch(Exception ex)
            {

            }
        }
        public void MigrateDrinkEntries(string drinkshistory, string usersext, string productsext)
        {
            try
            {
                var history = JsonConvert.DeserializeObject<List<DrinksEntry>>(drinkshistory);
                var products = JsonConvert.DeserializeObject<List<Product>>(productsext);
                var users = JsonConvert.DeserializeObject<List<User>>(usersext);
                var userDebts = new Dictionary<string, long>();
                foreach(var entry in history)
                {
                    var user = users.FirstOrDefault(u => u.Id == entry.UserId);
                    if (user == null) throw new Exception("user to entry not found");
                    if (!userDebts.ContainsKey(user.Name)) userDebts.Add(user.Name, 0);
                    if (entry.Amount != null)
                    {
                        userDebts[user.Name] -= (long)Math.Round((decimal)entry.Amount * 100);
                    }
                    else if(entry.ProductId != null)
                    {
                        var product = products.FirstOrDefault(p => p.Id == entry.ProductId);
                        if (product == null) throw new Exception("no product found that corresponds to drinks entry");
                        if (entry.Ratio == null) throw new Exception("no ratio given at drinks entry");
                        userDebts[user.Name] += (long)Math.Round((decimal)entry.Ratio * (decimal)product.Cost * 100);
                    }
                    else
                    {
                        throw new Exception("no information found for extracting the cost");
                    }
                }
                foreach(var kv in userDebts)
                {
                    var dbuser = dataContext.Users.FirstOrDefault(u => u.Name == kv.Key);
                    if (dbuser == null) throw new Exception(kv.Key + " not found in users");
                    dataContext.Purchased.Add(new Model.DrinkPurchase
                    {
                        Challenged = false,
                        Comment = "cumulative debt from migration",
                        Cost = kv.Value,
                        Timestamp = DateTime.Now,
                        UserId = dbuser.UserId
                    });
                }
                dataContext.SaveChanges();
            }
            catch(Exception ex)
            {

            }
        }
        private DateTime DateFromTimestamp(long time)
        {
            var date = new DateTime(1970, 1, 1, 0, 0, 0);
            date = date.AddMilliseconds(time);
            return date;
        }
    }
    internal class Balance
    {
        public int Id;
        public DateTime Date;
        public List<User> Users;
        public int Karma;
        public bool Balanced;

    }
    internal class User
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
    internal class Product
    {
        public int Id;
        public string Name;
        public float Cost;
        public bool Expired;
    }
    internal class KarmaTask
    {
        public int Id;
        public string Name;
        public float Frequency;
        public int Karma;
        public List<string> Categories;
        public long Todo;
    }
    internal class KarmaTaskEntry
    {
        public int UserId;
        public long Date;
        public int KarmaTaskId;
    }
    internal class DrinksEntry
    {
        public float? Amount;
        public int? ProductId;
        public int UserId;
        public long Timestamp;
        public float? Ratio;
        public int? NEntries;
    }
}
