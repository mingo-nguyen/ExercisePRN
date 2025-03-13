using FUBusiness.Entities;
using System.ComponentModel.DataAnnotations;

namespace SP25_PRN222.WebApp.Models.CourseModels
{
    public class CourseViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(100)]
        public string Category { get; set; }

        [Required(ErrorMessage = "Course code is required")]
        [MaxLength(20)]
        public string CourseCode { get; set; }

        [Required(ErrorMessage = "Capacity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Capacity must be greater than or equal to 0")]
        public int Capacity { get; set; }
    }
    public class CourseEnrollmentViewModel
    {
        public Course Course { get; set; }
        public bool IsEnrolled { get; set; }
        public bool HasAvailableCapacity { get; set; }
    }

    public class CourseDetailsViewModel
    {
        public Course Course { get; set; }
        public bool IsEnrolled { get; set; }
        public int EnrollmentId { get; set; }
    }

    public class CourseDetailsAdminViewModel
    {
        public Course Course { get; set; }
        public IEnumerable<EnrollmentRecords> Enrollments { get; set; }
        public int ActiveEnrollments { get; set; }
        public int DroppedEnrollments { get; set; }
    }

}
