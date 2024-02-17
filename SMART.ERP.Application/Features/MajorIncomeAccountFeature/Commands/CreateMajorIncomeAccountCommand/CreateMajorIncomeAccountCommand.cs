using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.MajorIncomeAccount;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.MajorIncomeAccountSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.MajorIncomeAccountFeature.Commands.CreateMajorIncomeAccountCommand
{
    public class CreateMajorIncomeAccountCommand : IRequest<Response<MajorIncomeAccountDto>>
    {
        public string Name { get; set; } = null!;
    }
    public class CreateMajorIncomeAccountCommandHandler : IRequestHandler<CreateMajorIncomeAccountCommand, Response<MajorIncomeAccountDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<MajorIncomeAccount> _repositoryAsync;
        private readonly IOutputCacheStore _outputCacheStored;
        public CreateMajorIncomeAccountCommandHandler(IMapper mapper, IOutputCacheStore outputCacheStore, IRepositoryAsync<MajorIncomeAccount> repositoryAsync)
        {
            _mapper = mapper;
            _outputCacheStored = outputCacheStore;
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<MajorIncomeAccountDto>> Handle(CreateMajorIncomeAccountCommand request, CancellationToken cancellationToken)
        {
            var checkbyName = await _repositoryAsync.FirstOrDefaultAsync(new FilterMajorIncomeAccountFromNameSpecification(request.Name));
            if (checkbyName != null)
            {
                throw new ApiException($"Ya existe una cuenta con el nombre {request.Name}");
            }
            var newRecord = _mapper.Map<MajorIncomeAccount>(request);
            var response = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_major_income_account", cancellationToken);
            var dto = _mapper.Map<MajorIncomeAccountDto>(response);
            return new Response<MajorIncomeAccountDto>(dto, $"{request.Name} agregada correctamente");
        }
    }
}
