using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Address;
using SMART.ERP.Application.DTOs.Bank;
using SMART.ERP.Application.Features.CityFeature.Queries;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.BankSpecification;
using SMART.ERP.Application.Specifications.CitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.BankFeature.Queries
{
    public class GetAllBanksQuery : IRequest<PagedResponse<List<BankDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }

        public class GetAllBanksQueryHandler : IRequestHandler<GetAllBanksQuery, PagedResponse<List<BankDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<Bank> _repositoryAsync;
            public GetAllBanksQueryHandler(IMapper mapper, IRepositoryAsync<Bank> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<PagedResponse<List<BankDto>>> Handle(GetAllBanksQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }
                var cities = await _repositoryAsync.ListAsync(new FilterAndPaginationBankSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<BankDto>>(cities);
                return new PagedResponse<List<BankDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());

            }
        }
    }
}
