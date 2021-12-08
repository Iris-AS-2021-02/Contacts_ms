using BusinessLogic.ServiceContracts;
using DataAccess.RepositoryFactory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Support.Dtos;
using Support.Dtos.CloudStorage;
using Support.Dtos.GraphQl;
using Support.Entities;
using System.Text;
using System.Text.RegularExpressions;

namespace BusinessLogic.Services
{
    public class ContactService : IContactService
    {
        private readonly IConfiguration _configuration;
        private readonly IRepositoryFactory _repository;
        private readonly ICloudStorageService _cloudStorageService;
        private CloudStorage? cloudStorage;
        

        public ContactService(IConfiguration configuration, IRepositoryFactory repository, ICloudStorageService cloudStorageService)
        {
            _configuration = configuration;
            _repository = repository;
            _cloudStorageService = cloudStorageService;
        }

        public async Task<IEnumerable<Contact>> GetContactsByUserId(string userId)
        {
            var contacts = _repository.Contacts.GetAll(x => x.UserID.Equals(userId));
            var wallpapers = await _cloudStorageService.GetStorageObjectList(userId);
            cloudStorage = _cloudStorageService.GetCloudStorageConfiguration();

            if (wallpapers?.Items != null)
            {
                foreach (var wallpaper in wallpapers.Items)
                {
                    var pattern = @$"{cloudStorage.Folder}/{userId}/(?<contactId>.+?)_(?<name>.+)";
                    var contactId = Regex.Match(wallpaper.Name, pattern).Groups["contactId"].Value;

                    var contact = contacts.FirstOrDefault(x => x.ContactID.Equals(Guid.Parse(contactId)));

                    if (contact != null)
                        contact.Wallpaper = wallpaper.MediaLink;

                }
            }

            return contacts;
        }

        public async Task<Contact?> GetContactById(Guid contactId)
        {
            var contact = _repository.Contacts.GetById(contactId);

            if (contact?.Wallpaper != null)
            {
                var wallpaper = await _cloudStorageService.GetStorageObject(contact.Wallpaper);
                contact.Wallpaper = wallpaper?.MediaLink;
            }
            return contact;
        }

        public async Task<IEnumerable<Contact>> SynchronizeContacts(IEnumerable<PhoneContact> phoneContacts, string userId)
        {
            //Start TODO: Hacer petición al Gateway
            var APIGatewayURI = _configuration.GetSection("APIGatewayURI").Value;

            //TODO: solicitar metodo para verificar que el usuario existe
            //User user = null;
            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri(APIGatewayURI);
            //    var result = await client.GetAsync($"user/find/{userId}");

            //    if (result.IsSuccessStatusCode)
            //    {
            //        var content = await result.Content.ReadAsStringAsync();
            //        user = JsonConvert.DeserializeObject<User>(content);
            //    }
            //}
            //if (user is null)
            //    throw new KeyNotFoundException("User does not exist");


            var numbersList = from phoneContact in phoneContacts select phoneContact.ContactPhone;
            string numbers = string.Join(",", numbersList);

            var queryObject = new
            {
                query = @"query {
                    usersWithNumber (number: """ + numbers + @"""){
                        ID
                        Name
                        Number
                    }
                }",
                variables = new { }
            };


            List<User>? activeUsers = new List<User>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(APIGatewayURI);
                var result = await client.PostAsync("graphql", new StringContent(JsonConvert.SerializeObject(queryObject), Encoding.UTF8, "application/json"));

                if (result.IsSuccessStatusCode)
                {
                    var content = await result.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<GraphQLResponse<UsersWithNumberQueryResponse>>(content);
                    activeUsers = response?.Data.UsersWithNumber;
                }
            }

            var activeContacts = (
                from activeUser in activeUsers
                join phoneContact in phoneContacts
                on activeUser.Phone equals phoneContact.ContactPhone
                select new User
                {
                    UserID = activeUser.UserID,
                    Name = phoneContact.ContactName,
                    Phone = phoneContact.ContactPhone
                }).ToList();

            var currentContacts = _repository.Contacts.GetAll(x => x.UserID.Equals(userId));

            var newContacts = from ac in activeContacts join cc in currentContacts on ac.Phone equals cc.ContactPhone into UsersContacts
                              from uc in UsersContacts.DefaultIfEmpty() where uc is null
                              select new Contact 
                              { 
                                  UserID = userId,
                                  ContactPhone = ac.Phone,
                                  ContactName = ac.Name,
                                  Blocked = false,
                                  SeeStatus = true,
                                  Wallpaper = null
                              };

            var contacts = from ac in activeContacts join cc in currentContacts on ac.Phone equals cc.ContactPhone
                           select new Contact
                           {
                               ContactID = cc.ContactID,
                               UserID = cc.UserID,
                               ContactPhone = cc.ContactPhone,
                               ContactName = ac.Name,
                               Blocked = cc.Blocked,
                               SeeStatus = cc.SeeStatus,
                               Wallpaper = cc.Wallpaper
                           };


            var deletedContacts = from cc in currentContacts join ac in activeContacts on cc.ContactPhone equals ac.Phone into ContactsUsers
                                  from cu in ContactsUsers.DefaultIfEmpty() where cu is null
                                  select cc;


            //TODO: pasar ciclo a query en base de datos
            foreach (var newContact in newContacts)
            {
                _repository.Contacts.Insert(newContact);
                _repository.Commit();
            }

            foreach (var contact in contacts)
            {
                var dbContact = _repository.Contacts.GetById(contact.ContactID);
                contact.Wallpaper = dbContact.Wallpaper;
                _repository.Contacts.Update(contact);
            }

            foreach (var deletedContact in deletedContacts)
            {
                _repository.Contacts.DeleteById(deletedContact.ContactID);
                _repository.Commit();
            }

            return await GetContactsByUserId(userId);
        }

        public async Task<ContactSettingsResponse?> SetSettings(ContactSettingsRequest contactSettings)
        {
            var dbContact = _repository.Contacts.GetById(contactSettings.ContactID);
            if (dbContact is null)
                throw new KeyNotFoundException("Contact does not exist");

            dbContact.Blocked = contactSettings.Blocked ?? dbContact.Blocked;
            dbContact.SeeStatus = contactSettings.SeeStatus ?? dbContact.SeeStatus;
            
            if (contactSettings.removeCurrentWallpaper is true && contactSettings.URIWallpaper is null && dbContact.Wallpaper != null)
            {
                await _cloudStorageService.DeleteStorageObject(dbContact.Wallpaper);
                dbContact.Wallpaper = null;
            }

            if (contactSettings.URIWallpaper != null)
            {
                if (contactSettings.Extension is null)
                    throw new ArgumentNullException(nameof(contactSettings.Extension));

                cloudStorage = _cloudStorageService.GetCloudStorageConfiguration();

                var pattern = @"data:(?<mediatype>.+?);base64,(?<data>.+)";
                var data = Regex.Match(contactSettings.URIWallpaper, pattern).Groups["data"].Value;
                var mediaType = Regex.Match(contactSettings.URIWallpaper, pattern).Groups["mediatype"].Value;

                var bytes = Convert.FromBase64String(data);

                var wallpaper = new StorageModel { SubFolder = dbContact.UserID, ObjectId = contactSettings.ContactID, Bytes = bytes, MediaType = mediaType, Extension = contactSettings.Extension };
                var storageObject = await _cloudStorageService.UploadStorageObject(wallpaper);
                    
                if (storageObject is null)
                    throw new Exception("Error storing the image");

                if (dbContact.Wallpaper != null)
                    await _cloudStorageService.DeleteStorageObject(dbContact.Wallpaper).ConfigureAwait(false);

                dbContact.Wallpaper = storageObject.SelfLink;
                
            }
            _repository.Contacts.Update(dbContact);
            _repository.Commit();

            var contact = await GetContactById(dbContact.ContactID);
            var jsonContact = JsonConvert.SerializeObject(contact);
            var response = JsonConvert.DeserializeObject<ContactSettingsResponse>(jsonContact);

            return response;
        }
    }
}
