using humanlab.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace humanlab
{
   public class ApplicationDbContext: DbContext
    {

        public virtual DbSet<Element> Elements { get; set; }
        public virtual DbSet<Grid> Grids { get; set; }
        public virtual DbSet<Activity> Activities { get; set; }
        public virtual DbSet<Category> Categories { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Humanlab.db");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GridElements>()
                .HasKey(t => new { t.GridId, t.ElementId });
            modelBuilder.Entity<ActivityGrids>()
                .HasKey(t => new { t.ActivityId, t.GridId });
        }
    }
}
