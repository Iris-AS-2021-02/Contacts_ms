using DataAccess.RepositoryContracts;
using Newtonsoft.Json;
using Support.AppDbContext;
using Support.Entities;

namespace DataAccess.Repositories
{
    public class ContactRepository : Repository<Contact>, IContactRepository
    {
        private readonly ApplicationDbContext _db;
        public ContactRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _db = dbContext;
        }

        public void Update(Contact contact)
        {
            var dbContact = _db.Contacts.FirstOrDefault(x => x.ContactID == contact.ContactID);
            if (dbContact is null)
                throw new Exception("Contact not found");

            var contactJson = JsonConvert.SerializeObject(contact);
            JsonConvert.PopulateObject(contactJson, dbContact);
            _db.SaveChanges();
        }
    }
}
