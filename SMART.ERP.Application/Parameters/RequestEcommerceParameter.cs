namespace SMART.ERP.Application.Parameters
{
    public class RequestEcommerceParameter
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Parameter { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
        public bool IsUserSignIn { get; set; }
        public int? CustomerTypeId { get; set; }

        public RequestEcommerceParameter()
        {
            PageNumber = 0;
            PageSize = 10;
        }

        public RequestEcommerceParameter(int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber < 0 ? 0 : pageNumber;
            this.PageSize = pageSize > 10 ? 10 : pageSize;
        }
    }
}
