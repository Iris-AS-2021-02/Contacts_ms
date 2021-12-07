using Support.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Support.Dtos
{
    public class GraphQLResponse<T> where T : class
    {
        public T Data { get; set; }
    }

    public class UsersWithNumberGraphQlResponse
    {
        public ActiveUserList UsersWithNumber { get; set; }
    }

    public class ContactsByUserIdGraphQlResponse
    {
        public List<Contact> ContactsByUserId { get; set; }
    }
}
