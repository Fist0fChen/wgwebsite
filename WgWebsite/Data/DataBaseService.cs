using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using WgWebsite.Model;

namespace WgWebsite.Data
{
    public class DataBaseService
    {
        private const int Retries = 3;
        private KarmaDataContext context;
        private bool busy;
        private Random rnd;
        private int RandomDelay => 50 + rnd.Next(100);
        public DataBaseService()
        {
            rnd = new Random(DateTime.Now.Millisecond);
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
        private T SafeTaskWrapperSingle<T, U>(U arg, Func<U, T> func, int retry = 0)
        {
            if (busy)
            {
                if (retry >= Retries) return default(T);
                Thread.Sleep(RandomDelay);
                return SafeTaskWrapperSingle(arg, func, ++retry);
            }
            else
            {
                try
                {
                    busy = true;
                    var result = func(arg);
                    busy = false;
                    return result;
                }
                catch (Exception)
                {
                    busy = false;
                    return default(T);
                }
                finally
                {
                    busy = false;
                }
            }
        }
        public bool AddDrink(Drink drink)
        {
            return SafeTaskWrapperSingle(drink, d =>
            {
                if (drink.Name.Length > 3 && drink.Price > 0)
                {
                    context.Drinks.Add(drink);
                    context.SaveChanges();
                    return true;
                }
                return false;
            });
        }
        public bool HighlightTask(long taskid)
        {
            return SafeTaskWrapperSingle(taskid, (taskid) =>
            {
                var task = context.Tasks.FirstOrDefault(t => t.KarmaTaskId == taskid);
                if (task == null) return false;
                task.Highlighted = DateTime.Now;
                context.Tasks.Update(task);
                context.SaveChanges();
                return true;
            }); 
        }
        public User GetUserById(long userid)
        {
            return SafeTaskWrapperSingle(userid, (userid) =>
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
            });
        }
        public bool AddUser(User user)
        {
            return SafeTaskWrapperSingle(user, (user) =>
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
            });
        }
        public IEnumerable<User> GetUserRoles()
        {
            return SafeTaskWrapperSingle(0, n =>
            {
                var users = new List<User>();
                foreach (var u in context.Users)
                {
                    users.Add(new User
                    {
                        Name = u.Name,
                        Role = u.Role,
                        UserId = u.UserId
                    });
                }
                return users;
            });
        }
        public IEnumerable<KarmaTask> GetKarmaTasks()
        {
            return SafeTaskWrapperSingle(0, n => context.Tasks.ToList());
        }
        public IEnumerable<Drink> GetDrinks(int retry = 0)
        {
            return SafeTaskWrapperSingle(0, n => context.Drinks.ToList());
        }
        public bool ChangeUserRoles(IEnumerable<User> users)
        {
            return SafeTaskWrapperSingle(users, users =>
            {
                bool foundall = true;
                foreach (var user in users)
                {
                    EditUserPrivate(user, "Role");
                }
                context.SaveChangesAsync();
                return foundall;
            });
        }
        private bool EditUserPrivate(User user, string parameters)
        {
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
            return allfound;
        }
        public bool EditUser(User user, string parameters)
        {
            return SafeTaskWrapperSingle(user, user =>
            {
                var res = EditUserPrivate(user, parameters);
                context.SaveChanges();
                return res;
            });
        }
        private void EditKarmaTaskPrivate(KarmaTask task)
        {
            _ = SafeTaskWrapperSingle(task, task =>
            {
                if (task.Description == null) task.Description = "";

                if (context.Tasks.Any(t => t.KarmaTaskId == task.KarmaTaskId))
                {
                    context.Tasks.Update(task);
                }
                else
                {
                    context.Tasks.Add(task);
                }
                return false;
            });
        }
        public void EditKarmaTask(KarmaTask task)
        {
            _ = SafeTaskWrapperSingle(task, task => { EditKarmaTaskPrivate(task); return false; });
        }
        public void EditKarmaTasks(IEnumerable<KarmaTask> tasks)
        {
            _ = SafeTaskWrapperSingle(tasks, tasks =>
            {
                foreach (var t in tasks) EditKarmaTaskPrivate(t);
                context.SaveChangesAsync();
                return false;
            });
        }
        public bool DoKarma(KarmaEntry entry)
        {
            return SafeTaskWrapperSingle(entry, entry =>
            {
                if (entry.Comment == null || entry.Karma < 0 || entry.UserId < 1) return false;
                var user = context.Users.FirstOrDefault(u => u.UserId == entry.UserId);
                if (user == null) return false;
                if (user.Role.Split(" ").Any(r => r == Roles.Karma))
                {
                    entry.Approved = true;
                }
                else if (user.Role.Split(" ").Any(r => r == Roles.KarmaGuest))
                {
                    entry.Approved = false;
                }
                else return false;
                var task = context.Tasks.ToList().FirstOrDefault(t => t.KarmaTaskId == entry.KarmaTaskId);
                if (task != null)
                {
                    task.Highlighted = null;
                    context.Tasks.Update(task);
                }
                entry.Timestamp = DateTime.Now;
                context.TasksDone.Add(entry);
                context.SaveChanges();
                return true;
            });
        }
        public bool DeleteKarmaEntry(long entryid)
        {
            return SafeTaskWrapperSingle(entryid, entryid =>
            {
                context.TasksDone.Remove(context.TasksDone.ToList().FirstOrDefault(e => e.KarmaEntryId == entryid));
                context.SaveChanges();
                return true;
            });
        }
        public IEnumerable<User> GetKarmaStats()
        {
            return SafeTaskWrapperSingle(0, n =>
            {
                var currentPeriod = new DateTime(0);
                var balances = context.KarmaBalances.ToList();
                foreach (var balance in balances)
                {
                    if (balance.BalanceTo != null && balance.BalanceTo > currentPeriod)
                        currentPeriod = balance.BalanceTo;
                }
                var users = context.Users.ToList();
                var pubusers = new List<User>();
                foreach (var u in users)
                {
                    if (!(u.Role.Split(" ").Any(r => r == Roles.Karma || r == Roles.KarmaGuest))) continue;
                    pubusers.Add(new User
                    {
                        UserId = u.UserId,
                        Name = u.Name
                    });
                }
                for (var k = 0; k < pubusers.Count; k++)
                {
                    var karmalist = new List<KarmaEntry>();
                    foreach (var td in context.TasksDone.ToList())
                        if (td.UserId == pubusers[k].UserId && td.Timestamp > currentPeriod)
                            karmalist.Add(td);
                    pubusers[k].KarmaEntries = karmalist;
                }
                return pubusers;
            });
        }

        public bool ActivateDrinks(IEnumerable<Drink> drinks)
        {
            return SafeTaskWrapperSingle(drinks, drinks =>
            {
                foreach (var drink in drinks)
                {
                    var dbdrink = context.Drinks.FirstOrDefault(d => d.DrinkId == drink.DrinkId);
                    if (dbdrink == null) continue;
                    dbdrink.Active = drink.Active;
                    context.Update(dbdrink);
                }
                context.SaveChanges();
                return true;
            });
        }
        public bool EnterDrink(long userid, long drinkid)
        {
            return SafeTaskWrapperSingle(drinkid, drinkid =>
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
                context.SaveChanges();
                return true;
            });
        }
        public IEnumerable<User> GetDrinkEntries()
        {
            return SafeTaskWrapperSingle(0, n =>
            {
                var pubusers = new List<User>();
                var users = context.Users.Include(u => u.DrinkPurchases);
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
            });
        }
        public bool DeletePurchase(long userid, long purchaseid)
        {
            return SafeTaskWrapperSingle(purchaseid, purchaseid =>
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
            });
        }
        public bool RestorePurchase(long userid, long purchaseid)
        {
            return SafeTaskWrapperSingle(purchaseid, purchaseid =>
            {
                var user = context.Users.FirstOrDefault(u => u.UserId == userid);
                var purchase = context.Purchased.FirstOrDefault(p => p.DrinkPurchaseId == purchaseid);
                if (user == null || purchase == null)
                    return false;
                var isAdmin = user.Role.Contains(Roles.DrinksAdmin) || user.Role.Contains(Roles.Admin);
                if (purchase.UserId != userid && !isAdmin) return false;
                purchase.Challenged = false;
                context.Update(purchase);
                context.SaveChanges();
                return true;
            });
        }
        public bool EnterPayment(long fromuser, long touser, long amount)
        {
            return SafeTaskWrapperSingle(amount, amount =>
            {
                if (fromuser == touser || amount == 0) return true;
                if (amount < 0) return false;
                if (fromuser < 0 && touser < 0) return false;
                if (fromuser < 0 && touser > 0)
                {
                    if (!context.Users.Any(u => u.UserId == touser))
                        return false;
                    context.Purchased.Add(new DrinkPurchase
                    {
                        Challenged = false,
                        Comment = "payout",
                        Cost = amount,
                        Timestamp = DateTime.Now,
                        UserId = touser
                    });
                }
                else if (fromuser > 0 && touser < 0)
                {
                    if (!context.Users.Any(u => u.UserId == fromuser))
                        return false;
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
                        return false;
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
                return true;
            });
        }
        public IEnumerable<TodoTask> GetTodos()
        {
            return SafeTaskWrapperSingle(0, n => context.Todos.ToList());
        }
        public bool AddTodo(TodoTask todo)
        {
            return SafeTaskWrapperSingle(todo, todo =>
            {
                if (todo.Karma < 0) return false;
                if (todo.Name == null || todo.Name.Length < 3) return false;
                context.Todos.Add(todo);
                context.SaveChanges();
                return true;
            });
        }
        public bool FinishTodo(long todoid, long userid)
        {
            return SafeTaskWrapperSingle(todoid, tid =>
            {
                var user = context.Users.FirstOrDefault(u => u.UserId == userid);
                if (user == null) return false;
                var todo = context.Todos.FirstOrDefault(t => t.TodoTaskId == tid);
                if (todo == null) return false;
                if (todo.Karma < 0) return false;
                if (todo.Karma > 0)
                    context.TasksDone.Add(new KarmaEntry
                    {
                        Karma = todo.Karma,
                        UserId = userid,
                        Comment = "Finished Todo " + todo.Name + " " + todo.TodoTaskId,
                        Timestamp = DateTime.Now,
                        Approved = user.Role.Split(" ").Any(r => r == Roles.Karma)
                    });
                todo.Done = true;
                context.Todos.Update(todo);
                context.SaveChanges();
                return true;
            });
        }
        public bool RemoveTodo(long todoid)
        {
            return SafeTaskWrapperSingle(todoid, todoid =>
            {
                var todo = context.Todos.FirstOrDefault(t => t.TodoTaskId == todoid);
                if (todo == null) return false;
                context.Todos.Remove(todo);
                context.SaveChanges();
                return true;
            });
        }
        public bool ApproveKarmaEntry(long entryid)
        {
            return SafeTaskWrapperSingle(entryid, entryid =>
            {
                if (!context.TasksDone.Any(t => t.KarmaEntryId == entryid)) return false;
                var karmaentry = context.TasksDone.FirstOrDefault(t => t.KarmaEntryId == entryid);
                karmaentry.Approved = true;
                context.TasksDone.Update(karmaentry);
                context.SaveChanges();
                return true;
            });
        }
        public bool DeclineKarmaEntry(long entryid)
        {
            return SafeTaskWrapperSingle(entryid, entryid =>
            {
                if (!context.TasksDone.Any(t => t.KarmaEntryId == entryid)) return false;
                var karmaentry = context.TasksDone.FirstOrDefault(t => t.KarmaEntryId == entryid);
                context.TasksDone.Remove(karmaentry);
                context.SaveChanges();
                return true;
            });
        }
        public bool UnHighlightTask(long taskid)
        {
            return SafeTaskWrapperSingle(taskid, taskid =>
            {
                var task = context.Tasks.FirstOrDefault(t => t.KarmaTaskId == taskid);
                if (task == null) return false;
                task.Highlighted = null;
                context.Tasks.Update(task);
                context.SaveChanges();
                return true;
            });
        }
        public int CurrentKarma(long userid)
        {
            return SafeTaskWrapperSingle(userid, userid =>
            {
                var currentPeriod = new DateTime(0);
                var balances = context.KarmaBalances.ToList();
                foreach (var balance in balances)
                {
                    if (balance.BalanceTo != null && balance.BalanceTo > currentPeriod)
                        currentPeriod = balance.BalanceTo;
                }
                var karma = 0;
                foreach(var entry in context.TasksDone)
                {
                    if (entry.Timestamp > currentPeriod && entry.UserId == userid) karma += entry.Karma;
                }
                return karma;
            });
        }
        public long CurrentDebt(long userid)
        {
            return SafeTaskWrapperSingle(userid, uid =>
            {
                var user = context.Users.Include(u => u.DrinkPurchases).FirstOrDefault(u => u.UserId == uid);
                return user.DrinkPurchases.Aggregate((long)0, (a, b) => a + b.Cost);
            });
        }
    }
}
