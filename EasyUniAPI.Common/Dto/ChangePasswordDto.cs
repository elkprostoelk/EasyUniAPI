namespace EasyUniAPI.Common.Dto
{
    public class ChangePasswordDto
    {
        public string UserId { get; set; }

        public required string OldPassword { get; set; }

        public required string NewPassword { get; set; }
    }
}
