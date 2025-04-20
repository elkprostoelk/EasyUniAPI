namespace EasyUniAPI.DataAccess.Entities
{
    public class UserRole
    {
        public required string UserId { get; set; }

        public required int RoleId { get; set; }

        public virtual Role Role { get; set; } = null!;

        public virtual User User { get; set; } = null!;
    }
}
