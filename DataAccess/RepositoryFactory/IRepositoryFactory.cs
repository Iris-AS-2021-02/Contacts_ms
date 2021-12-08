using DataAccess.RepositoryContracts;

namespace DataAccess.RepositoryFactory
{
    public interface IRepositoryFactory
    {
        IContactRepository Contacts { get; }
        void Dispose();
        int Commit();
    }
}
