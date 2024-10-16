﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; // Important to include, so DbContext gets regionized
namespace Barroc_Intens.Data
{
    internal class AppDbContext : DbContext
    {
        // DbSet for each model
        // Example: public DbSet<User> Users { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<User> Users { get; set; }

        // Override OnConfiguring from DbContext
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("server=localhost;port=3306;user=root;password=;database=barrocintens",ServerVersion.Parse("8.0.30"));
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            // Example seed data
            //modelBuilder.Entity<Message>()
            //.HasOne(m => m.Chatroom)
            //.WithMany(c => c.Messages)
            //.HasForeignKey(m => m.ChatroomId);

            //modelBuilder.Entity<Message>()
            //    .HasOne(m => m.User)
            //    .WithMany(u => u.Messages)
            //    .HasForeignKey(m => m.UserId);
            modelBuilder.Entity<Department>().HasData(
                new Department
                {
                    Id = 1,
                    Type = "Financial"
                }
            );
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    UserName = "Alice",
                    Email = "alice@example.com",
                    Password = SecureHasher.Hash("pass"),
                    DepartmentId = 1,
                },
                new User
                {
                    Id = 2,
                    UserName = "Bob",
                    Email = "bob@example.com",
                    Password = SecureHasher.Hash("pass"),
                    DepartmentId = 1,
                }
            );
        }


    }
}
