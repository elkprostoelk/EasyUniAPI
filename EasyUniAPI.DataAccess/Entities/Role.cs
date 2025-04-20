namespace EasyUniAPI.DataAccess.Entities
{
    public class Role
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public virtual List<User> Users { get; set; } = [];
    }
}
