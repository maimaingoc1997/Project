using CourseShopOnline.Models;
using CourseShopOnline.Models.Enum;
using Microsoft.AspNetCore.Identity;
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
    public static async Task SeedData(ApplicationDbContext context, UserManager<User> userManager)
    {
        if (!context.Users.Any())
        {
            var teacher = new User { 
                UserName = "teacher3", 
                FullName = "Teacher Three", 
                Email = "teacher3@example.com"
            };

            var passwordHasher = new PasswordHasher<User>();
            teacher.PasswordHash = passwordHasher.HashPassword(teacher, "Password123!");
            var result = await userManager.CreateAsync(teacher, "Password123!");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(teacher, "Teacher");  // Gán vai trò Teacher
            }
            else
            {
                throw new Exception($"Error creating user {teacher.UserName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            // Sau khi tạo giáo viên, thêm khóa học
            var courses = new List<Course>
            {
                new Course { Title = "Course 1", Description = "Description 1", Price = 100, Status = CourseStatus.Approved, TeacherId = teacher.Id },
                new Course { Title = "Course 2", Description = "Description 2", Price = 200, Status = CourseStatus.PendingApproval, TeacherId = teacher.Id },
                new Course { Title = "Course 3", Description = "Description 3", Price = 300, Status = CourseStatus.Approved, TeacherId = teacher.Id },
                new Course { Title = "Course 4", Description = "Description 4", Price = 400, Status = CourseStatus.Rejected, TeacherId = teacher.Id },
                new Course { Title = "Course 5", Description = "Description 5", Price = 500, Status = CourseStatus.PendingApproval, TeacherId = teacher.Id },
                new Course { Title = "Course 6", Description = "Description 6", Price = 600, Status = CourseStatus.Approved, TeacherId = teacher.Id },
                new Course { Title = "Course 7", Description = "Description 7", Price = 700, Status = CourseStatus.Approved, TeacherId = teacher.Id },
                new Course { Title = "Course 8", Description = "Description 8", Price = 800, Status = CourseStatus.Rejected, TeacherId = teacher.Id },
                new Course { Title = "Course 9", Description = "Description 9", Price = 900, Status = CourseStatus.PendingApproval, TeacherId = teacher.Id },
                new Course { Title = "Course 10", Description = "Description 10", Price = 100, Status = CourseStatus.Approved, TeacherId = teacher.Id }
            };

            // Thêm khóa học vào cơ sở dữ liệu
            context.Courses.AddRange(courses);
            await context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu
        }
           
    }

    
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