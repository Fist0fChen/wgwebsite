using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using WgWebsite.Model;
using System.Threading;

namespace WgWebsite.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        public string ReturnUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(string paramUsername, string paramPassword)
        {
            string returnUrl = Url.Content("~/");
            try
            {
                // Clear the existing external cookie
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            catch { }


            // *** !!! This is where you would validate the user !!! ***
            var dbcontext = new Data.KarmaDataContext();
            var mySHA256 = SHA256.Create();
            User user;
            string userid = "";
            List<string> roles = new List<string>();
            if(!Program.Test)
                foreach(var u in dbcontext.Users){
                    if((u.Name == paramUsername || u.Email == paramUsername) &&
                        CheckHash(u.PassHash, mySHA256.ComputeHash(Encoding.ASCII.GetBytes(paramPassword))))
                    {
                        user = u;
                        userid = user.UserId.ToString();
                        roles = user.Role.Split(" ").ToList();
                        break;
                    }
                }
            else
            {
                user = dbcontext.Users.FirstOrDefault(u => u.Name == paramUsername || u.Email == paramUsername);
                if(user != null)
                {
                    user.PassHash = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(paramPassword));
                    dbcontext.Users.Update(user);
                }
                else
                {
                    dbcontext.Users.Add(new User
                    {
                        Name = paramUsername,
                        PassHash = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(paramPassword)),
                        Role = "Admin"
                    });
                }
                dbcontext.SaveChanges();
                user = dbcontext.Users.FirstOrDefault(u => u.Name == paramUsername);
            }
            if (userid == "")
            {
                Thread.Sleep(10000);
                return LocalRedirect(returnUrl);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, paramUsername),
                new Claim("UserId", userid)
            };
            roles.ForEach(r => claims.Add(new Claim("Role", r)));

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                RedirectUri = this.Request.Host.Value
            };

            try
            {
                await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
            }

            catch (Exception ex)
            {
                string error = ex.Message;
            }
            return LocalRedirect(returnUrl);
        }

        private bool CheckHash(byte[] a, byte[] b)
        {
            var res = true;
            for(var k = 0; k < a.Length; k++)
            {
                if (a[k] == b[k]) continue;
                res = false;
                break;
            }
            return res;
        }
    }
}
