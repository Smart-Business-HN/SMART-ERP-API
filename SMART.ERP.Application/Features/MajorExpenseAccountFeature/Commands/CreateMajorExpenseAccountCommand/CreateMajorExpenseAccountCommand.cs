using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.Bank;
using SMART.ERP.Application.DTOs.MajorExpenseAccount;
using SMART.ERP.Application.Exceptions;
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

namespace SMART.ERP.Application.Features.MajorExpenseAccountFeature.Commands.CreateMajorExpenseAccountCommand
{
    public class CreateMajorExpenseAccountCommand : IRequest<Response<MajorExpenseAccountDto>>
    {
        public string Name { get; set; } = null!;
    }
    public class CreateMajorExpenseAccountCommandHandler : IRequestHandler<CreateMajorExpenseAccountCommand, Response<MajorExpenseAccountDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<MajorExpenseAccount> _repositoryAsync;
        private readonly IOutputCacheStore _outputCacheStored;
        public CreateMajorExpenseAccountCommandHandler(IMapper mapper, IOutputCacheStore outputCacheStore, IRepositoryAsync<MajorExpenseAccount> repositoryAsync)
        {
            _mapper = mapper;
            _outputCacheStored = outputCacheStore;
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<MajorExpenseAccountDto>> Handle(CreateMajorExpenseAccountCommand request, CancellationToken cancellationToken)
        {
            var checkbyName = await _repositoryAsync.FirstOrDefaultAsync(new FilterMajorExpenseAccountFromNameSpecification(request.Name));
            if (checkbyName != null)
            {
                throw new ApiException($"Ya existe una cuenta con el nombre {request.Name}");
            }
            var newRecord = _mapper.Map<MajorExpenseAccount>(request);
            var response = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_major_expense_account", cancellationToken);
            var dto = _mapper.Map<MajorExpenseAccountDto>(response);
            return new Response<MajorExpenseAccountDto>(dto, $"{request.Name} agregado correctamente");
        }
    }
}
