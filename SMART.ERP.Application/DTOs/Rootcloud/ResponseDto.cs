namespace SMART.ERP.Application.DTOs.Rootcloud
{
    public class ResponseDto<T>
    {

        public string? Ret { get; set; }
        public string? Msg { get; set; }
        public T Data { get; set; }

        public ResponseDto(T data, string? message = null, string? ret = null)
        {
            Ret = ret;
            Msg = message;
            Data = data;
        }
    }
}
