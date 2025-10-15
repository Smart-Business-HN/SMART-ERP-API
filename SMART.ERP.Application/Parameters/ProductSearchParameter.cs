namespace SMART.ERP.Application.Parameters
{
    public class ProductSearchParameter
    {
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 0;
        public int PageSize { get; set; } = 10;
        public string? Order { get; set; } = "asc";
        public string? Column { get; set; } = "name";
        public bool All { get; set; } = false;
        public bool IsUserSignIn { get; set; } = false;
        public int? CustomerTypeId { get; set; }
        
        // Filtros adicionales para mejorar la búsqueda
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public bool? InStock { get; set; }
        public bool? HasImages { get; set; }
        public string? SortBy { get; set; } = "relevance"; // relevance, price, name, newest

        public ProductSearchParameter()
        {
            PageNumber = 0;
            PageSize = 10;
        }

        public ProductSearchParameter(int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber < 0 ? 0 : pageNumber;
            this.PageSize = pageSize > 50 ? 50 : pageSize; // Aumentar límite para búsquedas
        }
    }
}

