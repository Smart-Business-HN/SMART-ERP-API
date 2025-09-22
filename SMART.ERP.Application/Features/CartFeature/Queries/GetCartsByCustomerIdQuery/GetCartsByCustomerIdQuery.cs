using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Cart;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CartSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CartFeature.Queries.GetCartsByCustomerIdQuery;

public class GetCartsByCustomerIdQuery : IRequest<Response<List<CartDto>>>
{
    public Guid CustomerId { get; set; }
}
public class GetCartsByCustomerIdQueryHandler : IRequestHandler<GetCartsByCustomerIdQuery, Response<List<CartDto>>>
{
    private readonly IRepositoryAsync<Cart> _repositoryAsync;
    private readonly IMapper _mapper;

    public GetCartsByCustomerIdQueryHandler(IMapper mapper, IRepositoryAsync<Cart> repositoryAsync)
    {
        _mapper = mapper;
        _repositoryAsync = repositoryAsync;
    }

    public async Task<Response<List<CartDto>>> Handle(GetCartsByCustomerIdQuery request, CancellationToken cancellationToken)
    {
       var carts = await _repositoryAsync.ListAsync(new FilterCartByCustomerIdSpecification(request.CustomerId,null), cancellationToken);
       var dto = _mapper.Map<List<Cart>, List<CartDto>>(carts);
       return new Response<List<CartDto>>(dto);
    }
}