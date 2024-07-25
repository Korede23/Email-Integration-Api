namespace Email_Integration_Api.Dto.ResponseModel
{
    public class BaseResponse<T>
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public T Data { get; set; }
    }
}
