using CourseShopOnline.DataAccess.Context;
using CourseShopOnline.Interfaces;
using CourseShopOnline.Models;
using CourseShopOnline.Models.Enum;

namespace CourseShopOnline.Services;

public class CourseService 
{
    private readonly ApplicationDbContext _context;

    public CourseService(ApplicationDbContext context)
    {
        _context = context;
    }

    

    public IEnumerable<Course> GetAllCourses()
    {
        return _context.Courses.ToList();
    }
}
