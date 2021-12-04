using BusinessLogic.ServiceContracts;
using DataAccess.RepositoryFactory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Support.Dtos;
using Support.Entities;
using System.Text.RegularExpressions;

namespace BusinessLogic.Services
{
    public class ContactService : IContactService
    {
        private readonly IRepositoryFactory _repository;
        private readonly ICloudStorageService _cloudStorageService;
        private CloudStorage? cloudStorage;
        

        public ContactService(IRepositoryFactory repository, ICloudStorageService cloudStorageService)
        {
            _repository = repository;
            _cloudStorageService = cloudStorageService;
        }

        public async Task<IEnumerable<Contact>> GetContactsByUserId(int userId)
        {
            var contacts = _repository.Contacts.GetAll(x => x.UserID == userId);
            var wallpapers = await _cloudStorageService.GetStorageObjectList(userId);
            var cloudStorage = _cloudStorageService.GetCloudStorageConfiguration();

            foreach (var wallpaper in wallpapers.Items)
            {
                var pattern = @$"{cloudStorage.Folder}/{userId}/(?<contactId>.+?)_(?<name>.+)";
                var contactId = Regex.Match(wallpaper.Name, pattern).Groups["contactId"].Value;

                var contact = contacts.FirstOrDefault(x => x.ContactID == Int32.Parse(contactId));

                if (contact != null)
                    contact.Wallpaper = wallpaper.MediaLink;

            }

            return contacts;
        }

        public async Task<Contact?> GetContactById(int contactId)
        {
            var contact = _repository.Contacts.GetById(contactId);

            if (contact.Wallpaper != null)
            {
                var wallpaper = await _cloudStorageService.GetStorageObject(contact.Wallpaper);
                contact.Wallpaper = wallpaper.MediaLink;
            }
            return contact;
        }

        public async Task<IEnumerable<Contact>> SynchronizeContacts(IEnumerable<PhoneContact> phoneContacts, int userId)
        {
            //Start TODO: Hacer petición al Gateway

            //simulación usuarios activos
            List<User> users = new List<User>() {
                new User(){ UserID = 1, Phone = "+573144218013", Name = "Deicy Páez"},
                new User(){ UserID = 2, Phone = "+573168280345", Name = "Francis Moscoso"},
                new User(){ UserID = 3, Phone = "+573057174334", Name = "Harold Bartolo"},
                new User(){ UserID = 4, Phone = "+573132093073", Name = "Harold Bartolo"},
                new User(){ UserID = 5, Phone = "+573004062480", Name = "Andres Miramag"},
                new User(){ UserID = 6, Phone = "+573214067679", Name = "Andres Aldana"},
            };

            //verificar que el usuario existe
            var user = users.FirstOrDefault(x => x.UserID == userId);
            if (user is null)
                return new List<Contact>();


            //simulación petición a users_ms por cada contacto de telefono;
            //se puede solicitar enviar una lista y que devuelva una lista

            List<User> activeUsers = new List<User>();
            foreach (var phoneContact in phoneContacts)
            {
                var activeUser = users.FirstOrDefault(user => user.Phone.Equals(phoneContact.ContactPhone));
                if (activeUser != null)
                {
                    activeUser.Name = phoneContact.ContactName;
                    activeUsers.Add(activeUser);
                }
            }

            //End TODO: Hacer petición al Gateway

            var currentContacts = await GetContactsByUserId(userId);

            var newContacts = from au in activeUsers join cc in currentContacts on au.Phone equals cc.ContactPhone into UsersContacts
                              from uc in UsersContacts.DefaultIfEmpty() where uc is null
                              select new Contact 
                              { 
                                  UserID = userId,
                                  ContactPhone = au.Phone,
                                  ContactName = au.Name,
                                  Blocked = false,
                                  SeeStatus = true,
                                  Wallpaper = null
                              };

            var contacts = from au in activeUsers join cc in currentContacts on au.Phone equals cc.ContactPhone
                           select new Contact
                           {
                               ContactID = cc.ContactID,
                               UserID = cc.UserID,
                               ContactPhone = cc.ContactPhone,
                               ContactName = au.Name,
                               Blocked = cc.Blocked,
                               SeeStatus = cc.SeeStatus,
                               Wallpaper = cc.Wallpaper
                           };


            var deletedContacts = from cc in currentContacts join au in activeUsers on cc.ContactPhone equals au.Phone into ContactsUsers
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
                _repository.Contacts.Update(contact);
            }

            foreach (var deletedContact in deletedContacts)
            {
                _repository.Contacts.DeleteById(deletedContact.ContactID);
                _repository.Commit();
            }

            return await GetContactsByUserId(userId);
        }

        public async Task<bool> SetSettings(ContactSettings contactSettings)
        {
            var dbContact = _repository.Contacts.GetById(contactSettings.ContactID);
            if (dbContact is null)
                return false;
            
            dbContact.Blocked = contactSettings.Blocked ?? dbContact.Blocked;
            dbContact.SeeStatus = contactSettings.SeeStatus ?? dbContact.SeeStatus;

            if(contactSettings.URIWallpaper != null)
            {
                if (contactSettings.Extension is null)
                    return false;

                try
                {
                    cloudStorage = _cloudStorageService.GetCloudStorageConfiguration();

                    var pattern = @"data:(?<mediatype>.+?);base64,(?<data>.+)";
                    var data = Regex.Match(contactSettings.URIWallpaper, pattern).Groups["data"].Value;
                    var mediaType = Regex.Match(contactSettings.URIWallpaper, pattern).Groups["mediatype"].Value;

                    if (!mediaType.Contains("image"))
                        return false;

                    var bytes = Convert.FromBase64String(data);

                    var wallpaper = new StorageModel { SubFolder = dbContact.UserID, ObjectId = contactSettings.ContactID, Bytes = bytes, MediaType = mediaType, Extension = contactSettings.Extension };
                    var storageObject = await _cloudStorageService.UploadStorageObject(wallpaper);
                    
                    if (storageObject is null)
                        return false;

                    if (dbContact.Wallpaper != null)
                        await _cloudStorageService.DeleteStorageObject(dbContact.Wallpaper).ConfigureAwait(false);

                    dbContact.Wallpaper = storageObject.SelfLink;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            _repository.Contacts.Update(dbContact);
            _repository.Commit();
            return true;
        }
    }
}
