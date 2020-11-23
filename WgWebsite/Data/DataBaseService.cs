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
        public User GetUserById(long userid)
        {
            var user = context.Users.FirstOrDefault(u => u.UserId == userid);
            if(user != null) user.PassHash = null;
            return user;
        }
        public bool AddUser(User user)
        {
            if (user.Email != null && user.Name != null && user.PassHash != null)
            {
                if (user.Email.Contains("@") && !context.Users.Any(u => u.Name == user.Name) && user.Name.Length > 3 &&
                    !context.Users.Any(u => u.Email == user.Email))
                {
                    context.Users.Add(user);
                    user.Language = Translator.English;
                    user.Role = Roles.Guest;
                    user.Theme = Themes.Light;
                    user.BrowsePosition = "/";
                    context.SaveChangesAsync();
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
                default:
                    allfound = false;
                    break;
            }
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
            var users = context.Users.Include(u => u.Entries);
            return users;
        }
    }
}
