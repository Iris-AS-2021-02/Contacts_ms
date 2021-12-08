using Support.Dtos.CloudStorage;

namespace BusinessLogic.ServiceContracts
{
    public interface ICloudStorageService
    {
        Task DeleteStorageObject(string urlStorageObject);

        CloudStorage GetCloudStorageConfiguration();

        Task<StorageObject?> GetStorageObject(string urlStorageObject);

        Task<StorageObjectList?> GetStorageObjectList(string userID);
        
        Task<StorageObject?> UploadStorageObject(StorageModel storageModel);
    }
}
