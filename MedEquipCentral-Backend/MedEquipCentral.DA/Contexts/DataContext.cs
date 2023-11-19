﻿using Microsoft.EntityFrameworkCore;
using MedEquipCentral.DA.Contracts.Model;

namespace MedEquipCentral.DA.Contexts
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<Company>? Company { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Location> Locations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>()
                .HasOne(c => c.Location)
                .WithMany()
                .HasForeignKey(c => c.LocationId);
        }
    }
}