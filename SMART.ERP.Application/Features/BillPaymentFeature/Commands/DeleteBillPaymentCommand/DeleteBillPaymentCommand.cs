using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BillPaymentFeature.Commands.DeleteBillPaymentCommand
{
    public class DeleteBillPaymentCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteBillPaymentCommandHandler : IRequestHandler<DeleteBillPaymentCommand, Response<string>>
    {
        private readonly IRepositoryAsync<BillPayment> _repositoryAsync;
        private readonly IRepositoryAsync<Invoice> _invoiceRepositoryAsync;

        public DeleteBillPaymentCommandHandler(IRepositoryAsync<BillPayment> repositoryAsync, IRepositoryAsync<Invoice> invoiceRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _invoiceRepositoryAsync = invoiceRepositoryAsync;
        }

        public async Task<Response<string>> Handle(DeleteBillPaymentCommand request, CancellationToken cancellationToken)
        {
            var checkBillPayment = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkBillPayment == null)
            {
                throw new KeyNotFoundException($"No se encontro un pago con id {request.Id}");
            }
            var invoice = await _invoiceRepositoryAsync.GetByIdAsync(checkBillPayment.InvoiceId);
            invoice!.Outstanding = invoice.Outstanding + checkBillPayment.Amount;

            try
            {

                await _repositoryAsync.DeleteAsync(checkBillPayment);
                await _repositoryAsync.SaveChangesAsync();
                await _invoiceRepositoryAsync.UpdateAsync(invoice);
                await _invoiceRepositoryAsync.SaveChangesAsync();

                return new Response<string>("Eliminado correctamente");
            }
            catch (Exception)
            {
                throw new ApiException("Ocurrio un error al tratar de eliminar este registro, verifica que no se este utilizando en otro registro.");
            }
        }
    }
}
