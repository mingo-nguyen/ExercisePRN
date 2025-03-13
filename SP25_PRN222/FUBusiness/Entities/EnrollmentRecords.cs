using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace FUBusiness.Entities
{
    public class EnrollmentRecords
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int CourseId { get; set; }

        [Required]
        public int UserId { get; set; }

        public DateTime EntrollDate { get; set; } = DateTime.Now;
        public bool Dropped { get; set; } = false;

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }

    }
}
