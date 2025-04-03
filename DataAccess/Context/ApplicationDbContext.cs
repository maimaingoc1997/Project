using CourseShopOnline.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CourseShopOnline.DataAccess.Context;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<TransactionDetail> TransactionDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User: email unique
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Course - Teacher (1-n)
        modelBuilder.Entity<Course>()
            .HasOne(c => c.Teacher)
            .WithMany()
            .HasForeignKey(c => c.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        // Enrollment: n-n User (Student) <-> Course
        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Student)
            .WithMany()
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId);

        // Review: 1-n Course
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Course)
            .WithMany(c => c.Reviews)
            .HasForeignKey(r => r.CourseId);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.Student)
            .WithMany()
            .HasForeignKey(r => r.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        // CartItem
        modelBuilder.Entity<CartItem>()
            .HasOne(ci => ci.Student)
            .WithMany()
            .HasForeignKey(ci => ci.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CartItem>()
            .HasOne(ci => ci.Course)
            .WithMany()
            .HasForeignKey(ci => ci.CourseId);

        // Transaction - User
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Student)
            .WithMany()
            .HasForeignKey(t => t.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        // TransactionDetail
        modelBuilder.Entity<TransactionDetail>()
            .HasOne(td => td.Transaction)
            .WithMany(t => t.Details)
            .HasForeignKey(td => td.TransactionId);

        modelBuilder.Entity<TransactionDetail>()
            .HasOne(td => td.Course)
            .WithMany()
            .HasForeignKey(td => td.CourseId);
    }
}