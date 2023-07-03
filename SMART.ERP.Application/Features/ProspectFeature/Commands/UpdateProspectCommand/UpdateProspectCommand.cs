using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Prospect;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.ClientSpecification;
using SMART.ERP.Application.Specifications.ProspectSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using System.Text.RegularExpressions;

namespace SMART.ERP.Application.Features.ProspectFeature.Commands.UpdateProspectCommand
{
    public class UpdateProspectCommand : IRequest<Response<ProspectDto>>
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? Email { get; set; }
        public int HeadingId { get; set; }
        public int TypeOriginId { get; set; }
        public int SocialReasonId { get; set; }
        public int? PostalCode { get; set; }
        public string? Address { get; set; }
        public string? Observation { get; set; }
        public string? MetaAdCampaignId { get; set; }
        public string? WebsiteUrl { get; set; }
        public int? CityId { get; set; }
        public int? GenderId { get; set; }
        public string? PreferredContactMethod { get; set; }
        public bool AccountHN { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactPersonPhone { get; set; }
        public string? ContactPersonEmail { get; set; }
    }

    public class UpdateProspectCommandHandler : IRequestHandler<UpdateProspectCommand, Response<ProspectDto>>
    {
        private readonly IRepositoryAsync<Prospect> _repositoryAsync;
        private readonly IRepositoryAsync<TypeOrigin> _originRepositoryAsync;
        private readonly IRepositoryAsync<Heading> _headingRepositoryAsync;
        private readonly IRepositoryAsync<SocialReason> _socialReasonRepositoryAsync;
        private readonly IRepositoryAsync<Gender> _genderRepositoryAsync;
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<City> _cityRepositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IRepositoryAsync<Customer> _repositoryHNAsync;
        private readonly IRepositoryAsync<Country> _countryRepositoryAsync;
        private readonly IRepositoryAsync<MetaAdCampaign> _adCampaignRepositoryAsync;

        public UpdateProspectCommandHandler(IRepositoryAsync<Prospect> repositoryAsync, IRepositoryAsync<TypeOrigin> originRepositoryAsync,
            IRepositoryAsync<Heading> headingRepositoryAsync, IRepositoryAsync<SocialReason> socialReasonRepositoryAsync, IRepositoryAsync<Gender> genderRepositoryAsync,
            IMapper mapper, IRepositoryAsync<City> cityRepositoryAsync, IJwtService jwtService,
            IRepositoryAsync<Customer> repositoryHNAsync, IRepositoryAsync<Country> countryRepositoryAsync,
            IRepositoryAsync<MetaAdCampaign> adCampaignRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _originRepositoryAsync = originRepositoryAsync;
            _headingRepositoryAsync = headingRepositoryAsync;
            _socialReasonRepositoryAsync = socialReasonRepositoryAsync;
            _genderRepositoryAsync = genderRepositoryAsync;
            _mapper = mapper;
            _cityRepositoryAsync = cityRepositoryAsync;
            _jwtService = jwtService;
            _repositoryHNAsync = repositoryHNAsync;
            _countryRepositoryAsync = countryRepositoryAsync;
            _adCampaignRepositoryAsync = adCampaignRepositoryAsync;
        }

        public async Task<Response<ProspectDto>> Handle(UpdateProspectCommand request, CancellationToken cancellationToken)
        {
            var checkProspect = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkProspect == null)
            {
                throw new KeyNotFoundException($"No se encontro el prospecto con Id {request.Id}");
            }
            var checkHeading = await _headingRepositoryAsync.GetByIdAsync(request.HeadingId);
            if (checkHeading == null)
            {
                throw new KeyNotFoundException($"No se encontro el rubro con id {request.HeadingId}");
            }
            var checkSocialReason = await _socialReasonRepositoryAsync.GetByIdAsync(request.SocialReasonId);
            if (checkSocialReason == null)
            {
                throw new KeyNotFoundException($"No se encontro el tipo de cliente con id {request.SocialReasonId}");
            }
            var checkTypeOrigin = await _originRepositoryAsync.GetByIdAsync(request.TypeOriginId);
            if (checkTypeOrigin == null)
            {
                throw new KeyNotFoundException($"No se encontro el origen con id {request.TypeOriginId}");
            }
            if (request.CityId != null)
            {
                var checkCity = await _cityRepositoryAsync.GetByIdAsync((int)request.CityId);
                if (checkCity == null)
                {
                    throw new KeyNotFoundException($"No se encontro la ciudad con id {request.CityId}");
                }
            }
            var checkFields = await _repositoryHNAsync.FirstOrDefaultAsync(new FilterClientByUniqueValues(request.FullName, request.PhoneNumber, request.Email, null));
            if (checkFields != null)
            {
                if (checkFields.FullName == request.FullName)
                {
                    throw new ApiException($"Ya existe un cliente con el nombre {request.FullName}");
                }
                if (checkFields.Email == request.Email && request.Email != null)
                {
                    throw new ApiException($"Ya existe un cliente con el correo {request.Email}");
                }
                if (checkFields.PhoneNumber == request.PhoneNumber)
                {
                    throw new ApiException($"Ya existe un cliente con el número telefónico {request.PhoneNumber}");
                }
            }
            var checkProspectField = await _repositoryAsync.FirstOrDefaultAsync(new FilterProspectByUniqueFieldsSpecification(request.FullName, request.PhoneNumber, request.Email, request.Id));
            if (checkProspectField != null)
            {
                if (checkProspectField.FullName == request.FullName)
                {
                    throw new ApiException($"Ya existe un prospecto con el nombre {request.FullName}");
                }
                if (checkProspectField.Email == request.Email && request.Email != null)
                {
                    throw new ApiException($"Ya existe un prospecto con el correo {request.Email}");
                }
                if (checkProspectField.PhoneNumber == request.PhoneNumber)
                {
                    throw new ApiException($"Ya existe un prospecto con el número telefónico {request.PhoneNumber}");
                }
            }

            var getCountry = await _countryRepositoryAsync.GetByIdAsync(checkProspect.CountryId);
            if (getCountry!.Name != "Honduras")
            {
                bool match = Regex.IsMatch(request.PhoneNumber, "[(][0-9]{3}[)][\\s][0-9]{3}[-][0-9]{4}");
                if (!match)
                {
                    throw new ApiException($"El formato del numero no coincide para el pais {getCountry.Name}.\n Ejemplo: (000) 000-0000");
                }
            }
            else
            {
                bool match = Regex.IsMatch(request.PhoneNumber, "[0-9]{4}-[0-9]{4}");
                if (!match)
                {
                    throw new ApiException($"El formato del numero no coincide para el pais {getCountry.Name}.\n Ejemplo: 0000-0000");
                }
            }

            checkProspect.FullName = request.FullName;
            checkProspect.Email = request.Email;
            checkProspect.PhoneNumber = request.PhoneNumber;
            checkProspect.HeadingName = checkHeading.Name;
            checkProspect.SocialReasonName = checkSocialReason.Name;
            checkProspect.TypeOriginId = checkTypeOrigin.Id;
            checkProspect.PostalCode = request.PostalCode == 0 || request.PostalCode == null ? null : request.PostalCode;
            checkProspect.WebsiteUrl = request.WebsiteUrl;
            checkProspect.CityId = request.CityId;
            checkProspect.Address = request.Address;
            checkProspect.Observation = request.Observation;
            checkProspect.PreferredContactMethod = request.PreferredContactMethod;
            checkProspect.AccountHN = request.AccountHN;
            checkProspect.ModificationDate = DateTime.Now;
            checkProspect.ModificatedBy = _jwtService.GetSubjectToken();
            if (request.GenderId != null)
            {
                var checkGender = await _genderRepositoryAsync.GetByIdAsync((int)request.GenderId);
                if (checkGender == null)
                {
                    throw new KeyNotFoundException($"No se encontro el genero con id {request.GenderId}");
                }
                checkProspect.GenderId = checkGender.Id;
            }
            if (!string.IsNullOrEmpty(request.MetaAdCampaignId))
            {
                var checkAd = await _adCampaignRepositoryAsync.GetByIdAsync(request.MetaAdCampaignId);
                if (checkAd == null)
                {
                    throw new KeyNotFoundException($"No se encontro la camapaña con id {request.MetaAdCampaignId}");
                }
                checkProspect.MetaAdCampaignId = checkAd.Id;
            }

            await _repositoryAsync.UpdateAsync(checkProspect);
            await _repositoryAsync.SaveChangesAsync();

            var dto = _mapper.Map<ProspectDto>(checkProspect);
            return new Response<ProspectDto>(dto, $"{checkProspect.FullName} Actualizado correctamente");
        }
    }
}
