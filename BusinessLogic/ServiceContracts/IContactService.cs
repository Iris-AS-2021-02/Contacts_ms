using Support.Dtos;
using Support.Entities;

namespace BusinessLogic.ServiceContracts
{
    public interface IContactService
    {
        IEnumerable<Contact> GetContactsByUserId(int userId);
        
        Contact? GetContactById(int contactId);
        
        IEnumerable<Contact> SynchronizeContacts(IEnumerable<PhoneContact> phoneContacts, int userId);
        
        bool SetSettings(ContactSettings contactSettings);

    }
}
