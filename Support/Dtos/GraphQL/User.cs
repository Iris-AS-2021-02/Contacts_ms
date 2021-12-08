using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Support.Dtos.GraphQl
{
    public class User
    {
        [JsonProperty("ID")]
        public string UserID { get; set; }

        [JsonProperty("Number")]
        [RegularExpression(@"^\+(?:[0-9]●?){6,14}[0-9]$", ErrorMessage = "Cell phone number format error.")]
        public string Phone { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }
    }
}
