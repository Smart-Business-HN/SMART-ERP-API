namespace SMART.ERP.Application.Wrappers
{
    public class PagedRootcloudResponse<T> : Response<T>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalItems { get; set; }

        public PagedRootcloudResponse(T data, int pageNumber, int pageSize, int totalItems)
        {
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
            this.Data = data;
            this.Message = null;
            this.Succeeded = true;
            this.Errors = null;
            this.TotalItems = totalItems;
        }
    }
}
