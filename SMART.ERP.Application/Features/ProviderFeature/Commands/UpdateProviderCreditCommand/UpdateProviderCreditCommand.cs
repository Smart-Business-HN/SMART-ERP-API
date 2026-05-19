using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.Provider;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.ProviderSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProviderFeature.Commands.UpdateProviderCreditCommand
{
    public class UpdateProviderCreditCommand : IRequest<Response<ProviderDto>>
    {
        public int Id { get; set; }
        public bool CreditEnabled { get; set; }
        public decimal CreditLimit { get; set; }
    }

    public class UpdateProviderCreditCommandHandler : IRequestHandler<UpdateProviderCreditCommand, Response<ProviderDto>>
    {
        private readonly IRepositoryAsync<Provider> _repositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly IOutputCacheStore _outputCacheStored;

        public UpdateProviderCreditCommandHandler(
            IMapper mapper,
            IRepositoryAsync<Provider> repositoryAsync,
            IJwtService jwtService,
            IOutputCacheStore outputCacheStored)
        {
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
            _mapper = mapper;
            _outputCacheStored = outputCacheStored;
        }

        public async Task<Response<ProviderDto>> Handle(UpdateProviderCreditCommand request, CancellationToken cancellationToken)
        {
            var provider = await _repositoryAsync.FirstOrDefaultAsync(new FilterProviderSpecification(null, request.Id), cancellationToken);
            if (provider == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }

            provider.CreditEnabled = request.CreditEnabled;
            provider.CreditLimit = request.CreditLimit;
            provider.ModificationDate = DateTime.Now;
            provider.ModificatedBy = _jwtService.GetSubjectToken();

            await _repositoryAsync.UpdateAsync(provider, cancellationToken);
            await _repositoryAsync.SaveChangesAsync(cancellationToken);
            await _outputCacheStored.EvictByTagAsync("cache_providers", cancellationToken);

            var dto = _mapper.Map<ProviderDto>(provider);
            return new Response<ProviderDto>(dto, "Crédito del proveedor actualizado correctamente");
        }
    }
}
