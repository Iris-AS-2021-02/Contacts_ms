using Support.Entities;

namespace DataAccess.RepositoryContracts
{
    public interface IContactRepository : IRepository<Contact>
    {
        void Update(Contact contact);
    }
}
