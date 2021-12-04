using Support.Dtos;
using Support.Entities;

namespace BusinessLogic.ServiceContracts
{
    public interface IContactService
    {
        Task<IEnumerable<Contact>> GetContactsByUserId(int userId);

        Task<Contact?> GetContactById(int contactId);

        Task<IEnumerable<Contact>> SynchronizeContacts(IEnumerable<PhoneContact> phoneContacts, int userId);
        
        Task<bool> SetSettings(ContactSettings contactSettings);

    }
}
