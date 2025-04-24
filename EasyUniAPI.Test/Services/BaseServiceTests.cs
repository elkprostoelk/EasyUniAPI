using EasyUniAPI.DataAccess;
using EasyUniAPI.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace EasyUniAPI.Test.Services
{
    public class BaseServiceTests
    {
        protected readonly EasyUniDbContext easyUniDbContext;
        protected readonly IRepository<User, string> userRepository;
        protected readonly IRepository<Role, int> roleRepository;
        protected readonly IRepository<UserRole, long> userRoleRepository;

        public BaseServiceTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<EasyUniDbContext>()
                .UseInMemoryDatabase("EasyUniTestDb")
                .Options;

            easyUniDbContext = new EasyUniDbContext(dbContextOptions);
            easyUniDbContext.Database.EnsureCreated();

            userRepository = new Repository<User, string>(easyUniDbContext);
            roleRepository = new Repository<Role, int>(easyUniDbContext);
            userRoleRepository = new Repository<UserRole, long>(easyUniDbContext);

            SeedUsers();
        }

        private void SeedUsers()
        {
            easyUniDbContext.Users.AddRange(
                new User { Id = "01JSMN2ZYJTXZ93HNTZJF593TH", FirstName = "Admin", MiddleName = "Adminovich", LastName = "Admin", Email = "admin@admin.com", PhoneNumber = "+1234567890", BirthDate = new DateOnly(1990, 1, 1), PasswordHash = "UxgDBwC9sS+9Wl+b5MOPRdNlSzURsNBhIC5OQFVwkOQ=", PasswordSalt = "9KrQ5NYDug57LGh/A+ctgA==" } // pass: strongPa$$word123
                );

            easyUniDbContext.SaveChanges();

            easyUniDbContext.UserRoles.AddRange(
                new UserRole { UserId = "01JSMN2ZYJTXZ93HNTZJF593TH", RoleId = 1 },
                new UserRole { UserId = "01JSMN2ZYJTXZ93HNTZJF593TH", RoleId = 2 },
                new UserRole { UserId = "01JSMN2ZYJTXZ93HNTZJF593TH", RoleId = 3 }
                );
            easyUniDbContext.SaveChanges();
        }
    }
}
