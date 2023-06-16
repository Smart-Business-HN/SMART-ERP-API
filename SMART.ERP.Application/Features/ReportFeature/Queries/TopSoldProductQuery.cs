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
    public class TopSoldProductQuery : IRequest<PagedResponse<List<ReportQuoteProductDto>>>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? BranchOfficeId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool All { get; set; }
    }

    public class TopSoldProductQueryHandler : IRequestHandler<TopSoldProductQuery, PagedResponse<List<ReportQuoteProductDto>>>
    {
        private readonly IRepositoryAsync<Opportunity> _repositoryAsync;
        private readonly IRepositoryAsync<BranchOffices> _branchRepositoryAsync;
        private readonly IRepositoryAsync<Category> _categoryRepositoryAsync;

        public TopSoldProductQueryHandler(IRepositoryAsync<Opportunity> repositoryAsync, IRepositoryAsync<BranchOffices> branchRepositoryAsync,
            IRepositoryAsync<Category> categoryRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _branchRepositoryAsync = branchRepositoryAsync;
            _categoryRepositoryAsync = categoryRepositoryAsync;
        }
        public async Task<PagedResponse<List<ReportQuoteProductDto>>> Handle(TopSoldProductQuery request, CancellationToken cancellationToken)
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
            var opportunities = await _repositoryAsync.ListAsync(new TopSoldProductSpecification(request.StartDate, request.EndDate, request.BranchOfficeId));
            var response = new List<ReportQuoteProductDto>();
            opportunities.ForEach(opp =>
            {
                if (opp.QuoteProducts != null)
                {
                    opp.QuoteProducts!.ForEach(quote =>
                    {
                        if (response.Exists(z => z.Name == quote.Product!.Name))
                        {
                            var quoteProduct = response.Find(x => x.Name == quote.Product!.Name);
                            quoteProduct!.TotalNum += quote.Quantity;
                            quoteProduct!.Total += quote.Quantity * quote.SalePrice;
                            decimal marginResult = quote.Quantity * quote.SalePrice - quote.Product!.RecomendedSalePrice * quote.Quantity;
                            if (marginResult > 0)
                            {
                                quoteProduct!.WinTotal += marginResult;
                            }
                            else
                            {
                                quoteProduct!.LossTotal += Math.Abs(marginResult);
                            }
                        }
                        else
                        {
                            var category = categories.Find(x => x.Subcategories.Exists(y => y.Id == quote.Product!.SubCategoryId));
                            var newReportQuote = new ReportQuoteProductDto();
                            newReportQuote.Name = quote.Product!.Name;
                            newReportQuote.SubCategory = quote.Product!.SubCategory!.Name;
                            newReportQuote.Category = category!.Name;
                            newReportQuote.TotalNum = quote.Quantity;
                            newReportQuote.Total = quote.Quantity * quote.SalePrice;
                            decimal marginResult = quote.Quantity * quote.SalePrice - quote.Product!.RecomendedSalePrice * quote.Quantity;
                            if (marginResult > 0)
                            {
                                newReportQuote.WinTotal += marginResult;
                            }
                            else
                            {
                                newReportQuote.LossTotal += Math.Abs(marginResult);
                            }
                            response.Add(newReportQuote);
                        }
                    });
                }
            });
            response.Sort(delegate (ReportQuoteProductDto a, ReportQuoteProductDto b)
            {
                return b.TotalNum.CompareTo(a.TotalNum);
            });
            if (request.All)
            {
                request.PageNumber = 0;
                request.PageSize = response.Count;
            }
            var pagedResponse = response.Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
            return new PagedResponse<List<ReportQuoteProductDto>>(pagedResponse, request.PageNumber, request.PageSize, response.Count);
        }
    }
}
