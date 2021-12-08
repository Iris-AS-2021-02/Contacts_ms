namespace Support.Dtos.CloudStorage
{
    public class StorageObject
    {
        public string Kind { get; set; }
        
        public string Id { get; set; }
        
        public string SelfLink { get; set; }
        
        public string MediaLink { get; set; }
        
        public string Name { get; set; }
        
        public string Bucket { get; set; }
        
        public string Generation { get; set; }
        
        public string MetaGeneration { get; set; }
        
        public string ContentType { get; set; }
        
        public string StorageClass { get; set; }
        
        public string Size { get; set; }
        
        public string Md5Hash { get; set; }
        
        public string Crc32c { get; set; }
        
        public string Etag { get; set; }
        
        public string TimeCreated { get; set; }
        
        public string Updated { get; set; }

        public string TimeStorageClassUpdated { get; set; }
    }
}
