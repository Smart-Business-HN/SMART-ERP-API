using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Bank;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.BankSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BankFeature.Queries
{
    public class GetBankByIdQuery : IRequest<Response<BankDto>>
    {
        public int Id { get; set; }

        public class GetBankByIdQueryHandler : IRequestHandler<GetBankByIdQuery, Response<BankDto>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<Bank> _repositoryAsync;
            public GetBankByIdQueryHandler(IMapper mapper, IRepositoryAsync<Bank> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<Response<BankDto>> Handle(GetBankByIdQuery request, CancellationToken cancellationToken)
            {
                var bank = await _repositoryAsync.FirstOrDefaultAsync(new BankIncludesSpecification(request.Id));
                if (bank == null)
                {
                    throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
                }
                var dto = _mapper.Map<BankDto>(bank);
                return new Response<BankDto>(dto);
            }
        }
    }
}
