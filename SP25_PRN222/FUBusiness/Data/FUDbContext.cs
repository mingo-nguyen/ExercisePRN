using Microsoft.EntityFrameworkCore;
using FUBusiness.Entities;
using System;

namespace FUBusiness.Data
{
    public class FUDbContext : DbContext
    {
        public FUDbContext(DbContextOptions<FUDbContext> options): base(options) {
            
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<EnrollmentRecords> EnrollmentRecords { get; set; }
        public DbSet<Sessions> Sessions { get; set; }
        public DbSet<User> Users { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>().HasIndex(c => c.CourseCode).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Sessions>().HasIndex(s => s.SessionId).IsUnique();
            modelBuilder.Entity<EnrollmentRecords>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Enrollments)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Enrollments)
                .WithOne(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Sessions)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        }

    }
}
