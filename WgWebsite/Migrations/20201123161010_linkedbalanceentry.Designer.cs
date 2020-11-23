﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WgWebsite.Data;

namespace WgWebsite.Migrations
{
    [DbContext(typeof(KarmaDataContext))]
    [Migration("20201123161010_linkedbalanceentry")]
    partial class linkedbalanceentry
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("WgWebsite.Model.Drink", b =>
                {
                    b.Property<long>("DrinkId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<bool>("Active")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.HasKey("DrinkId");

                    b.ToTable("Drinks");
                });

            modelBuilder.Entity("WgWebsite.Model.DrinkPurchase", b =>
                {
                    b.Property<long>("DrinkPurchaseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<bool>("Challenged")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Comment")
                        .HasColumnType("text");

                    b.Property<long>("Cost")
                        .HasColumnType("bigint");

                    b.Property<long?>("DrinkId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("DrinkPurchaseId");

                    b.HasIndex("DrinkId");

                    b.HasIndex("UserId");

                    b.ToTable("Purchased");
                });

            modelBuilder.Entity("WgWebsite.Model.KarmaBalance", b =>
                {
                    b.Property<long>("KarmaBalanceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<bool>("Acknowledged")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("BalanceFrom")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("BalanceTo")
                        .HasColumnType("datetime");

                    b.HasKey("KarmaBalanceId");

                    b.ToTable("KarmaBalances");
                });

            modelBuilder.Entity("WgWebsite.Model.KarmaBalanceEntry", b =>
                {
                    b.Property<long>("KarmaBalanceEntryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<long>("Karma")
                        .HasColumnType("bigint");

                    b.Property<long>("KarmaBalanceId")
                        .HasColumnType("bigint");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("KarmaBalanceEntryId");

                    b.HasIndex("KarmaBalanceId");

                    b.HasIndex("UserId");

                    b.ToTable("BalanceEntries");
                });

            modelBuilder.Entity("WgWebsite.Model.KarmaEntry", b =>
                {
                    b.Property<long>("KarmaEntryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<string>("Comment")
                        .HasColumnType("text");

                    b.Property<int>("Karma")
                        .HasColumnType("int");

                    b.Property<long?>("KarmaTaskId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("KarmaEntryId");

                    b.HasIndex("KarmaTaskId");

                    b.HasIndex("UserId");

                    b.ToTable("TasksDone");
                });

            modelBuilder.Entity("WgWebsite.Model.KarmaTask", b =>
                {
                    b.Property<long>("KarmaTaskId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<bool>("Active")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Categories")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<float>("Frequency")
                        .HasColumnType("float");

                    b.Property<DateTime?>("Highlighted")
                        .HasColumnType("datetime");

                    b.Property<int>("Karma")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("KarmaTaskId");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("WgWebsite.Model.TodoTask", b =>
                {
                    b.Property<long>("TodoTaskId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<bool>("Done")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("Karma")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<ulong?>("UserId")
                        .HasColumnType("bigint");

                    b.Property<long?>("UserId1")
                        .HasColumnType("bigint");

                    b.HasKey("TodoTaskId");

                    b.HasIndex("UserId1");

                    b.ToTable("Todos");
                });

            modelBuilder.Entity("WgWebsite.Model.User", b =>
                {
                    b.Property<long>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<string>("BrowsePosition")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("Language")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Notifications")
                        .HasColumnType("text");

                    b.Property<byte[]>("PassHash")
                        .HasColumnType("varbinary(4000)");

                    b.Property<string>("Role")
                        .HasColumnType("text");

                    b.Property<string>("Theme")
                        .HasColumnType("text");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("WgWebsite.Model.DrinkPurchase", b =>
                {
                    b.HasOne("WgWebsite.Model.Drink", "Drink")
                        .WithMany()
                        .HasForeignKey("DrinkId");

                    b.HasOne("WgWebsite.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WgWebsite.Model.KarmaBalanceEntry", b =>
                {
                    b.HasOne("WgWebsite.Model.KarmaBalance", "KarmaBalance")
                        .WithMany("Entries")
                        .HasForeignKey("KarmaBalanceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WgWebsite.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WgWebsite.Model.KarmaEntry", b =>
                {
                    b.HasOne("WgWebsite.Model.KarmaTask", "KarmaTask")
                        .WithMany()
                        .HasForeignKey("KarmaTaskId");

                    b.HasOne("WgWebsite.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WgWebsite.Model.TodoTask", b =>
                {
                    b.HasOne("WgWebsite.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId1");
                });
#pragma warning restore 612, 618
        }
    }
}
