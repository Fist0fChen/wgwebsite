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
        private bool busy;
        public DataBaseService()
        {
            context = new KarmaDataContext();
            /*if(context.KarmaBalances.ToList().Count == 0)
            {
                context.KarmaBalances.Add(new KarmaBalance
                {
                    Acknowledged = true,
                    BalanceTo = DateTime.Now
                });
            }*/
        }
        public bool AddDrink(Drink drink)
        {
            if (busy) return false;
            try
            {
                if (drink.Name.Length > 3 && drink.Price > 0)
                {
                    busy = true;
                    context.Drinks.Add(drink);
                    context.SaveChanges();
                    busy = false;
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                busy = false;
                return false;
            }
            finally
            {
                busy = false;
            }
        }
        public bool HighlightTask(long taskid)
        {
            if (busy) return false;
            try
            {
                var task = context.Tasks.FirstOrDefault(t => t.KarmaTaskId == taskid);
                if (task == null) return false;
                busy = true;
                task.Highlighted = DateTime.Now;
                context.Tasks.Update(task);
                context.SaveChanges();
                busy = false;
                return true;
            }
            catch (Exception)
            {
                busy = false;
                return false;
            }
            finally
            {
                busy = false;
            }
        }
        public User GetUserById(long userid)
        {
            var user = context.Users.FirstOrDefault(u => u.UserId == userid);
            if (user == null) return null;
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
            if (busy) return false;
            try
            {
                busy = true;
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
                        busy = false;
                        return true;
                    }
                }
                busy = false;
                return false;
            }
            catch (Exception)
            {
                busy = false;
                return false;
            }
            finally
            {
                busy = false;
            }
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
            if (busy) return false;
            try {
                busy = true;
                var dbuser = context.Users.FirstOrDefault(u => u.UserId == user.UserId);
                if (dbuser == null)
                {
                    busy = false;
                    return false;
                }
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
                busy = false;
                return allfound;
            }
            catch (Exception)
            {
                busy = false;
                return false;
            }
            finally
            {
                busy = false;
            }
        }
        public void EditKarmaTask(KarmaTask task, bool save = true)
        {
            try
            {
                if (task.Description == null) task.Description = "";
                if (busy) return;
                busy = true;
                if (context.Tasks.Any(t => t.KarmaTaskId == task.KarmaTaskId))
                {
                    context.Tasks.Update(task);
                }
                else
                {
                    context.Tasks.Add(task);
                }
                if (save)
                    context.SaveChangesAsync();
                busy = false;
            }
            catch (Exception)
            {
                busy = false;
            }
            finally
            {
                busy = false;
            }
        }
        public void EditKarmaTasks(IEnumerable<KarmaTask> tasks)
        {
            foreach (var t in tasks) EditKarmaTask(t, false);
            context.SaveChangesAsync();
        }
        public bool DoKarma(KarmaEntry entry)
        {
            if(entry.Comment == null || entry.Karma < 0 || entry.UserId < 0 || busy) return false;
            try
            {
                busy = true;
                var task = context.Tasks.ToList().FirstOrDefault(t => t.KarmaTaskId == entry.KarmaTaskId);
                if(task != null)
                {
                    task.Highlighted = null;
                    context.Tasks.Update(task);
                }
                entry.Timestamp = DateTime.Now;
                context.TasksDone.Add(entry);
                context.SaveChanges();
                busy = false;
                return true;
            }
            catch(Exception)
            {
                busy = false;
                return false;
            }
            finally
            {
                busy = false;
            }
        }
        public bool DeleteKarmaEntry(long entryid)
        {
            if (busy) return false;
            try
            {
                busy = true;
                context.TasksDone.Remove(context.TasksDone.ToList().FirstOrDefault(e => e.KarmaEntryId == entryid));
                context.SaveChanges();
                busy = false;
                return true;
            }
            catch (Exception)
            {
                busy = false;
                return false;
            }
            finally
            {
                busy = false;
            }
        }
        public IEnumerable<User> GetKarmaStats()
        {
            if (busy) return null;
            try
            {
                busy = true;
                var currentPeriod = new DateTime(0);
                foreach(var balance in context.KarmaBalances.ToArray())
                {
                    if (balance.BalanceTo != null && balance.BalanceTo > currentPeriod)
                        currentPeriod = balance.BalanceTo;
                }
                var users = context.Users.ToList();
                for(var k = 0; k < users.Count; k++)
                {
                    var karmalist = new List<KarmaEntry>();
                    foreach (var td in context.TasksDone.ToList())
                        if (td.UserId == users[k].UserId)
                            karmalist.Add(td);
                    users[k].KarmaEntries = karmalist;
                }
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
                busy = false;
                return pubusers;
            }
            catch (Exception)
            {
                busy = false;
                return null;
            }
            finally
            {
                busy = false;
            }
        }
        public bool ActivateDrinks(IEnumerable<Drink> drinks)
        {
            if (busy) return false;
            try
            {
                busy = true;
                foreach (var drink in drinks)
                {
                    var dbdrink = context.Drinks.FirstOrDefault(d => d.DrinkId == drink.DrinkId);
                    if (dbdrink == null) continue;
                    dbdrink.Active = drink.Active;
                    context.Update(dbdrink);
                }
                context.SaveChangesAsync();
                return true;
            }
            catch(Exception)
            {
                busy = false;
                return false;
            }
            finally
            {
                busy = false;
            }
        }
        public bool EnterDrink(long userid, long drinkid)
        {
            if (busy) return false;
            var drink = context.Drinks.FirstOrDefault(d => d.DrinkId == drinkid);
            var user = context.Users.FirstOrDefault(u => u.UserId == userid);
            if (drink == null || user == null) return false;
            try
            {
                busy = true;
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
            catch (Exception)
            {
                busy = false;
                return false;
            }
            finally
            {
                busy = false;
            }
        }
        public IEnumerable<User> GetDrinkEntries()
        {
            if (busy) return null;
            var pubusers = new List<User>();
            var users = context.Users.Include(u => u.DrinkPurchases);
            try
            {
                busy = true;
                foreach (var u in users)
                {
                    pubusers.Add(new User
                    {
                        UserId = u.UserId,
                        Name = u.Name,
                        DrinkPurchases = u.DrinkPurchases
                    });
                }
                busy = false;
                return pubusers;
            }
            catch (Exception)
            {
                busy = false;
                return null;
            }
            finally
            {
                busy = false;
            }
        }
        public bool DeletePurchase(long userid, long purchaseid)
        {
            if (busy) return false;
            var user = context.Users.FirstOrDefault(u => u.UserId == userid);
            var purchase = context.Purchased.FirstOrDefault(p => p.DrinkPurchaseId == purchaseid);
            if (user == null || purchase == null) return false;
            var isAdmin = user.Role.Contains(Roles.DrinksAdmin) || user.Role.Contains(Roles.Admin);
            try
            {
                busy = true;
                if (isAdmin) context.Purchased.Remove(purchase);
                else
                {
                    purchase.Challenged = true;
                    context.Purchased.Update(purchase);
                }
                context.SaveChanges();
                busy = false;
                return true;
            }
            catch (Exception)
            {
                busy = false;
                return false;
            }
            finally
            {
                busy = false;
            }
        }
        public bool RestorePurchase(long userid, long purchaseid)
        {
            if (busy) return false;
            var user = context.Users.FirstOrDefault(u => u.UserId == userid);
            var purchase = context.Purchased.FirstOrDefault(p => p.DrinkPurchaseId == purchaseid);
            if (user == null || purchase == null) return false;
            var isAdmin = user.Role.Contains(Roles.DrinksAdmin) || user.Role.Contains(Roles.Admin);
            try
            {
                busy = true;
                if (purchase.UserId != userid && !isAdmin) return false;
                purchase.Challenged = false;
                context.Update(purchase);
                context.SaveChanges();
                busy = false;
                return true;
            }
            catch (Exception)
            {
                busy = false;
                return false;
            }
            finally
            {
                busy = false;
            }
        }
        public bool EnterPayment(long fromuser, long touser, long amount)
        {
            if (busy) return false;
            if (fromuser == touser || amount == 0) return true;
            if (amount < 0) return false;
            if (fromuser < 0 && touser < 0) return false;
            try
            {
                busy = true;
                if (fromuser < 0 && touser > 0)
                {
                    if (!context.Users.Any(u => u.UserId == touser))
                    {
                        busy = false;
                        return false;
                    }
                    context.Purchased.Add(new DrinkPurchase
                    {
                        Challenged = false,
                        Comment = "payout",
                        Cost = amount,
                        Timestamp = DateTime.Now,
                        UserId = touser
                    });
                }
                else if(fromuser > 0 && touser < 0)
                {
                    if (!context.Users.Any(u => u.UserId == fromuser))
                    {
                        busy = false;
                        return false;
                    }
                    context.Purchased.Add(new DrinkPurchase
                    {
                        Challenged = false,
                        Comment = "deposit",
                        Cost = -amount,
                        Timestamp = DateTime.Now,
                        UserId = fromuser
                    });
                }
                else
                {
                    if (!context.Users.Any(u => u.UserId == fromuser) || !context.Users.Any(u => u.UserId == touser))
                    {
                        busy = false;
                        return false;
                    }
                    context.Purchased.Add(new DrinkPurchase
                    {
                        Challenged = false,
                        Comment = "transfer to " + touser,
                        Cost = amount,
                        Timestamp = DateTime.Now,
                        UserId = fromuser
                    });
                    context.Purchased.Add(new DrinkPurchase
                    {
                        Challenged = false,
                        Comment = "transfer from " + fromuser,
                        Cost = -amount,
                        Timestamp = DateTime.Now,
                        UserId = touser
                    });
                }

                context.SaveChanges();
                busy = false;
                return true;
            }
            catch (Exception)
            {
                busy = false;
                return false;
            }
            finally
            {
                busy = false;
            }
        }
        public IEnumerable<TodoTask> GetTodos()
        {
            return context.Todos.ToList();
        }
        public bool AddTodo(TodoTask todo)
        {
            if (busy) return false;
            if (todo.Name == null) return false;
            try
            {
                busy = true;
                context.Todos.Add(todo);
                context.SaveChanges();
                busy = false;
                return true;
            }
            catch (Exception)
            {
                busy = false;
                return false;
            }
            finally
            {
                busy = false;
            }
        }
    }
}
