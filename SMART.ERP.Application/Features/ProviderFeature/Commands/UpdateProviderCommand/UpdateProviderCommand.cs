using AutoMapper;
using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.ProviderSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Product;

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
    }

    public class UpdateProviderCommandHandler : IRequestHandler<UpdateProviderCommand, Response<ProviderDto>>
    {
        private readonly IRepositoryAsync<Provider> _repositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;

        public UpdateProviderCommandHandler(IMapper mapper, IRepositoryAsync<Provider> repositoryAsync,
            IJwtService jwtService)
        {
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
            _mapper = mapper;
        }
        public async Task<Response<ProviderDto>> Handle(UpdateProviderCommand request, CancellationToken cancellationToken)
        {
            var dataSheet = await _repositoryAsync.GetByIdAsync(request.Id);
            if (dataSheet == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            var checkIfExistByName = await _repositoryAsync.FirstOrDefaultAsync(new FilterProviderSpecification(request.Name, request.Id));
            if (checkIfExistByName != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }
            var checkIfExistByEmail = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterProviderSpecification(request.Email, request.Id));
            if (checkIfExistByEmail != null)
            {
                throw new ApiException($"Ya existe un registro con el correo {request.Email}");
            }
            var checkIfExistByRTN = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterProviderSpecification(request.RTN, request.Id));
            if (checkIfExistByRTN != null)
            {
                throw new ApiException($"Ya existe un registro con el RTN {request.RTN}");
            }
            var checkIfExistByPhoneNumber = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterProviderSpecification(request.PhoneNumber, request.Id));
            if (checkIfExistByPhoneNumber != null)
            {
                throw new ApiException($"Ya existe un registro con el numero de telefono {request.PhoneNumber}");
            }

            dataSheet.Name = request.Name;
            dataSheet.Email = request.Email;
            dataSheet.RTN = request.RTN;
            dataSheet.PhoneNumber = request.PhoneNumber;
            dataSheet.ContactEmail = request.ContactEmail;
            dataSheet.ContactPerson = request.ContactPerson;
            dataSheet.ContactPhoneNumber = request.ContactPhoneNumber;
            dataSheet.WebsiteUrl = request.WebsiteUrl;
            dataSheet.Address = request.Address;
            dataSheet.IsActive = request.IsActive;
            dataSheet.ModificationDate = DateTime.Now;
            dataSheet.ModificatedBy = _jwtService.GetSubjectToken();
            await _repositoryAsync.UpdateAsync(dataSheet);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<ProviderDto>(dataSheet);
            return new Response<ProviderDto>(dto, message: $"{dataSheet.Name} actualizado correctamente");
        }
    }
}
