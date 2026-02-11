using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.PaymentMethod;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.PaymentMethodFeature.Queries;

public class GetPaymentMethodByIdQuery : IRequest<Response<PaymentMethodDto>>
{
    public int Id { get; set; }
}

public class GetPaymentMethodByIdQueryHandler : IRequestHandler<GetPaymentMethodByIdQuery, Response<PaymentMethodDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryAsync<PaymentMethod> _repositoryAsync;

    public GetPaymentMethodByIdQueryHandler(IMapper mapper, IRepositoryAsync<PaymentMethod> repositoryAsync)
    {
        _mapper = mapper;
        _repositoryAsync = repositoryAsync;
    }

    public async Task<Response<PaymentMethodDto>> Handle(GetPaymentMethodByIdQuery request, CancellationToken cancellationToken)
    {
        var record = await _repositoryAsync.GetByIdAsync(request.Id, cancellationToken);
        if (record == null)
        {
            throw new KeyNotFoundException($"Método de pago no encontrado con el id {request.Id}");
        }

        var dto = _mapper.Map<PaymentMethodDto>(record);
        return new Response<PaymentMethodDto>(dto);
    }
}
