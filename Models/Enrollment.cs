using CourseShopOnline.Models.Enum;

namespace CourseShopOnline.Models;

public class Enrollment
{
    public int EnrollmentId { get; set; }

    public int CourseId { get; set; }
    public Course Course { get; set; }

    public string StudentId { get; set; }
    public User Student { get; set; }

    public DateTime EnrollmentDate { get; set; }
    public LearningStatus Status { get; set; }
    public EnollingStatus EnollingStatus { get; set; } 
}