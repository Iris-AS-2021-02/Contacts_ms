using DataAccess.Repositories;
using DataAccess.RepositoryContracts;
using Support.AppDbContext;

namespace DataAccess.RepositoryFactory
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly ApplicationDbContext _db;
        private bool disposed = false;
        private IContactRepository contacts;

        public RepositoryFactory(ApplicationDbContext db)
        {
            _db = db;
        }
        public IContactRepository Contacts
        {
            get {
                if (contacts is null)
                    contacts = new ContactRepository(_db);
                return contacts;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                if (disposing)
                    _db.Dispose();
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public int Commit()
        {
            return _db.SaveChanges();
        }
    }
}
