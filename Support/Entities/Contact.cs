using System.ComponentModel.DataAnnotations;

namespace Support.Entities
{
    public class Contact
    {
        [Key]
        public Guid ContactID { get; set; }

        [Required] 
        [StringLength(24)]
        public string UserID { get; set; }

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
