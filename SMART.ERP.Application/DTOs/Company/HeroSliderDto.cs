using SMART.ERP.Application.DTOs.Product;

namespace SMART.ERP.Application.DTOs.Company
{
    public class HeroSliderDto
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public int ProductId { get; set; }
        public ResumeProductDto? Product { get; set; }
    }
}
