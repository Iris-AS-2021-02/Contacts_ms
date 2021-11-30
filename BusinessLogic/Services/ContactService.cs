using BusinessLogic.ServiceContracts;
using DataAccess.RepositoryFactory;
using Support.Dtos;
using Support.Entities;

namespace BusinessLogic.Services
{
    public class ContactService : IContactService
    {
        private readonly IRepositoryFactory _repository;
        public ContactService(IRepositoryFactory repository)
        {
            _repository = repository;
        }

        public IEnumerable<Contact> GetContactsByUserId(int userId)
        {
            var contacts = _repository.Contacts.GetAll(x => x.UserID == userId);
            return contacts;
        }

        public Contact? GetContactById(int contactId)
        {
            var contact = _repository.Contacts.GetById(contactId);
            return contact;
        }

        public IEnumerable<Contact> SynchronizeContacts(IEnumerable<PhoneContact> phoneContacts, int userId)
        {
            //simulación usuarios activos
            List<User> users = new List<User>() {
                new User(){ UserID = 1, Phone = "+573144218013", Name = "Deicy Páez"},
                new User(){ UserID = 2, Phone = "+573168280345", Name = "Francis Moscoso"},
                new User(){ UserID = 3, Phone = "+573057174334", Name = "Harold Bartolo"},
                new User(){ UserID = 4, Phone = "+573132093073", Name = "Harold Bartolo"},
                new User(){ UserID = 5, Phone = "+573004062480", Name = "Andres Miramag"},
                new User(){ UserID = 6, Phone = "+573214067679", Name = "Andres Aldana"},
            };


            //simulación petición a users_ms por cada usuario de telefono;
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

            var currentContacts = GetContactsByUserId(userId).ToList();

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

            return GetContactsByUserId(userId);
        }

        public bool SetSettings(ContactSettings contactSettings)
        {
            var dbContact = _repository.Contacts.GetById(contactSettings.ContactID);
            if (dbContact is null)
                return false;
            
            dbContact.Blocked = contactSettings.Blocked ?? dbContact.Blocked;
            dbContact.SeeStatus = contactSettings.SeeStatus ?? dbContact.SeeStatus;
            dbContact.Wallpaper = contactSettings.Wallpaper ?? dbContact.Wallpaper;

            _repository.Contacts.Update(dbContact);
            _repository.Commit();
            return true;

        }
    }
}
