using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        //DbSet przyjmuje AppUser jako parametr dla którego ma zrobić tabelę 
        public DbSet<AppUser> Users { get; set; }
    }
}