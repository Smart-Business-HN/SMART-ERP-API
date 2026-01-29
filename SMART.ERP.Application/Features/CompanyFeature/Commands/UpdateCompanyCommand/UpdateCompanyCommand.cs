using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Company;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CompanyFeature.Commands.UpdateCompanyCommand
{
    public class UpdateCompanyCommand : IRequest<Response<CompanyDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string AboutUs { get; set; } = null!;
        public string? FacebookUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? YoutubeUrl { get; set; }
        public int? CaiId { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, Response<CompanyDto>>
    {
        private readonly IRepositoryAsync<Company> _repositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;

        public UpdateCompanyCommandHandler(IMapper mapper, IRepositoryAsync<Company> repositoryAsync,
            IJwtService jwtService)
        {
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
            _mapper = mapper;
        }
        public async Task<Response<CompanyDto>> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
        {
            var company = await _repositoryAsync.GetByIdAsync(request.Id);
            if (company == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            if(request.CaiId != null)
            {
                company.CaiId = request.CaiId;
            }
            company.Name = request.Name;
            company.IsActive = request.IsActive;
            company.Email = request.Email;
            company.PhoneNumber = request.PhoneNumber;
            company.FacebookUrl = request.FacebookUrl;
            company.TwitterUrl = request.TwitterUrl;
            company.YoutubeUrl = request.YoutubeUrl;
            company.InstagramUrl = request.InstagramUrl;
            company.Address = request.Address;
            company.AboutUs = request.AboutUs;
            company.ModificationDate = DateTime.Now;
            company.ModificatedBy = _jwtService.GetSubjectToken();
            await _repositoryAsync.UpdateAsync(company);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<CompanyDto>(company);
            return new Response<CompanyDto>(dto, message: $"{company.Name} actualizado correctamente");
        }
    }
}
