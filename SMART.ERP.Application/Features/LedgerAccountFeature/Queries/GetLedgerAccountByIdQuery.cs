using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.LedgerAccount;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.LedgerAccountSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.LedgerAccountFeature.Queries
{
    public class GetLedgerAccountByIdQuery : IRequest<Response<LedgerAccountDto>>
    {
        public int Id { get; set; }

        public class GetLedgerAccountByIdQueryHandler : IRequestHandler<GetLedgerAccountByIdQuery, Response<LedgerAccountDto>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<LedgerAccount> _repositoryAsync;

            public GetLedgerAccountByIdQueryHandler(IMapper mapper, IRepositoryAsync<LedgerAccount> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }

            public async Task<Response<LedgerAccountDto>> Handle(GetLedgerAccountByIdQuery request, CancellationToken cancellationToken)
            {
                var account = await _repositoryAsync.FirstOrDefaultAsync(new FilterLedgerAccountByIdSpecification(request.Id), cancellationToken)
                    ?? throw new KeyNotFoundException($"No existe una cuenta con el Id {request.Id}.");
                var dto = _mapper.Map<LedgerAccountDto>(account);
                return new Response<LedgerAccountDto>(dto);
            }
        }
    }
}
