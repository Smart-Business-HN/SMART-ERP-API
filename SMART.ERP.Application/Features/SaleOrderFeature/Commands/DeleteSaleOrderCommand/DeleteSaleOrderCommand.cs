using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.SaleOrderSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.SaleOrderFeature.Commands.DeleteSaleOrderCommand
{
    public class DeleteSaleOrderCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteSaleOrderCommandHandle : IRequestHandler<DeleteSaleOrderCommand, Response<string>>
    {
        private readonly IRepositoryAsync<SaleOrder> _repositoryAsync;
        private readonly IRepositoryAsync<SaleOrderProduct> _saleOrderProductRepositoryAsync;

        public DeleteSaleOrderCommandHandle(IRepositoryAsync<SaleOrder> repositoryAsync,
            IRepositoryAsync<SaleOrderProduct> saleOrderProductRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _saleOrderProductRepositoryAsync = saleOrderProductRepositoryAsync;
        }
        public async Task<Response<string>> Handle(DeleteSaleOrderCommand request, CancellationToken cancellationToken)
        {
            var saleOrder = await _repositoryAsync.FirstOrDefaultAsync(
                new SaleOrderIncludesSpecification(request.Id));
            if (saleOrder == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            if (saleOrder.SaleOrderProducts != null)
            {
                foreach (var item in saleOrder.SaleOrderProducts)
                {
                    await _saleOrderProductRepositoryAsync.DeleteAsync(item);
                    await _saleOrderProductRepositoryAsync.SaveChangesAsync();
                }
            }
            await _repositoryAsync.DeleteAsync(saleOrder);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"Cotización con codigo {saleOrder.Code} eliminada correctamente", "Eliminada correctamente");
        }
    }
}
