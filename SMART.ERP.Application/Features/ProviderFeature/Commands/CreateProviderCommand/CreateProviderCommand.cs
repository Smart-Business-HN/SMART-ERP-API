using AutoMapper;
using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.ProviderSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Product;

namespace SMART.ERP.Application.Features.ProviderFeature.Commands.CreateProviderCommand
{
    public class CreateProviderCommand : IRequest<Response<ProviderDto>>
    {
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

    public class CreateProviderCommandHandler : IRequestHandler<CreateProviderCommand, Response<ProviderDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Provider> _repositoryAsync;
        private readonly IJwtService _jwtService;

        public CreateProviderCommandHandler(IMapper mapper, IRepositoryAsync<Provider> repositoryAsync,
            IJwtService jwtService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
        }

        public async Task<Response<ProviderDto>> Handle(CreateProviderCommand request, CancellationToken cancellationToken)
        {
            var checkIfExistByName = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterProviderSpecification(request.Name, null));
            if (checkIfExistByName != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }
            var checkIfExistByEmail = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterProviderSpecification(request.Email, null));
            if (checkIfExistByEmail != null)
            {
                throw new ApiException($"Ya existe un registro con el correo {request.Email}");
            }
            var checkIfExistByRTN = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterProviderSpecification(request.RTN, null));
            if (checkIfExistByRTN != null)
            {
                throw new ApiException($"Ya existe un registro con el RTN {request.RTN}");
            }
            var checkIfExistByPhoneNumber = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterProviderSpecification(request.PhoneNumber, null));
            if (checkIfExistByPhoneNumber != null)
            {
                throw new ApiException($"Ya existe un registro con el numero de telefono {request.PhoneNumber}");
            }

            var newRecord = _mapper.Map<Provider>(request);
            newRecord.CreatedBy = _jwtService.GetSubjectToken();
            newRecord.CreationDate = DateTime.Now;

            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<ProviderDto>(data);

            return new Response<ProviderDto>(dto, message: $"{request.Name} creado exitosamente");
        }
    }

}
