using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.PaymentMethod;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.PaymentMethodSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.PaymentMethodFeature.Commands.UpdatePaymentMethodCommand;

public class UpdatePaymentMethodCommand : IRequest<Response<PaymentMethodDto>>
{
    public int Id { get; set; }
    public string Alias { get; set; } = null!;
    public string CardholderName { get; set; } = null!;
    public int ExpirationMonth { get; set; }
    public int ExpirationYear { get; set; }
    public bool IsActive { get; set; }
}

public class UpdatePaymentMethodCommandHandler : IRequestHandler<UpdatePaymentMethodCommand, Response<PaymentMethodDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryAsync<PaymentMethod> _repositoryAsync;

    public UpdatePaymentMethodCommandHandler(IMapper mapper, IRepositoryAsync<PaymentMethod> repositoryAsync)
    {
        _mapper = mapper;
        _repositoryAsync = repositoryAsync;
    }

    public async Task<Response<PaymentMethodDto>> Handle(UpdatePaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var record = await _repositoryAsync.GetByIdAsync(request.Id, cancellationToken);
        if (record == null)
        {
            throw new KeyNotFoundException($"No se encontró ningún método de pago con el id {request.Id}");
        }

        var checkIfExist = await _repositoryAsync.FirstOrDefaultAsync(
            new FilterPaymentMethodSpecification(request.Alias, record.EcommerceUserId, request.Id), cancellationToken);
        if (checkIfExist != null)
        {
            throw new ApiException($"Ya existe un método de pago con el alias {request.Alias}");
        }

        record.Alias = request.Alias;
        record.CardholderName = request.CardholderName;
        record.ExpirationMonth = request.ExpirationMonth;
        record.ExpirationYear = request.ExpirationYear;
        record.IsActive = request.IsActive;

        await _repositoryAsync.UpdateAsync(record, cancellationToken);
        await _repositoryAsync.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<PaymentMethodDto>(record);
        return new Response<PaymentMethodDto>(dto, message: $"Método de pago '{record.Alias}' actualizado correctamente");
    }
}
