using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Customer;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ClientCurrencyFeature.Queries
{
    public class GetAllCurrencyQuery : IRequest<Response<List<CurrencyDto>>>
    {
        public class GetAllCurrencyQueryHandler : IRequestHandler<GetAllCurrencyQuery, Response<List<CurrencyDto>>>
        {
            private readonly IRepositoryAsync<Currency> _repositoryHNAsync;
            private readonly IMapper _mapper;

            public GetAllCurrencyQueryHandler(IRepositoryAsync<Currency> repositoryHNAsync, IMapper mapper)
            {
                _repositoryHNAsync = repositoryHNAsync;
                _mapper = mapper;
            }

            public async Task<Response<List<CurrencyDto>>> Handle(GetAllCurrencyQuery request, CancellationToken cancellationToken)
            {
                var currencyList = await _repositoryHNAsync.ListAsync();
                var dto = _mapper.Map<List<CurrencyDto>>(currencyList);
                return new Response<List<CurrencyDto>>(dto);
            }
        }
    }
}
