using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Data
{
    public partial class AsiBasecodeDBContext : DbContext
    {
        public AsiBasecodeDBContext(DbContextOptions<AsiBasecodeDBContext> options)
            : base(options)
        {
        }

      
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Expense> Expenses { get; set; }

        public virtual DbSet<Category> Categories { get; set; }
    

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User Entity Configuration
            modelBuilder.Entity<User>(entity =>
            {
                // Configure primary key
                entity.HasKey(e => e.Id);

                // Ensure UserName is unique
                entity.HasIndex(e => e.UserName, "UQ__Users__UserName")
                      .IsUnique();

                // Configure fields
                entity.Property(e => e.UserName)
                      .IsRequired()
                      .HasMaxLength(50) // Ensure this matches Category.UserName
                      .IsUnicode(false);

                entity.Property(e => e.FirstName)
                      .IsRequired()
                      .HasMaxLength(50)
                      .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

                entity.Property(e => e.Password)
                      .IsRequired()
                      .HasMaxLength(50)
                      .IsUnicode(false);

                entity.Property(e => e.ConfirmPassword)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProfilePictureUrl)
                .HasMaxLength(255) // Adjust the length based on your file paths
                .IsUnicode(false)  // If it's a URL or file path, Unicode is not necessary
                .HasDefaultValue(null); // O


                entity.Property(e => e.CreatedBy)
                      .HasMaxLength(50)
                      .IsUnicode(false);

                entity.Property(e => e.CreatedTime)
                      .HasColumnType("datetime");

                entity.Property(e => e.UpdatedBy)
                      .HasMaxLength(50)
                      .IsUnicode(false);

                entity.Property(e => e.UpdatedTime)
                      .HasColumnType("datetime");




                // Configure relationship between User and Expenses
                entity.HasMany(u => u.Expenses)
                      .WithOne(e => e.User)
                      .HasForeignKey(e => e.UserName)
                      .HasPrincipalKey(u => u.UserName)
                      .OnDelete(DeleteBehavior.Cascade);


            });

            // Expense Entity Configuration
            modelBuilder.Entity<Expense>(entity =>
            {
                // Configure primary key
                entity.HasKey(e => e.ExpenseId);

                // Configure fields
                entity.Property(e => e.ExpenseId)
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.Title)
                      .IsRequired()
                      .HasMaxLength(100)
                      .IsUnicode(false);

                entity.Property(e => e.Category)
                      .IsRequired()
                      .HasMaxLength(50)
                      .IsUnicode(false);

                entity.Property(e => e.Amount)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(e => e.Date)
                      .HasColumnType("datetime")
                      .IsRequired();

                entity.Property(e => e.UserName)
                      .IsRequired()
                      .HasMaxLength(50) // Ensure this matches User.UserName
                      .IsUnicode(false);

                // Configure relationship between Expense and User
                entity.HasOne(e => e.User)
                      .WithMany(u => u.Expenses)
                      .HasForeignKey(e => e.UserName)
                      .HasPrincipalKey(u => u.UserName)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Category Entity Configuration
            modelBuilder.Entity<Category>(entity =>
            {


                entity.HasKey(e => e.CategoryId);

                entity.Property(e => e.CategoryName)
                      .IsRequired()
                      .HasMaxLength(100)
                      .IsUnicode(false);

                entity.Property(e => e.DateCreated)
                      .HasColumnType("datetime")
                      .IsRequired();

                // Configure UserName as a foreign key referencing the User table
                entity.Property(e => e.UserName)
                      .IsRequired()
                      .HasMaxLength(50) // Ensure this matches User.UserName
                      .IsUnicode(false);

                entity.HasOne(e => e.User)
                      .WithMany(u => u.Categories)
                      .HasForeignKey(e => e.UserName)
                      .HasPrincipalKey(u => u.UserName)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Call any additional configurations
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
