using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.PaymentMethod;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.CardEncryptionService;
using SMART.ERP.Application.Specifications.PaymentMethodSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.PaymentMethodFeature.Commands.CreatePaymentMethodCommand;

public class CreatePaymentMethodCommand : IRequest<Response<PaymentMethodDto>>
{
    public Guid EcommerceUserId { get; set; }
    public string Alias { get; set; } = null!;
    public string CardholderName { get; set; } = null!;
    public string CardNumber { get; set; } = null!;
    public int ExpirationMonth { get; set; }
    public int ExpirationYear { get; set; }
    public string CardBrand { get; set; } = null!;
}

public class CreatePaymentMethodCommandHandler : IRequestHandler<CreatePaymentMethodCommand, Response<PaymentMethodDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryAsync<PaymentMethod> _repositoryAsync;
    private readonly ICardEncryptionService _cardEncryptionService;

    public CreatePaymentMethodCommandHandler(IMapper mapper, IRepositoryAsync<PaymentMethod> repositoryAsync, ICardEncryptionService cardEncryptionService)
    {
        _mapper = mapper;
        _repositoryAsync = repositoryAsync;
        _cardEncryptionService = cardEncryptionService;
    }

    public async Task<Response<PaymentMethodDto>> Handle(CreatePaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repositoryAsync.FirstOrDefaultAsync(
            new FilterPaymentMethodSpecification(request.Alias, request.EcommerceUserId, null), cancellationToken);
        if (existing != null)
        {
            throw new ApiException($"Ya existe un método de pago con el alias {request.Alias}");
        }

        var newRecord = _mapper.Map<PaymentMethod>(request);
        newRecord.Last4Digits = request.CardNumber[^4..];
        newRecord.EncryptedCardNumber = _cardEncryptionService.Encrypt(request.CardNumber);
        newRecord.CreationDate = DateTime.UtcNow;
        newRecord.IsActive = true;

        var data = await _repositoryAsync.AddAsync(newRecord, cancellationToken);
        await _repositoryAsync.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<PaymentMethodDto>(data);
        return new Response<PaymentMethodDto>(dto, message: $"Método de pago '{request.Alias}' creado exitosamente");
    }
}
