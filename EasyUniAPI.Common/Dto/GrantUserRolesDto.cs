namespace EasyUniAPI.Common.Dto
{
    public class GrantUserRolesDto
    {
        public required string UserId { get; set; }

        public List<int> RoleIds { get; set; } = [];
    }
}
