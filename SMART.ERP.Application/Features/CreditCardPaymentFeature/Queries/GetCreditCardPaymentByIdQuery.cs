using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.CreditCardPayment;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CreditCardPaymentSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CreditCardPaymentFeature.Queries
{
    public class GetCreditCardPaymentByIdQuery : IRequest<Response<CreditCardPaymentDto>>
    {
        public int Id { get; set; }
    }

    public class GetCreditCardPaymentByIdQueryHandler : IRequestHandler<GetCreditCardPaymentByIdQuery, Response<CreditCardPaymentDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<CreditCardPayment> _repositoryAsync;

        public GetCreditCardPaymentByIdQueryHandler(IMapper mapper, IRepositoryAsync<CreditCardPayment> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<CreditCardPaymentDto>> Handle(GetCreditCardPaymentByIdQuery request, CancellationToken cancellationToken)
        {
            var payment = await _repositoryAsync.FirstOrDefaultAsync(new FilterCreditCardPaymentByIdSpecification(request.Id), cancellationToken)
                ?? throw new ApiException($"No existe un pago de tarjeta de crédito con Id {request.Id}.");
            var dto = _mapper.Map<CreditCardPaymentDto>(payment);
            return new Response<CreditCardPaymentDto>(dto);
        }
    }
}
