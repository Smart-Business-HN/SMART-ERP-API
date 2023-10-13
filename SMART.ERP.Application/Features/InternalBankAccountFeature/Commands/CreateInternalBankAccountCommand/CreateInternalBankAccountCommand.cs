using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Address;
using SMART.ERP.Application.DTOs.Bank;
using SMART.ERP.Application.DTOs.InternalBankAccount;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Features.CityFeature.Commands.CreateCityCommand;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CitySpecification;
using SMART.ERP.Application.Specifications.InternalBankAccountSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.InternalBankAccountFeature.Commands.CreateInternalBankAccountCommand
{
    public class CreateInternalBankAccountCommand : IRequest<Response<InternalBankAccountDto>>
    {
        public string Name { get; set; } = null!;
        public int BankId { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateInternalBankAccountCommandHandler : IRequestHandler<CreateInternalBankAccountCommand, Response<InternalBankAccountDto>>
    {
        private readonly IRepositoryAsync<InternalBankAccount> _repositoryAsync;
        private readonly IRepositoryAsync<Bank> _bankRepositoryAsync;
        private readonly IMapper _mapper;

        public CreateInternalBankAccountCommandHandler(IRepositoryAsync<InternalBankAccount> repositoryAsync, IRepositoryAsync<Bank> bankRepositoryAsync,
            IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _bankRepositoryAsync = bankRepositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<InternalBankAccountDto>> Handle(CreateInternalBankAccountCommand request, CancellationToken cancellationToken)
        {
            var checkbyName = await _repositoryAsync.FirstOrDefaultAsync(new FilterInternalBankAccountFromNameSpecification(request.Name));
            if (checkbyName != null)
            {
                throw new ApiException($"Ya existe una ciudad con el nombre {request.Name}");
            }

            var checkDepartment = await _bankRepositoryAsync.GetByIdAsync(request.BankId);
            if (checkDepartment == null)
            {
                throw new KeyNotFoundException($"No se encontro el banco con id {request.BankId}");
            }

            var newRecord = _mapper.Map<InternalBankAccount>(request);
            var response = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();

            var dto = _mapper.Map<InternalBankAccountDto>(response);
            return new Response<InternalBankAccountDto>(dto, $"{request.Name} agregada correctamente");
        }
    }
}
