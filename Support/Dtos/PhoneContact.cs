using System.ComponentModel.DataAnnotations;

namespace Support.Dtos
{
    public class PhoneContact
    {
        [Required]
        [RegularExpression(@"^\+(?:[0-9]●?){6,14}[0-9]$", ErrorMessage = "Cell phone number format error.")]
        public string ContactPhone { get; set; }

        [Required]
        public string ContactName { get; set; }
    }
}
