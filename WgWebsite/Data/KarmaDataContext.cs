using System;
using Microsoft.EntityFrameworkCore;
using WgWebsite.Model;

namespace WgWebsite.Data
{
    public class KarmaDataContext : DbContext
    {
        public KarmaDataContext()
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseMySQL("server=karmadb;database=KarmaData;user=root;password=r1chl1k35b33r4nd");
            //builder.UseMySQL("server=localhost;database=KarmaData;user=root;password=r1chl1k35b33r4nd");
        }

        public DbSet<Drink> Drinks { get; set; }
        public DbSet<DrinkPurchase> Purchased { get; set; }
        public DbSet<KarmaTask> Tasks { get; set; }
        public DbSet<KarmaEntry> TasksDone { get; set; }
        public DbSet<TodoTask> Todos { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<KarmaBalanceEntry> BalanceEntries { get; set; }
        public DbSet<KarmaBalance> KarmaBalances { get; set; }
    }
}
