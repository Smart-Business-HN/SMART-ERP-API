using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CartFeature.Commands.AdminDeleteCartCommand;

public class AdminDeleteCartCommand : IRequest<Response<string>>
{
    public Guid CartId { get; set; }
}

public class AdminDeleteCartCommandHandler : IRequestHandler<AdminDeleteCartCommand, Response<string>>
{
    private readonly IRepositoryAsync<Cart> _cartRepositoryAsync;

    public AdminDeleteCartCommandHandler(IRepositoryAsync<Cart> cartRepositoryAsync)
    {
        _cartRepositoryAsync = cartRepositoryAsync;
    }

    public async Task<Response<string>> Handle(AdminDeleteCartCommand request, CancellationToken cancellationToken)
    {
        var cart = await _cartRepositoryAsync.GetByIdAsync(request.CartId, cancellationToken);
        if (cart == null)
            throw new KeyNotFoundException($"Carrito no encontrado con el id: {request.CartId}");

        await _cartRepositoryAsync.DeleteAsync(cart, cancellationToken);

        return new Response<string>("Carrito eliminado exitosamente.");
    }
}
