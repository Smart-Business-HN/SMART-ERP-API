using SMART.ERP.Application.DTOs.Customer;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.DTOs.Status;

namespace SMART.ERP.Application.DTOs.WishList
{
    public class WishListProductDto
    {
        public int Id { get; init; }
        public int ProductId { get; set; }
        public BasicDetailProductDto? Product { get; set; }
        public int StatusId { get; set; }
        public StatusDto? Status { get; set; }
        public DateTime CreationDate { get; set; }
        public int Quantity { get; set; }
        public bool IsActive { get; set; }
    }
}
