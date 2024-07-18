using MediatR;
using SMART.ERP.Application.DTOs.Report;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CategorySpecification;
using SMART.ERP.Application.Specifications.ReportSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ReportFeature.Queries
{
    public class TopSoldProductQuery : IRequest<PagedResponse<List<ReportProductSoldDto>>>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? BranchOfficeId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool All { get; set; }
    }

    public class TopSoldProductQueryHandler : IRequestHandler<TopSoldProductQuery, PagedResponse<List<ReportProductSoldDto>>>
    {
        private readonly IRepositoryAsync<Invoice> _repositoryAsync;
        private readonly IRepositoryAsync<BranchOffices> _branchRepositoryAsync;
        private readonly IRepositoryAsync<Category> _categoryRepositoryAsync;

        public TopSoldProductQueryHandler(IRepositoryAsync<Invoice> repositoryAsync, IRepositoryAsync<BranchOffices> branchRepositoryAsync,
            IRepositoryAsync<Category> categoryRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _branchRepositoryAsync = branchRepositoryAsync;
            _categoryRepositoryAsync = categoryRepositoryAsync;
        }
        public async Task<PagedResponse<List<ReportProductSoldDto>>> Handle(TopSoldProductQuery request, CancellationToken cancellationToken)
        {
            if (request.StartDate != null && request.EndDate != null)
            {
                if (request.EndDate < request.StartDate)
                {
                    throw new ApiException("La fecha final debe ser mayor a la fecha inicial");
                }
            }
            if (request.BranchOfficeId != null)
            {
                var checkBranch = await _branchRepositoryAsync.GetByIdAsync((int)request.BranchOfficeId);
                if (checkBranch == null)
                {
                    throw new KeyNotFoundException($"No se encontro la sucursal con id {request.BranchOfficeId}");
                }
            }

            var categories = await _categoryRepositoryAsync.ListAsync(new IncludeCategorySpecification());
            var invoices = await _repositoryAsync.ListAsync(new TopSoldProductSpecification(request.StartDate, request.EndDate, request.BranchOfficeId));
            var response = new List<ReportProductSoldDto>();
            invoices.ForEach(opp =>
            {
                if (opp.ProductsSold != null)
                {
                    opp.ProductsSold!.ForEach(quote =>
                    {
                        if (response.Exists(z => z.Name == quote.Product!.Name))
                        {
                            var quoteProduct = response.Find(x => x.Name == quote.Product!.Name);
                            quoteProduct!.Quantity += (int)quote.Quantity;
                            quoteProduct!.Total += quote.TotalLine;
                        }
                        else
                        {
                            var category = categories.Find(x => x.Subcategories.Exists(y => y.Id == quote.Product!.SubCategoryId));
                            var newReportQuote = new ReportProductSoldDto();
                            newReportQuote.Name = quote.Product!.Name;
                            newReportQuote.SubCategory = quote.Product!.SubCategory!.Name;
                            newReportQuote.Category = category!.Name;
                            newReportQuote.Quantity = (int)quote.Quantity;
                            newReportQuote.Total = quote.TotalLine;
                            response.Add(newReportQuote);
                        }
                    });
                }
            });
            response.Sort(delegate (ReportProductSoldDto a, ReportProductSoldDto b)
            {
                return b.Quantity.CompareTo(a.Quantity);
            });
            if (request.All)
            {
                request.PageNumber = 0;
                request.PageSize = response.Count;
            }
            var pagedResponse = response.Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
            return new PagedResponse<List<ReportProductSoldDto>>(pagedResponse, request.PageNumber, request.PageSize, response.Count);
        }
    }
}
