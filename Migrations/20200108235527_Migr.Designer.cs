using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using humanlab;

namespace humanlab.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20200108235527_Migr")]
    partial class Migr
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.3");

            modelBuilder.Entity("humanlab.Models.Activity", b =>
                {
                    b.Property<int>("ActivityId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ActivityName")
                        .IsRequired()
                        .HasMaxLength(40);

                    b.Property<int>("FixingTime");

                    b.HasKey("ActivityId");

                    b.ToTable("Activities");
                });

            modelBuilder.Entity("humanlab.Models.ActivityGrids", b =>
                {
                    b.Property<int>("ActivityId");

                    b.Property<int>("GridId");

                    b.Property<int>("Order");

                    b.HasKey("ActivityId", "GridId");

                    b.HasIndex("ActivityId");

                    b.HasIndex("GridId");

                    b.ToTable("ActivityGrids");
                });

            modelBuilder.Entity("humanlab.Models.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasMaxLength(40);

                    b.HasKey("CategoryId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("humanlab.Models.Element", b =>
                {
                    b.Property<int>("ElementId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Audio");

                    b.Property<int?>("CategoryId");

                    b.Property<string>("ElementName")
                        .IsRequired()
                        .HasMaxLength(40);

                    b.Property<string>("Image")
                        .IsRequired();

                    b.Property<string>("SpeachText");

                    b.HasKey("ElementId");

                    b.HasIndex("CategoryId");

                    b.ToTable("Elements");
                });

            modelBuilder.Entity("humanlab.Models.Grid", b =>
                {
                    b.Property<int>("GridId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ElementsSize");

                    b.Property<string>("GridName")
                        .IsRequired()
                        .HasMaxLength(40);

                    b.HasKey("GridId");

                    b.ToTable("Grids");
                });

            modelBuilder.Entity("humanlab.Models.GridElements", b =>
                {
                    b.Property<int>("GridId");

                    b.Property<int>("ElementId");

                    b.Property<int>("Xposition");

                    b.Property<int>("Yposition");

                    b.HasKey("GridId", "ElementId");

                    b.HasIndex("ElementId");

                    b.HasIndex("GridId");

                    b.ToTable("GridElements");
                });

            modelBuilder.Entity("humanlab.Models.ActivityGrids", b =>
                {
                    b.HasOne("humanlab.Models.Activity", "Activity")
                        .WithMany()
                        .HasForeignKey("ActivityId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("humanlab.Models.Grid", "Grid")
                        .WithMany()
                        .HasForeignKey("GridId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("humanlab.Models.Element", b =>
                {
                    b.HasOne("humanlab.Models.Category", "Category")
                        .WithMany("Elements")
                        .HasForeignKey("CategoryId");
                });

            modelBuilder.Entity("humanlab.Models.GridElements", b =>
                {
                    b.HasOne("humanlab.Models.Element", "Element")
                        .WithMany()
                        .HasForeignKey("ElementId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("humanlab.Models.Grid", "Grid")
                        .WithMany()
                        .HasForeignKey("GridId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
