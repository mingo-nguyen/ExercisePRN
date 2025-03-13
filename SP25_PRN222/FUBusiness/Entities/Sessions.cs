using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUBusiness.Entities
{
    public class Sessions
    {
        [Key, MaxLength(50)]
        public string SessionId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required, MaxLength(20)]
        public string Role { get; set; }
        public DateTime ExpiresAt { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
