using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
