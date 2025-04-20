namespace EasyUniAPI.Common.Dto
{
    public class ServiceResultDto
    {
        public bool IsSuccess { get; set; }

        public List<string> Errors { get; set; } = [];
    }

    public class ServiceResultDto<T> : ServiceResultDto
    {
        public T? Result { get; set; }
    }
}
