namespace Support.Dtos
{
    public  class ContactSettingsResponse
    {
        public Guid ContactID { get; set; }

        public bool? Blocked { get; set; }

        public bool? SeeStatus { get; set; }

        public string? Wallpaper { get; set; }
    }
}
