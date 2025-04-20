using EasyUniAPI.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EasyUniAPI.DataAccess
{
    public class EasyUniDbContext(DbContextOptions<EasyUniDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
