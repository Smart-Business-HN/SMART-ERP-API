using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.Provider;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.ProviderSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProviderFeature.Commands.UpdateProviderCommand
{
    public class UpdateProviderCommand : IRequest<Response<ProviderDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string RTN { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? ContactPerson { get; set; }
        public string? ContactPhoneNumber { get; set; }
        public string? ContactEmail { get; set; }
        public string Address { get; set; } = null!;
        public string? WebsiteUrl { get; set; }
        public bool IsActive { get; set; }
        public int TypeProviderId { get; set; }
    }

    public class UpdateProviderCommandHandler : IRequestHandler<UpdateProviderCommand, Response<ProviderDto>>
    {
        private readonly IRepositoryAsync<Provider> _repositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly IOutputCacheStore _outputCacheStored;
        private readonly IRepositoryAsync<TypeProvider> _typeProviderRepositoryAsync;

        public UpdateProviderCommandHandler(IMapper mapper, IRepositoryAsync<Provider> repositoryAsync, 
            IJwtService jwtService, IOutputCacheStore outputCacheStored, IRepositoryAsync<TypeProvider> typeProviderRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
            _mapper = mapper;
            _outputCacheStored = outputCacheStored;
            _typeProviderRepositoryAsync = typeProviderRepositoryAsync;
        }
        public async Task<Response<ProviderDto>> Handle(UpdateProviderCommand request, CancellationToken cancellationToken)
        {
            var provider = await _repositoryAsync.FirstOrDefaultAsync( new FilterProviderSpecification(null,request.Id));
            if (provider == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            var checkIfExistByName = await _repositoryAsync.FirstOrDefaultAsync(new FilterProviderSpecification(request.Name, null));
            if (checkIfExistByName != null && checkIfExistByName.Id != provider.Id)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }
            var checkIfExistByEmail = await _repositoryAsync.FirstOrDefaultAsync( new FilterProviderSpecification(request.Email, null));
            if (checkIfExistByEmail != null && provider.Id != checkIfExistByEmail.Id)
            {
                throw new ApiException($"Ya existe un registro con el correo {request.Email}");
            }
            var checkIfExistByRTN = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterProviderSpecification(request.RTN, null));
            if (checkIfExistByRTN != null && provider.Id != checkIfExistByRTN.Id)
            {
                throw new ApiException($"Ya existe un registro con el RTN {request.RTN}");
            }
            var checkIfExistByPhoneNumber = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterProviderSpecification(request.PhoneNumber, null));
            if (checkIfExistByPhoneNumber != null && provider.Id != checkIfExistByPhoneNumber.Id)
            {
                throw new ApiException($"Ya existe un registro con el numero de telefono {request.PhoneNumber}");
            }
            var checkIfExistTypeProvider = await _typeProviderRepositoryAsync.GetByIdAsync(request.TypeProviderId);
            if (checkIfExistTypeProvider == null)
            {
                throw new ApiException($"No existe un tipo de proveedor con el id {request.TypeProviderId}");
            }

            provider.Name = request.Name;
            provider.Email = request.Email;
            provider.RTN = request.RTN;
            provider.PhoneNumber = request.PhoneNumber;
            provider.ContactEmail = request.ContactEmail;
            provider.ContactPerson = request.ContactPerson;
            provider.ContactPhoneNumber = request.ContactPhoneNumber;
            provider.WebsiteUrl = request.WebsiteUrl;
            provider.Address = request.Address;
            provider.IsActive = request.IsActive;
            provider.ModificationDate = DateTime.Now;
            provider.ModificatedBy = _jwtService.GetSubjectToken();
            await _repositoryAsync.UpdateAsync(provider);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_providers", cancellationToken);
            var dto = _mapper.Map<ProviderDto>(provider);
            return new Response<ProviderDto>(dto, message: $"{provider.Name} actualizado correctamente");
        }
    }
}
