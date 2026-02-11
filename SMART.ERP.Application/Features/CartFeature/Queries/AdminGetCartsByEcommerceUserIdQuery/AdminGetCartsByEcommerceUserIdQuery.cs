using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Cart;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CartSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CartFeature.Queries.AdminGetCartsByEcommerceUserIdQuery;

public class AdminGetCartsByEcommerceUserIdQuery : IRequest<Response<List<CartDto>>>
{
    public Guid EcommerceUserId { get; set; }
    public bool? IsActive { get; set; }
}

public class AdminGetCartsByEcommerceUserIdQueryHandler : IRequestHandler<AdminGetCartsByEcommerceUserIdQuery, Response<List<CartDto>>>
{
    private readonly IRepositoryAsync<Cart> _repositoryAsync;
    private readonly IMapper _mapper;

    public AdminGetCartsByEcommerceUserIdQueryHandler(IMapper mapper, IRepositoryAsync<Cart> repositoryAsync)
    {
        _mapper = mapper;
        _repositoryAsync = repositoryAsync;
    }

    public async Task<Response<List<CartDto>>> Handle(AdminGetCartsByEcommerceUserIdQuery request, CancellationToken cancellationToken)
    {
        var carts = await _repositoryAsync.ListAsync(
            new AdminFilterCartsByEcommerceUserIdSpecification(request.EcommerceUserId, request.IsActive),
            cancellationToken);
        var dto = _mapper.Map<List<Cart>, List<CartDto>>(carts);
        return new Response<List<CartDto>>(dto);
    }
}
