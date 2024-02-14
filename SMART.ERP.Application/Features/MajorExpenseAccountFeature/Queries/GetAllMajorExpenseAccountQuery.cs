using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Bank;
using SMART.ERP.Application.DTOs.MajorExpenseAccount;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.BankSpecification;
using SMART.ERP.Application.Specifications.MajorExpenseAccountSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.MajorExpenseAccountFeature.Queries
{
    public class GetAllMajorExpenseAccountQuery : IRequest<PagedResponse<List<MajorExpenseAccountDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
        public class GetAllMajorExpenseAccountQueryHandler : IRequestHandler<GetAllMajorExpenseAccountQuery,PagedResponse<List<MajorExpenseAccountDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<MajorExpenseAccount>  _repositoryAsync;
            public GetAllMajorExpenseAccountQueryHandler(IMapper mapper, IRepositoryAsync<MajorExpenseAccount> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<PagedResponse<List<MajorExpenseAccountDto>>> Handle(GetAllMajorExpenseAccountQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }
                var majorExpenseAccounts = await _repositoryAsync.ListAsync(new FilterAndPaginationMajorExpenseAccountSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<MajorExpenseAccountDto>>(majorExpenseAccounts);
                return new PagedResponse<List<MajorExpenseAccountDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
