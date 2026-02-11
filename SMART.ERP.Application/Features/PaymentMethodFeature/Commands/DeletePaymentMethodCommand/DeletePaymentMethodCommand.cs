using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.PaymentMethodFeature.Commands.DeletePaymentMethodCommand;

public class DeletePaymentMethodCommand : IRequest<Response<string>>
{
    public int Id { get; set; }
}

public class DeletePaymentMethodCommandHandler : IRequestHandler<DeletePaymentMethodCommand, Response<string>>
{
    private readonly IRepositoryAsync<PaymentMethod> _repositoryAsync;

    public DeletePaymentMethodCommandHandler(IRepositoryAsync<PaymentMethod> repositoryAsync)
    {
        _repositoryAsync = repositoryAsync;
    }

    public async Task<Response<string>> Handle(DeletePaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var record = await _repositoryAsync.GetByIdAsync(request.Id, cancellationToken);
        if (record == null)
        {
            throw new KeyNotFoundException($"No se encontró ningún método de pago con el id {request.Id}");
        }

        await _repositoryAsync.DeleteAsync(record, cancellationToken);
        await _repositoryAsync.SaveChangesAsync(cancellationToken);

        return new Response<string>($"Método de pago '{record.Alias}' eliminado correctamente", "Eliminado correctamente");
    }
}
