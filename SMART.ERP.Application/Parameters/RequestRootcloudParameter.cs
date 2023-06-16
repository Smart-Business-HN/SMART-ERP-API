namespace SMART.ERP.Application.Parameters
{
    public class RequestRootcloudParameter
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public string? Date { get; set; }
        public string? Parameter { get; set; }
        public bool All { get; set; }

        public RequestRootcloudParameter()
        {
            PageNumber = 0;
            PageSize = 10;
        }

        public RequestRootcloudParameter(int pageNumber, int pageSize, string parameter)
        {
            this.PageNumber = pageNumber < 0 ? 0 : pageNumber;
            this.PageSize = pageSize > 10 ? 10 : pageSize;
        }
    }
}
