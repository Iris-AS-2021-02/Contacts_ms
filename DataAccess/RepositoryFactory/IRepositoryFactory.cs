using DataAccess.RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.RepositoryFactory
{
    public interface IRepositoryFactory
    {
        IContactRepository Contacts { get; }
        void Dispose();
        int Commit();
    }
}
