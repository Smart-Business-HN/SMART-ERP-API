using AutoMapper;
using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Company;

namespace SMART.ERP.Application.Features.CompanyFeature.Commands.CreateCompanyCommand
{
    public class CreateCompanyCommand : IRequest<Response<CompanyDto>>
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, Response<CompanyDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Company> _repositoryAsync;
        private readonly IJwtService _jwtService;

        public CreateCompanyCommandHandler(IMapper mapper, IRepositoryAsync<Company> repositoryAsync,
            IJwtService jwtService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
        }

        public async Task<Response<CompanyDto>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
            if (await _repositoryAsync.AnyAsync())
            {
                throw new ApiException($"Ya existe una compañia");
            }
            var newRecord = _mapper.Map<Company>(request);
            newRecord.CreatedBy = _jwtService.GetSubjectToken();
            newRecord.CreationDate = DateTime.Now;

            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<CompanyDto>(data);
            return new Response<CompanyDto>(dto, message: $"{request.Name} creado exitosamente");
        }
    }
}
