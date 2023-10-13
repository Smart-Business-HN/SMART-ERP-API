using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Address;
using SMART.ERP.Application.DTOs.Bank;
using SMART.ERP.Application.Features.CityFeature.Commands.UpdateCityCommand;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.BankFeature.Commands.UpdateBankCommand
{
    public class UpdateBankCommand : IRequest<Response<BankDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool ItIsNationBank { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateBankCommandHandler : IRequestHandler<UpdateBankCommand, Response<BankDto>>
    {
        private readonly IRepositoryAsync<Bank> _repositoryAsync;
        private readonly IMapper _mapper;

        public UpdateBankCommandHandler(IRepositoryAsync<Bank> repositoryAsync,
            IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<BankDto>> Handle(UpdateBankCommand request, CancellationToken cancellationToken)
        {
            var checkBank = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkBank == null)
            {
                throw new KeyNotFoundException($"No se encontro el banco con id {request.Id}");
            }

            checkBank.Name = request.Name;
            checkBank.ItIsNationalBank = request.ItIsNationBank;
            checkBank.IsActive = request.IsActive;

            await _repositoryAsync.UpdateAsync(checkBank);
            await _repositoryAsync.SaveChangesAsync();

            var dto = _mapper.Map<BankDto>(checkBank);
            return new Response<BankDto>(dto, $"{request.Name} actualizado correctamente");
        }
    }
}
