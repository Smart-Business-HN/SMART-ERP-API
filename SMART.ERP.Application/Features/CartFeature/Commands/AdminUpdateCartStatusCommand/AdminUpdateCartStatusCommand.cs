using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Cart;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.MetaConversionsService;
using SMART.ERP.Application.Specifications.CartSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.CartFeature.Commands.AdminUpdateCartStatusCommand;

public class AdminUpdateCartStatusCommand : IRequest<Response<CartDto>>
{
    public Guid CartId { get; set; }
    public CartStatus NewStatus { get; set; }
    public string? PaymentLinkUrl { get; set; }
}

public class AdminUpdateCartStatusCommandHandler : IRequestHandler<AdminUpdateCartStatusCommand, Response<CartDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryAsync<Cart> _cartRepositoryAsync;
    private readonly IMetaConversionsService _metaConversionsService;

    public AdminUpdateCartStatusCommandHandler(
        IMapper mapper,
        IRepositoryAsync<Cart> cartRepositoryAsync,
        IMetaConversionsService metaConversionsService)
    {
        _mapper = mapper;
        _cartRepositoryAsync = cartRepositoryAsync;
        _metaConversionsService = metaConversionsService;
    }

    public async Task<Response<CartDto>> Handle(AdminUpdateCartStatusCommand request, CancellationToken cancellationToken)
    {
        var cart = await _cartRepositoryAsync.FirstOrDefaultAsync(
            new GetCartByIdSpecification(request.CartId), cancellationToken);

        if (cart == null)
            throw new KeyNotFoundException($"Carrito no encontrado con el id: {request.CartId}");

        ValidateStatusTransition(cart.Status, request.NewStatus);

        cart.Status = request.NewStatus;

        if (request.NewStatus == CartStatus.PaymentLinkSent)
        {
            if (string.IsNullOrWhiteSpace(request.PaymentLinkUrl))
                throw new ApplicationException("Debe proporcionar un URL de link de pago.");
            cart.PaymentLinkUrl = request.PaymentLinkUrl;
        }

        if (request.NewStatus == CartStatus.Rejected)
        {
            cart.PaymentLinkUrl = null;
        }

        if (request.NewStatus == CartStatus.Verified)
        {
            cart.IsActive = false;
        }

        if (request.NewStatus == CartStatus.Active)
        {
            cart.PaymentLinkUrl = null;
        }

        await _cartRepositoryAsync.UpdateAsync(cart, cancellationToken);

        var reloadedCart = await _cartRepositoryAsync.FirstOrDefaultAsync(
            new GetCartByIdSpecification(cart.Id), cancellationToken);

        // Meta Conversions API: Purchase fiable cuando la compra se confirma.
        // El reloadedCart viene con EcommerceUser y CartItems.Product. El servicio
        // nunca lanza, así que no compromete la actualización del estado.
        if (request.NewStatus == CartStatus.Verified && reloadedCart != null)
        {
            await _metaConversionsService.SendPurchaseAsync(reloadedCart, cancellationToken);
        }

        var dto = _mapper.Map<CartDto>(reloadedCart);
        return new Response<CartDto>(dto, "Estado del carrito actualizado exitosamente.");
    }

    private static void ValidateStatusTransition(CartStatus current, CartStatus target)
    {
        var allowed = current switch
        {
            CartStatus.Active => new[] { CartStatus.ReceiptSubmitted, CartStatus.PaymentLinkRequested },
            CartStatus.ReceiptSubmitted => new[] { CartStatus.Verified, CartStatus.Rejected },
            CartStatus.PaymentLinkRequested => new[] { CartStatus.PaymentLinkSent, CartStatus.Rejected },
            CartStatus.PaymentLinkSent => new[] { CartStatus.Verified, CartStatus.Rejected },
            CartStatus.Rejected => new[] { CartStatus.Active },
            _ => Array.Empty<CartStatus>()
        };

        if (!allowed.Contains(target))
            throw new ApplicationException(
                $"No se puede cambiar el estado del carrito de '{current}' a '{target}'.");
    }
}
