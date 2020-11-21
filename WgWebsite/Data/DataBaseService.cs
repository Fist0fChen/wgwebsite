using System;
using System.Linq;
using System.Collections.Generic;
using WgWebsite.Model;

namespace WgWebsite.Data
{
    public class DataBaseService
    {
        private KarmaDataContext context;
        public DataBaseService()
        {
            context = new KarmaDataContext();
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
    }
}
