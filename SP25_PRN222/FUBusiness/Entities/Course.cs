using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FUBusiness.Entities
{
    public class Course
    {
        public Course()
        {
            Enrollments = new HashSet<EnrollmentRecords>();
        }


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(100)]
        public string Category { get; set; }

        [Required, MaxLength(20)]
        public string CourseCode { get; set; }

        [Range(0, int.MaxValue)]
        public int Capacity { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<EnrollmentRecords> Enrollments { get; set; }
    }
}
