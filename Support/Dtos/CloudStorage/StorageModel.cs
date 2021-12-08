namespace Support.Dtos.CloudStorage
{
    public class StorageModel
    {
        public string SubFolder { get; set; }

        public Guid ObjectId { get; set; }

        public byte[] Bytes { get; set; }

        public string MediaType { get; set; }

        public string Extension { get; set; }
    }
}
