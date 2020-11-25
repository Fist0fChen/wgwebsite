using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using WgWebsite.Model;

namespace WgWebsite.Data
{
    public class DataBaseService
    {
        private KarmaDataContext context;
        public DataBaseService()
        {
            context = new KarmaDataContext();
            if(context.KarmaBalances.ToList().Count == 0)
            {
                context.KarmaBalances.Add(new KarmaBalance
                {
                    Acknowledged = true,
                    BalanceTo = DateTime.Now
                });
            }
        }
        public bool AddDrink(Drink drink)
        {
            if(drink.Name.Length > 3 && drink.Price > 0)
            {
                context.Drinks.Add(drink);
                context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public User GetUserById(long userid)
        {
            var user = context.Users.FirstOrDefault(u => u.UserId == userid);
            var usercp = new User
            {
                BrowsePosition = user.BrowsePosition,
                Email = user.Email,
                Language = user.Language,
                Name = user.Name,
                Notifications = user.Notifications,
                Role = user.Role,
                Theme = user.Theme,
                UserId = user.UserId
            };
            return usercp;
        }
        public bool AddUser(User user)
        {
            if (user.Email != null && user.Name != null && user.PassHash != null)
            {
                if (user.Email.Contains("@") && !context.Users.Any(u => u.Name == user.Name) && user.Name.Length > 3 &&
                    !context.Users.Any(u => u.Email == user.Email))
                {
                    user.Language = Translator.English;
                    user.Role = Roles.Guest;
                    user.Theme = Themes.Light;
                    user.BrowsePosition = "/";
                    context.Users.Add(user);
                    context.SaveChanges();
                    return true;
                }
            }
                   
            return false;
        }
        public IEnumerable<User> GetUserRoles()
        {
            var users = new List<User>();
            foreach(var u in context.Users)
            {
                users.Add(new User
                {
                    Name = u.Name,
                    Role = u.Role,
                    UserId = u.UserId
                });
            }
            return users;
        }
        public IEnumerable<KarmaTask> GetKarmaTasks()
        {
            return context.Tasks.ToList();
        }
        public IEnumerable<Drink> GetDrinks()
        {
            return context.Drinks.ToList();
        }
        public bool ChangeUserRoles(IEnumerable<User> users)
        {
            bool foundall = true;
            foreach(var user in users)
            {
                EditUser(user, "Role", false);
            }
            context.SaveChangesAsync();
            return foundall;
        }
        public bool EditUser(User user, string parameters, bool save = true)
        {
            var dbuser = context.Users.FirstOrDefault(u => u.UserId == user.UserId);
            if (dbuser == null) return false;
            var allfound = true;
            foreach(var param in parameters.Split(" "))
            switch (param)
            {
                case "Language":
                    dbuser.Language = user.Language;
                    break;
                case "Role":
                    dbuser.Role = user.Role;
                    break;
                case "PassHash":
                    dbuser.PassHash = user.PassHash;
                    break;
                default:
                    allfound = false;
                    break;
            }
            context.Update(dbuser);
            if (save)
            {
                context.SaveChangesAsync();
            }
            return allfound;
        }
        public void EditKarmaTask(KarmaTask task, bool save = true)
        {
            if (task.Description == null) task.Description = "";
            
            if(context.Tasks.Any(t => t.KarmaTaskId == task.KarmaTaskId))
            {
                context.Tasks.Update(task);
            }
            else
            {
                context.Tasks.Add(task);
            }
            if(save)
                context.SaveChangesAsync();
        }
        public void EditKarmaTasks(IEnumerable<KarmaTask> tasks)
        {
            foreach (var t in tasks) EditKarmaTask(t, false);
            context.SaveChangesAsync();
        }
        public bool DoKarma(KarmaEntry entry)
        {
            if(entry.Comment == null || entry.Karma < 0 || entry.UserId < 0 ) return false;
            try
            {
                entry.Timestamp = DateTime.Now;
                context.TasksDone.Add(entry);
                context.SaveChanges();
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }
        public bool DeleteKarmaEntry(long entryid)
        {
            try
            {
                context.TasksDone.Remove(context.TasksDone.FirstOrDefault(e => e.KarmaEntryId == entryid));
                context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public IEnumerable<User> GetKarmaStats()
        {
            var currentPeriod = new DateTime(0);
            foreach(var balance in context.KarmaBalances.ToArray())
            {
                if (balance.BalanceTo != null && balance.BalanceTo > currentPeriod)
                    currentPeriod = balance.BalanceTo;
            }
            var users = context.Users.Include(u => u.KarmaEntries).ToList();
            for(var k = 0; k < users.Count; k++)
            {
                users[k].KarmaEntries = users[k].KarmaEntries.Where(e => e.Timestamp > currentPeriod);
            }
            var pubusers = new List<User>();
            foreach(var u in users)
            {
                pubusers.Add(new User
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    KarmaEntries = u.KarmaEntries
                });
            }
            
            return pubusers;
        }
        public bool ActivateDrinks(IEnumerable<Drink> drinks)
        {
            foreach(var drink in drinks)
            {
                var dbdrink = context.Drinks.FirstOrDefault(d => d.DrinkId == drink.DrinkId);
                if (dbdrink == null) continue;
                dbdrink.Active = drink.Active;
                context.Update(dbdrink);
            }
            context.SaveChangesAsync();
            return true;
        }
        public bool EnterDrink(long userid, long drinkid)
        {
            var drink = context.Drinks.FirstOrDefault(d => d.DrinkId == drinkid);
            var user = context.Users.FirstOrDefault(u => u.UserId == userid);
            if (drink == null || user == null) return false;
            var entry = new DrinkPurchase()
            {
                Challenged = false,
                Cost = drink.Price,
                DrinkId = drink.DrinkId,
                Drink = drink,
                Timestamp = DateTime.Now,
                Comment = "",
                User = user,
                UserId = user.UserId
            };
            context.Purchased.Add(entry);
            context.SaveChangesAsync();
            return true;
        }
        public IEnumerable<User> GetDrinkEntries()
        {
            var pubusers = new List<User>();
            var users = context.Users.Include(u => u.DrinkPurchases);
            foreach(var u in users)
            {
                pubusers.Add(new User
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    DrinkPurchases = u.DrinkPurchases
                });
            }
            return pubusers;
        }
        public bool DeletePurchase(long userid, long purchaseid)
        {
            var user = context.Users.FirstOrDefault(u => u.UserId == userid);
            var purchase = context.Purchased.FirstOrDefault(p => p.DrinkPurchaseId == purchaseid);
            if (user == null || purchase == null) return false;
            var isAdmin = user.Role.Contains(Roles.DrinksAdmin) || user.Role.Contains(Roles.Admin);
            if (isAdmin) context.Purchased.Remove(purchase);
            else
            {
                purchase.Challenged = true;
                context.Purchased.Update(purchase);
            }
            context.SaveChanges();
            return true;
        }
        public bool RestorePurchase(long userid, long purchaseid)
        {
            var user = context.Users.FirstOrDefault(u => u.UserId == userid);
            var purchase = context.Purchased.FirstOrDefault(p => p.DrinkPurchaseId == purchaseid);
            if (user == null || purchase == null) return false;
            var isAdmin = user.Role.Contains(Roles.DrinksAdmin) || user.Role.Contains(Roles.Admin);
            if(purchase.UserId != userid || !isAdmin) return false;
            purchase.Challenged = false;
            context.Update(purchase);
            context.SaveChanges();
            return true;
        }
    }
}
