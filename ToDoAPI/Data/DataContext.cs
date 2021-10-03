using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoAPI.Data.Entities;

namespace ToDoAPI.Data
{
    public class DataContext : IdentityDbContext<IdentityUser>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }
		public DbSet<ToDoTask> ToDoTasks { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ToDoTask>()
                .HasKey(x=>x.Id);
            modelBuilder.Entity<ToDoTask>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<ToDoTask>()
                .HasOne(x => x.User)
                .WithMany()
               .HasForeignKey(x=>x.UserId);
            modelBuilder.Entity<ToDoTask>()
                .Property(x => x.UserId)
                .IsRequired();
            modelBuilder.Entity<ToDoTask>()
                .Property(x => x.Name).IsRequired();
            modelBuilder.Entity<ToDoTask>()
                .Property(x => x.IsCompleted).IsRequired();
        }
    }
}
