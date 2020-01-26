using humanlab.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace humanlab
{
    public class ApplicationDbContext : DbContext
    {

        public virtual DbSet<Element> Elements { get; set; }
        public virtual DbSet<Grid> Grids { get; set; }
        public virtual DbSet<Activity> Activities { get; set; }
        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<GridElements> GridElements { get; set; }

        public virtual DbSet<ActivityGrids> ActivityGrids { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Humanlab.db");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GridElements>()
                .HasKey(t => new { t.GridId, t.ElementId });

            modelBuilder.Entity<GridElements>()
           .HasOne(g => g.Grid)
           .WithMany(g => g.GridElements)
           .HasForeignKey(g => g.GridId);

            modelBuilder.Entity<GridElements>()
           .HasOne(e => e.Element)
           .WithMany(e => e.GridElements)
           .HasForeignKey(e => e.ElementId);

            modelBuilder.Entity<ActivityGrids>()
                .HasKey(t => new { t.ActivityId, t.GridId });

            modelBuilder.Entity<ActivityGrids>()
            .HasOne(a => a.Activity)
            .WithMany(a => a.ActivityGrids)
            .HasForeignKey(a => a.ActivityId);

            modelBuilder.Entity<ActivityGrids>()
           .HasOne(g => g.Grid)
           .WithMany(g => g.ActivityGrids)
           .HasForeignKey(g => g.GridId);

            modelBuilder.Entity<Element>()
            .Property(e => e.ElementId)
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<Grid>()
            .Property(g => g.GridId)
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<Activity>()
            .Property(a => a.ActivityId)
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<Category>()
            .Property(c => c.CategoryId)
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<Element>()
                .HasOne(p => p.Category)
                .WithMany(b => b.Elements)
                .HasForeignKey(e => e.CategoryId);

            modelBuilder.Entity<Category>()
                .HasMany(c => c.Elements)
                .WithOne(e => e.Category);
        }

    }
}
