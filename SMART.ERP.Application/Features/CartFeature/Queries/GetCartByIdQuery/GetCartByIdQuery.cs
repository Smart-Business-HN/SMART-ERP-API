using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Cart;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CartSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CartFeature.Queries.GetCartByIdQuery;

public class GetCartByIdQuery : IRequest<Response<CartDto>>
{
    public Guid Id { get; set; }
}
public record GetCartByIdQueryHandler : IRequestHandler<GetCartByIdQuery, Response<CartDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryAsync<Cart> _repositoryAsync;
    public GetCartByIdQueryHandler(IMapper mapper, IRepositoryAsync<Cart> repositoryAsync)
    {
        _mapper = mapper;
        _repositoryAsync = repositoryAsync;
    }
    public async Task<Response<CartDto>> Handle(GetCartByIdQuery request, CancellationToken cancellationToken)
    {
       var cart = await _repositoryAsync.FirstOrDefaultAsync(new GetCartByIdSpecification(request.Id));
        if (cart == null)
        {
            throw new KeyNotFoundException($"Carrito de compras no encontrado con el id {request.Id}");
        }
        var dto = _mapper.Map<CartDto>(cart);
        return new Response<CartDto>(dto);
    }
}