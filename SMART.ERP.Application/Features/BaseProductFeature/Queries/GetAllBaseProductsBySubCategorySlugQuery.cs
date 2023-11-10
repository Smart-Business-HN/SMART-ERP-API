using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CategorySpecification;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Specifications.SubcategorySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.BaseProductFeature.Queries
{
    public class GetAllBaseProductsBySubCategorySlugQuery : IRequest<PagedResponse<List<ProductDto>>>
    {
        public string SubCategorySlug { get; set; } = null!;
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public class GetAllBaseProductsBySubCategorySlugQueryHandler : IRequestHandler<GetAllBaseProductsBySubCategorySlugQuery, PagedResponse<List<ProductDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<Product> _repositoryAsync;
            private readonly IRepositoryAsync<Subcategory> _subCategoryRepositoryAsync;

            public GetAllBaseProductsBySubCategorySlugQueryHandler(IMapper mapper, IRepositoryAsync<Product> repositoryAsync,
                IRepositoryAsync<Subcategory> subCategoryRepositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
                _subCategoryRepositoryAsync = subCategoryRepositoryAsync;
            }
            public async Task<PagedResponse<List<ProductDto>>> Handle(GetAllBaseProductsBySubCategorySlugQuery request, CancellationToken cancellationToken)
            {
                var checkCategory = await _subCategoryRepositoryAsync.FirstOrDefaultAsync(new GetSubCategoryBySlugSpecification(request.SubCategorySlug));
                if (checkCategory == null)
                {
                    throw new KeyNotFoundException($"Registro no encontrado con el slug {request.SubCategorySlug}");
                }
                var products = await _repositoryAsync.ListAsync(
                    new FilterAndPaginationProductsBySubCategorySlugSpecification(request.SubCategorySlug, request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<ProductDto>>(products);
                return new PagedResponse<List<ProductDto>>(dto, request.PageNumber, request.PageSize, false ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
