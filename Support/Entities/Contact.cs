using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Support.Entities
{
    public class Contact
    {
        [Key]
        public int ContactID { get; set; }

        [Required] 
        public int UserID { get; set; }

        [Required]
        [RegularExpression(@"^\+(?:[0-9]●?){6,14}[0-9]$", ErrorMessage = "Cell phone number format error.")]
        public string ContactPhone { get; set; }
        
        [Required]
        public string ContactName { get; set; }
        
        [Required]
        public bool Blocked { get; set; }
        
        [Required]
        public bool SeeStatus { get; set; }

        public string? Wallpaper { get; set; }

    }
}
