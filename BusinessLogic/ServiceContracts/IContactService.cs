using Support.Dtos;
using Support.Entities;

namespace BusinessLogic.ServiceContracts
{
    public interface IContactService
    {
        Task<IEnumerable<Contact>> GetContactsByUserId(string userId);

        Task<Contact?> GetContactById(Guid contactId);

        Task<IEnumerable<Contact>> SynchronizeContacts(IEnumerable<PhoneContact> phoneContacts, string userId);
        
        Task<ContactSettingsResponse?> SetSettings(ContactSettingsRequest contactSettings);

    }
}
