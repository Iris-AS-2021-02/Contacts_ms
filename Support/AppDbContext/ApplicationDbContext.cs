using Support.Entities;
using Microsoft.EntityFrameworkCore;

namespace Support.AppDbContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Contact> Contacts { get; set; }
    }
}
