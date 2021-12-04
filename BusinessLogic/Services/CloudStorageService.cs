using BusinessLogic.ServiceContracts;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Support.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class CloudStorageService : ICloudStorageService
    {
        private readonly IConfiguration _configuration;
        private CloudStorage? cloudStorage;

        public CloudStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task DeleteStorageObject(string urlStorageObject)
        {
            var storageObject = await GetStorageObject(urlStorageObject);

            if (storageObject != null)
            {
                using (var client = new HttpClient())
                {
                    cloudStorage = GetCloudStorageConfiguration();

                    client.BaseAddress = new Uri(cloudStorage.BaseAddress);
                    var storageObjectName = System.Web.HttpUtility.UrlEncode(storageObject.Name);
                    var result = await client.DeleteAsync($"storage/v1/b/{storageObject.Bucket}/o/{storageObjectName}");
                }

            }
        }

        public CloudStorage GetCloudStorageConfiguration()
        {

            if (cloudStorage is null)
            {
                var settings = _configuration.GetSection("CloudStorage");

                cloudStorage = new CloudStorage()
                {
                    BaseAddress = settings.GetSection("BaseAddress").Value,
                    Bucket = settings.GetSection("Bucket").Value,
                    Folder = settings.GetSection("Folder").Value,
                };
            }

            return cloudStorage;
        }

        public async Task<StorageObject?> GetStorageObject(string urlStorageObject)
        {
            using (var client = new HttpClient())
            {
                var result = await client.GetAsync(urlStorageObject);
                if (result.IsSuccessStatusCode)
                {
                    var content = await result.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<StorageObject>(content);
                    return response;
                }
                else
                    return null;
            }
        }

        public async Task<StorageObjectList?> GetStorageObjectList(int userID)
        {
            using (var client = new HttpClient())
            {
                cloudStorage = GetCloudStorageConfiguration();

                client.BaseAddress = new Uri(cloudStorage.BaseAddress);

                var result = await client.GetAsync($"storage/v1/b/{cloudStorage.Bucket}/o?prefix={cloudStorage.Folder}/{userID}");
                if (result.IsSuccessStatusCode)
                {
                    var content = await result.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<StorageObjectList>(content);
                    return response;
                }
                else
                    return null;
            }
        }

        public async Task<StorageObject?> UploadStorageObject(StorageModel storageModel)
        {
            using (var client = new HttpClient())
            {
                cloudStorage = GetCloudStorageConfiguration();

                client.BaseAddress = new Uri(cloudStorage.BaseAddress);

                var objectName = DateTime.Now.ToString($"yyyyMMddhhmmss");
                var bytesContent = new ByteArrayContent(storageModel.Bytes);
                bytesContent.Headers.Add("Content-Type", storageModel.MediaType);

                var result = await client.PostAsync($"upload/storage/v1/b/{cloudStorage.Bucket}/o?uploadType=media&name={cloudStorage.Folder}/{storageModel.SubFolder}/{storageModel.ObjectId}_{objectName}.{storageModel.Extension}", bytesContent);

                if (!result.IsSuccessStatusCode)
                    return null;

                var content = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<StorageObject>(content);
                return response;
            }
        }
    }
}
