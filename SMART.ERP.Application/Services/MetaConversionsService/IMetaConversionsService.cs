using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Services.MetaConversionsService
{
    public interface IMetaConversionsService
    {
        // Envía el evento Purchase al webhook del ecommerce cuando la compra se
        // confirma (carrito verificado). El carrito debe venir cargado con
        // EcommerceUser y CartItems.Product. Nunca lanza excepciones.
        Task<bool> SendPurchaseAsync(Cart cart, CancellationToken cancellationToken = default);
    }
}
