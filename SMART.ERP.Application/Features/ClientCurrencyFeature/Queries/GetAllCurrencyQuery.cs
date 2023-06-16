using AutoMapper;
using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.MASTER.Domain.Entities;
using SMART.ERP.Application.DTOs.Customer;
using System.Collections.Generic;

namespace SMART.ERP.Application.Features.ClientCurrencyFeature.Queries
{
    public class GetAllCurrencyQuery : IRequest<Response<List<CurrencyDto>>>
    {
        public class GetAllCurrencyQueryHandler : IRequestHandler<GetAllCurrencyQuery, Response<List<CurrencyDto>>>
        {
            private readonly IRepositoryHNAsync<ClientCurrency> _repositoryHNAsync;
            private readonly IMapper _mapper;

            public GetAllCurrencyQueryHandler(IRepositoryHNAsync<ClientCurrency> repositoryHNAsync, IMapper mapper)
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
