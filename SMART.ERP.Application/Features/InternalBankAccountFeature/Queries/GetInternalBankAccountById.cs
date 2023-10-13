using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Address;
using SMART.ERP.Application.DTOs.InternalBankAccount;
using SMART.ERP.Application.Features.CityFeature.Queries;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.InternalBankAccountFeature.Queries
{
    public class GetInternalBankAccountById : IRequest<Response<InternalBankAccountDto>>
    {
        public int Id { get; set; }
    }
    public class GetInternalBankAccountByIdHandler : IRequestHandler<GetInternalBankAccountById, Response<InternalBankAccountDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<InternalBankAccount> _repositoryAsync;

        public GetInternalBankAccountByIdHandler(IMapper mapper, IRepositoryAsync<InternalBankAccount> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<InternalBankAccountDto>> Handle(GetInternalBankAccountById request, CancellationToken cancellationToken)
        {
            var internalBankAccount = await _repositoryAsync.GetByIdAsync(request.Id);
            if (internalBankAccount == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<InternalBankAccountDto>(internalBankAccount);
            return new Response<InternalBankAccountDto>(dto);
        }
    }
}
