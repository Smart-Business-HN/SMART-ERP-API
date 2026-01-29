using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Customer;
using SMART.ERP.Application.DTOs.WishList;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.WishListSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.WishListFeature.Queries
{
    public class GetAllWishListsQuery : IRequest<PagedResponse<List<WishListDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
        public class GetAllWishListsQueryHandler : IRequestHandler<GetAllWishListsQuery, PagedResponse<List<WishListDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<WishList> _repositoryAsync;
            private readonly IRepositoryAsync<Customer> _customerRepositoryAsync;

            public GetAllWishListsQueryHandler(IMapper mapper, IRepositoryAsync<WishList> repositoryAsync,
                IRepositoryAsync<Customer> customerRepositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
                _customerRepositoryAsync = customerRepositoryAsync;
            }

            public async Task<PagedResponse<List<WishListDto>>> Handle(GetAllWishListsQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }

                var wishLists = await _repositoryAsync.ListAsync(
                    new FilterAndPaginationWishListSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<WishListDto>>(wishLists);
                foreach (var wish in dto)
                {
                    var customer = await _customerRepositoryAsync.GetByIdAsync(wish.CustomerId);
                    wish.Customer = _mapper.Map<BasicInfoCustomerDto>(customer);
                }
                return new PagedResponse<List<WishListDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
