using MediatR;
using SMART.ERP.Application.DTOs.Mail;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.MailService;
using SMART.ERP.Application.Specifications.CartSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.CheckoutFeature.Commands.RequestPaymentLinkCommand
{
    public class RequestPaymentLinkCommand : IRequest<Response<string>>
    {
        public Guid CartId { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string CustomerEmail { get; set; } = null!;
        public string CustomerPhone { get; set; } = null!;
        public string? Message { get; set; }
    }

    public class RequestPaymentLinkCommandHandler : IRequestHandler<RequestPaymentLinkCommand, Response<string>>
    {
        private readonly IRepositoryAsync<Cart> _cartRepositoryAsync;
        private readonly IMailService _mailService;

        public RequestPaymentLinkCommandHandler(
            IRepositoryAsync<Cart> cartRepositoryAsync,
            IMailService mailService)
        {
            _cartRepositoryAsync = cartRepositoryAsync;
            _mailService = mailService;
        }

        public async Task<Response<string>> Handle(RequestPaymentLinkCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepositoryAsync.FirstOrDefaultAsync(
                new GetCartByIdSpecification(request.CartId), cancellationToken);

            if (cart == null)
                throw new KeyNotFoundException($"Cart not found with id {request.CartId}");

            if (cart.EcommerceUserId != request.CustomerId)
                throw new KeyNotFoundException($"Cart does not belong to customer {request.CustomerId}");

            if (cart.Status != CartStatus.Active)
                throw new ApplicationException("Este carrito ya tiene un proceso de pago en curso.");

            // Build order summary HTML
            var itemsHtml = "";
            decimal subtotal = 0;

            if (cart.CartItems != null)
            {
                foreach (var item in cart.CartItems)
                {
                    subtotal += item.TotalPrice;
                    itemsHtml += $@"
                        <tr>
                            <td style='padding:8px;border-bottom:1px solid #eee;'>{item.Product?.Name ?? "Producto"}</td>
                            <td style='padding:8px;border-bottom:1px solid #eee;text-align:center;'>{item.Quantity}</td>
                            <td style='padding:8px;border-bottom:1px solid #eee;text-align:right;'>L. {item.UnitPrice:N2}</td>
                            <td style='padding:8px;border-bottom:1px solid #eee;text-align:right;'>L. {item.TotalPrice:N2}</td>
                        </tr>";
                }
            }

            var tax = subtotal * 0.15m;
            var total = subtotal + tax;

            var body = $@"
                <h2>Solicitud de Link de Pago</h2>
                <p><strong>Cliente:</strong> {request.CustomerName}</p>
                <p><strong>Email:</strong> {request.CustomerEmail}</p>
                <p><strong>Teléfono:</strong> {request.CustomerPhone}</p>
                <p><strong>Carrito ID:</strong> {request.CartId}</p>
                {(string.IsNullOrEmpty(request.Message) ? "" : $"<p><strong>Mensaje:</strong> {request.Message}</p>")}
                <hr/>
                <h3>Resumen del Pedido</h3>
                <table style='width:100%;border-collapse:collapse;'>
                    <thead>
                        <tr style='background:#f5f5f5;'>
                            <th style='padding:8px;text-align:left;'>Producto</th>
                            <th style='padding:8px;text-align:center;'>Cant.</th>
                            <th style='padding:8px;text-align:right;'>Precio Unit.</th>
                            <th style='padding:8px;text-align:right;'>Total</th>
                        </tr>
                    </thead>
                    <tbody>
                        {itemsHtml}
                    </tbody>
                </table>
                <br/>
                <p><strong>Subtotal:</strong> L. {subtotal:N2}</p>
                <p><strong>ISV (15%):</strong> L. {tax:N2}</p>
                <p style='font-size:18px;'><strong>Total a Pagar:</strong> L. {total:N2}</p>";

            var mailRequest = new MailRequestDto
            {
                ToEmail = "ventas@smartbusiness.site",
                ToCCEmail = request.CustomerEmail,
                Subject = $"Solicitud de Link de Pago - {request.CustomerName} - Carrito #{request.CartId.ToString()[^8..]}",
                Body = body
            };

            await _mailService.SendEmailAsync(mailRequest);

            cart.Status = CartStatus.PaymentLinkRequested;
            await _cartRepositoryAsync.UpdateAsync(cart, cancellationToken);

            return new Response<string>("Solicitud de link de pago enviada exitosamente.");
        }
    }
}
