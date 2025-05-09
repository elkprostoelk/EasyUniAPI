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
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            easyUniDbContext = new EasyUniDbContext(dbContextOptions);
            easyUniDbContext.Database.EnsureCreated();

            userRepository = new Repository<User, string>(easyUniDbContext);
            roleRepository = new Repository<Role, int>(easyUniDbContext);
            userRoleRepository = new Repository<UserRole, long>(easyUniDbContext);

            SeedUsersWithRoles();
        }

        private void SeedUsersWithRoles()
        {
            easyUniDbContext.Users.AddRange(
                new User { Id = "01JSMN2ZYJTXZ93HNTZJF593TH", FirstName = "Admin", MiddleName = "Adminovich", LastName = "Admin", Email = "admin@admin.com", PhoneNumber = "+1234567890", BirthDate = new DateOnly(1990, 1, 1), PasswordHash = "UxgDBwC9sS+9Wl+b5MOPRdNlSzURsNBhIC5OQFVwkOQ=", PasswordSalt = "9KrQ5NYDug57LGh/A+ctgA==" }, // pass: strongPa$$word123
                new User { Id = "01JST5PR09DKBYK0FJKSPW61VT", FirstName = "Teacher", MiddleName = "Teacherovich", LastName = "Never", Email = "teacher.never@gmail.com", PhoneNumber = "+1111111111", BirthDate = new DateOnly(1980, 7, 26), PasswordHash = "yszyVc9o1X6wLL+Vw+lgSi4rPvvrO4YUGHjqaPWTlW0=", PasswordSalt = "Zw61OdyX5hJwjWB2FQimZA==" }, // pass: strongPa$$word345
                new User { Id = "01JTV1XEAH8ZT963X8XE3ACJ69", FirstName = "StudentOne", MiddleName = "Studentovich", LastName = "Never", Email = "studentone.never@gmail.com", PhoneNumber = "+2222222222", BirthDate = new DateOnly(2005, 11, 3), PasswordHash = "y6wWFj59IfYdkKtcrisDKXStvstM9q8d7lU4ZXkCIoE=", PasswordSalt = "IhrtoCrp2+W+/8I9Or6xPQ==", Active = false } // pass: strongPa$$word567
                );

            easyUniDbContext.SaveChanges();

            easyUniDbContext.UserRoles.AddRange(
                new UserRole { UserId = "01JSMN2ZYJTXZ93HNTZJF593TH", RoleId = 1 },
                new UserRole { UserId = "01JSMN2ZYJTXZ93HNTZJF593TH", RoleId = 2 },
                new UserRole { UserId = "01JSMN2ZYJTXZ93HNTZJF593TH", RoleId = 3 },
                new UserRole { UserId = "01JST5PR09DKBYK0FJKSPW61VT", RoleId = 2 },
                new UserRole { UserId = "01JTV1XEAH8ZT963X8XE3ACJ69", RoleId = 3 }
                );
            easyUniDbContext.SaveChanges();
        }
    }
}
