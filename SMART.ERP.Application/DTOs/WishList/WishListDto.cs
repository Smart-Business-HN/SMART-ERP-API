using SMART.ERP.Application.DTOs.Status;
using SMART.ERP.Application.DTOs.Customer;

namespace SMART.ERP.Application.DTOs.WishList
{
    public class WishListDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public int CantItems { get; set; }
        public Guid CustomerId { get; set; }
        public BasicInfoCustomerDto Customer { get; set; } = null!;
        public int StatusId { get; set; }
        public StatusDto? Status { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public List<WishListProductDto>? WishListProducts { get; set; }
    }
}
