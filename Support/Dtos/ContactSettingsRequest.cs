using System.ComponentModel.DataAnnotations;

namespace Support.Dtos
{
    public class ContactSettingsRequest
    {
        [Required]
        public Guid ContactID { get; set; }

        public bool? Blocked { get; set; }

        public bool? SeeStatus { get; set; }

        [RegularExpression(@"data:image/(?<type>.+?);base64,(?<data>.+)")]
        public string? URIWallpaper { get; set; }

        public string? Extension { get; set; }

        public bool? removeCurrentWallpaper { get; set; }
    }
}
