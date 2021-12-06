using Support.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
