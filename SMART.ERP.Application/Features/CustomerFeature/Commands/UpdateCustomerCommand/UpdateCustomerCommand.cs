using AutoMapper;
using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ClientSpecification;
using SMART.ERP.Application.Specifications.CustomerSpecification;
using SMART.ERP.Application.Specifications.ProspectSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Customer;

namespace SMART.ERP.Application.Features.CustomerFeature.Commands.UpdateCustomerCommand
{
    public class UpdateCustomerCommand : IRequest<Response<CustomerDto>>
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }
        public string? DNI { get; set; }
        public string? RTN { get; set; }
        public string? Company { get; set; }
        public DateTime? ConstitutionDate { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Email { get; set; }
        public bool ConfirmedEmail { get; set; }
        public string? SecondaryEmail { get; set; }
        public string? PhoneNumber { get; set; }
        public bool ConfirmedPhoneNumber { get; set; }
        public string? SecondaryPhoneNumber { get; set; }
        public string? CivilStatus { get; set; }
        public int GenderId { get; set; }
        public int CurrencyId { get; set; }
        public int SocialReasonId { get; set; }
        public int HeadingId { get; set; }
        public int? DepartmentId { get; set; }
        public int? CountryId { get; set; }
        public bool IsHisOwnContactPerson { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactPersonPhone { get; set; }
        public string? ContactPersonEmail { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Response<CustomerDto>>
    {
        private readonly IRepositoryAsync<Customer> _repositoryHNAsync;
        private readonly IRepositoryAsync<Customer> _customerRepositoryAsync;
        private readonly IRepositoryAsync<Gender> _genderRepositoryHNAsync;
        private readonly IRepositoryAsync<Heading> _headingRepositoryHNAsync;
        private readonly IRepositoryAsync<SocialReason> _socialReasonRepositoryHNAsync;
        private readonly IRepositoryAsync<Currency> _currencyRepositoryHNAsync;
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Prospect> _prospectRepositoryAsync;

        public UpdateCustomerCommandHandler(IRepositoryAsync<Customer> repositoryHNAsync, IRepositoryAsync<Customer> customerRepositoryAsync,
            IRepositoryAsync<Gender> genderRepositoryHNAsync, IRepositoryAsync<Heading> headingRepositoryHNAsync,
            IRepositoryAsync<SocialReason> socialReasonRepositoryHNAsync, IRepositoryAsync<Currency> currencyRepositoryHNAsync, IMapper mapper,
            IRepositoryAsync<Prospect> prospectRepositoryAsync)
        {
            _repositoryHNAsync = repositoryHNAsync;
            _customerRepositoryAsync = customerRepositoryAsync;
            _genderRepositoryHNAsync = genderRepositoryHNAsync;
            _headingRepositoryHNAsync = headingRepositoryHNAsync;
            _socialReasonRepositoryHNAsync = socialReasonRepositoryHNAsync;
            _currencyRepositoryHNAsync = currencyRepositoryHNAsync;
            _mapper = mapper;
            _prospectRepositoryAsync = prospectRepositoryAsync;
        }

        public async Task<Response<CustomerDto>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            var checkIfLocalCustomerExist = await _customerRepositoryAsync.FirstOrDefaultAsync(new FilterCustomerByMasterIdSpecification(request.Id));
            if (checkIfLocalCustomerExist == null)
            {
                throw new KeyNotFoundException($"Este cliente no se encuentra registrado");
            }

            var checkIfExist = await _repositoryHNAsync.GetByIdAsync(request.Id);
            if (checkIfExist == null)
            {
                throw new KeyNotFoundException($"No se encontro el registro con id {request.Id}");
            }

            var checkIfHeadingExist = await _headingRepositoryHNAsync.GetByIdAsync(request.HeadingId);
            if (checkIfHeadingExist == null)
            {
                throw new KeyNotFoundException($"No se encontro el rubro con id {request.HeadingId}");
            }

            if (request.CurrencyId > 0)
            {
                var checkIfCurrencyExist = await _currencyRepositoryHNAsync.GetByIdAsync(request.CurrencyId);
                if (checkIfCurrencyExist == null)
                {
                    throw new KeyNotFoundException($"No se encontro la moneda con el id {request.CurrencyId}");
                }
                checkIfExist.CurrencyId = request.CurrencyId;
            }

            if (request.GenderId > 0)
            {
                var checkIfGenderExist = await _genderRepositoryHNAsync.GetByIdAsync(request.GenderId);
                if (checkIfGenderExist == null)
                {
                    throw new KeyNotFoundException($"No se encontro el genero con id {request.GenderId}");
                }
                checkIfExist.GenderId = request.GenderId;
            }

            var checkIfSocialReasonExist = await _socialReasonRepositoryHNAsync.GetByIdAsync(request.SocialReasonId);
            if (checkIfSocialReasonExist == null)
            {
                throw new KeyNotFoundException($"No se encontro la razon social con id {request.SocialReasonId}");
            }

            if (checkIfSocialReasonExist.Name == "Natural" && request.FirstName != null && request.LastName != null)
            {
                checkIfExist.Company = null;
                checkIfExist.FullName = request.FirstName + " " + request.LastName;
                checkIfExist.FirstName = request.FirstName;
                checkIfExist.LastName = request.LastName;
            }
            else if (checkIfSocialReasonExist.Name == "Juridica" && request.FullName != null)
            {
                checkIfExist.Company = request.Company;
                checkIfExist.FullName = request.FullName;
                checkIfExist.FirstName = request.FirstName;
                checkIfExist.LastName = request.LastName;
            }

            var checkFields = await _prospectRepositoryAsync.FirstOrDefaultAsync(new FilterProspectByUniqueFieldsSpecification(
                checkIfExist.FullName, request.PhoneNumber, request.Email, null));
            if (checkFields != null && checkFields.ProspectStep.Name != "Convertido" && checkFields.ProspectStep.Name != "No Calificado")
            {
                if (checkFields.FullName == checkIfExist.FullName)
                {
                    throw new ApiException("Ya existe un prospecto con este nombre");
                }
                if (checkFields.Email == request.Email && request.Email != null)
                {
                    throw new ApiException("Ya existe un prospecto con este correo");
                }
                if (checkFields.PhoneNumber == request.PhoneNumber)
                {
                    throw new ApiException("Ya existe un prospecto con este número telefónico");
                }
            }

            var checkClientFields = await _repositoryHNAsync.FirstOrDefaultAsync(new FilterClientByUniqueValues(checkIfExist.FullName, checkIfExist.PhoneNumber, checkIfExist.Email, checkIfExist.Id));
            if (checkClientFields != null)
            {
                if (checkClientFields.FullName.ToLower() == checkIfExist.FullName.ToLower())
                {
                    throw new ApiException("Ya existe un cliente con este nombre");
                }
                if (checkClientFields.Email == request.Email && request.Email != null)
                {
                    throw new ApiException("Ya existe un cliente con este correo");
                }
                if (checkClientFields.PhoneNumber == request.PhoneNumber)
                {
                    throw new ApiException("Ya existe un cliente con este número telefónico");
                }
            }


            if (request.BirthDate != null)
            {
                checkIfExist.Age = DateTime.Now.Year - request.BirthDate.Value.Year;
            }

            checkIfExist.SocialReasonId = request.SocialReasonId;
            checkIfExist.DNI = request.DNI;
            checkIfExist.RTN = request.RTN;
            checkIfExist.ConstitutionDate = request.ConstitutionDate;
            checkIfExist.BirthDate = request.BirthDate;
            checkIfExist.Email = request.Email;
            checkIfExist.ConfirmedEmail = request.ConfirmedEmail;
            checkIfExist.SecondaryEmail = request.SecondaryEmail;
            checkIfExist.PhoneNumber = request.PhoneNumber;
            checkIfExist.ConfirmedPhoneNumber = request.ConfirmedPhoneNumber;
            checkIfExist.SecondaryPhoneNumber = request.SecondaryPhoneNumber;
            checkIfExist.CivilStatus = request.CivilStatus;
            checkIfExist.HeadingId = request.HeadingId;
            checkIfExist.IsHisOwnContactPerson = request.IsHisOwnContactPerson;
            checkIfExist.ContactPerson = request.ContactPerson;
            checkIfExist.ContactPersonPhone = request.ContactPersonPhone;
            checkIfExist.ContactPersonEmail = request.ContactPersonEmail;
            checkIfExist.IsActive = request.IsActive;
            checkIfExist.CountryId = request.CountryId;
            checkIfExist.DepartmentId = request.DepartmentId;
            await _repositoryHNAsync.UpdateAsync(checkIfExist);
            await _repositoryHNAsync.SaveChangesAsync();

            var dto = _mapper.Map<CustomerDto>(checkIfExist);
            return new Response<CustomerDto>(dto, $"{dto.FullName} actualizado correctamente");
        }
    }
}
