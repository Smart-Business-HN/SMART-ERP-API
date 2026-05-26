using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.CreditCardPayment;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CreditCardPaymentSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CreditCardPaymentFeature.Queries
{
    public class GetAllCreditCardPaymentsQuery : IRequest<PagedResponse<List<CreditCardPaymentDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }

        public class GetAllCreditCardPaymentsQueryHandler : IRequestHandler<GetAllCreditCardPaymentsQuery, PagedResponse<List<CreditCardPaymentDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<CreditCardPayment> _repositoryAsync;

            public GetAllCreditCardPaymentsQueryHandler(IMapper mapper, IRepositoryAsync<CreditCardPayment> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }

            public async Task<PagedResponse<List<CreditCardPaymentDto>>> Handle(GetAllCreditCardPaymentsQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync(cancellationToken);
                }

                var items = await _repositoryAsync.ListAsync(
                    new FilterAndPaginationCreditCardPaymentSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column),
                    cancellationToken);
                var dto = _mapper.Map<List<CreditCardPaymentDto>>(items);
                return new PagedResponse<List<CreditCardPaymentDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync(cancellationToken));
            }
        }
    }
}
