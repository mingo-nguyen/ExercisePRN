
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FUBusiness.Entities
{
    public class User
    {
        public User()
        {
            Enrollments = new HashSet<EnrollmentRecords>();
            Sessions = new HashSet<Sessions>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required, MaxLength(100)]
        public string FullName { get; set; }
        [Required, MaxLength(100), EmailAddress]
        public string Email { get; set; }
        [Required, MaxLength(255)]
        public string Password { get; set; }
        [Required, MaxLength(20)]
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<EnrollmentRecords> Enrollments { get; set; }
        public virtual ICollection<Sessions> Sessions { get; set; }
    }
}
