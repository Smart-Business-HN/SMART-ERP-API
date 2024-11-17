using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Prospect;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Services.NewEncryptionService;
using SMART.ERP.Application.Specifications.ClientCountrySpecification;
using SMART.ERP.Application.Specifications.ClientSpecification;
using SMART.ERP.Application.Specifications.CountrySpecification;
using SMART.ERP.Application.Specifications.CustomerSpecification;
using SMART.ERP.Application.Specifications.InterestLevelSpecification;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Specifications.OpportunityStepSpecification;
using SMART.ERP.Application.Specifications.ProspectQuoteProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;
using SMART.ERP.Application.Services.RegisterClientService;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SMART.ERP.Application.Features.ProspectFeature.Commands.ConvertProspectCommand
{
    public class ConvertProspectCommand : IRequest<Response<ProspectDto>>
    {
        public Guid Id { get; set; }
    }

    public class ConvertProspectCommandHandler : IRequestHandler<ConvertProspectCommand, Response<ProspectDto>>
    {
        private readonly IRepositoryAsync<Prospect> _repositoryAsync;
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<ProspectStep> _prospectStepRepositoryAsync;
        private readonly IRepositoryAsync<OpportunityStep> _stepRepositoryAsync;
        private readonly IRepositoryAsync<ProspectQuoteProduct> _prospectQuoteRepositoryAsync;
        private readonly IRepositoryAsync<Heading> _headingRepositoryAsync;
        private readonly IRepositoryAsync<SocialReason> _reasonRepositoryAsync;
        private readonly IRepositoryAsync<TypeOrigin> _originRepositoryAsync;
        private readonly IRepositoryAsync<CustomerType> _clientTypeRepositoryAsync;
        private readonly IRepositoryAsync<Customer> _clientRepositoryAsync;
        private readonly IRegisterClientService _registerClientService;
        private readonly IRepositoryAsync<Opportunity> _opportunityRepositoryAsync;
        private readonly INewEncryptionService _newEncryptionService;
        private readonly IRepositoryAsync<Customer> _customerRepositoryAsync;
        private readonly IRepositoryAsync<InterestLevel> _interestRepositoryAsync;
        private readonly IRepositoryAsync<Product> _productRepositoryAsync;
        private readonly IRepositoryAsync<QuoteProduct> _quoteRepositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IRepositoryAsync<Country> _countryRepositoryAsync;
        private readonly IRepositoryAsync<Country> _clientCountryHNAsync;

        public ConvertProspectCommandHandler(IRepositoryAsync<Prospect> repositoryAsync,
            IMapper mapper,
            IRepositoryAsync<ProspectStep> prospectStepRepositoryAsync,
            IRepositoryAsync<OpportunityStep> stepRepositoryAsync,
            IRepositoryAsync<ProspectQuoteProduct> prospectQuoteRepositoryAsync,
            IRepositoryAsync<Heading> headingRepositoryAsync,
            IRepositoryAsync<SocialReason> reasonRepositoryAsync,
            IRepositoryAsync<TypeOrigin> originRepositoryAsync,
            IRepositoryAsync<CustomerType> clientTypeRepositoryAsync,
            IRepositoryAsync<Customer> clientRepositoryAsync,
            IRegisterClientService registerClientService,
            IRepositoryAsync<Opportunity> opportunityRepositoryAsync,
            INewEncryptionService newEncryptionService,
            IRepositoryAsync<Customer> customerRepositoryAsync,
            IRepositoryAsync<InterestLevel> interestRepositoryAsync,
            IRepositoryAsync<Product> productRepositoryAsync,
            IRepositoryAsync<QuoteProduct> quoteRepositoryAsync,
            IJwtService jwtService,
            IRepositoryAsync<Country> countryRepositoryAsync,
            IRepositoryAsync<Country> clientCountryHNAsync)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
            _prospectStepRepositoryAsync = prospectStepRepositoryAsync;
            _stepRepositoryAsync = stepRepositoryAsync;
            _prospectQuoteRepositoryAsync = prospectQuoteRepositoryAsync;
            _headingRepositoryAsync = headingRepositoryAsync;
            _reasonRepositoryAsync = reasonRepositoryAsync;
            _originRepositoryAsync = originRepositoryAsync;
            _clientTypeRepositoryAsync = clientTypeRepositoryAsync;
            _clientRepositoryAsync = clientRepositoryAsync;
            _registerClientService = registerClientService;
            _opportunityRepositoryAsync = opportunityRepositoryAsync;
            _newEncryptionService = newEncryptionService;
            _customerRepositoryAsync = customerRepositoryAsync;
            _interestRepositoryAsync = interestRepositoryAsync;
            _productRepositoryAsync = productRepositoryAsync;
            _quoteRepositoryAsync = quoteRepositoryAsync;
            _jwtService = jwtService;
            _countryRepositoryAsync = countryRepositoryAsync;
            _clientCountryHNAsync = clientCountryHNAsync;
        }

        public async Task<Response<ProspectDto>> Handle(ConvertProspectCommand request, CancellationToken cancellationToken)
        {
            var checkProspect = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkProspect == null)
            {
                throw new KeyNotFoundException($"No se encontro el prospecto con id {request.Id}");
            }
            if (checkProspect.CityId == null)
            {
                throw new ApiException("Debe asignar una ciudad al prospecto.");
            }
            if (checkProspect.Address == null)
            {
                throw new ApiException("Debe asignar una direccion al prospecto.");
            }
            var prospectSteps = await _prospectStepRepositoryAsync.ListAsync();
            var checkProspectStep = prospectSteps.FirstOrDefault(x => x.Id == checkProspect.ProspectStepId);
            if (checkProspectStep!.Name == "Convertido" || checkProspectStep.Name == "No Calificado")
            {
                throw new ApiException("No es posible convertir este prospecto.");
            }
            checkProspect.ProspectStepId = prospectSteps.Find(x => x.Name == "Convertido")!.Id;
            checkProspect.ModificationDate = DateTime.Now;
            checkProspect.ModificatedBy = _jwtService.GetSubjectToken();
            var checkFields = await _clientRepositoryAsync.FirstOrDefaultAsync(new FilterClientByUniqueValues(checkProspect.FullName, checkProspect.PhoneNumber, checkProspect.Email, null));
            if (checkFields != null)
            {
                if (checkFields.FullName == checkProspect.FullName)
                {
                    throw new ApiException("Ya existe un cliente con este nombre");
                }
                if (checkFields.Email == checkProspect.Email)
                {
                    throw new ApiException("Ya existe un cliente con este correo");
                }
                if (checkFields.PhoneNumber == checkProspect.PhoneNumber)
                {
                    throw new ApiException("Ya existe un cliente con este número telefónico");
                }
            }
            var newClient = new Customer();
            var getMotorsCountry = await _countryRepositoryAsync.FirstOrDefaultAsync(new
                IncludeCountrySpecification(checkProspect.CountryId));
            var prospectRegion = getMotorsCountry!.Regions!.Find(x => x.Departments!.Any(y => y.Id == checkProspect.DepartmentId));
            if (prospectRegion != null)
            {
                throw new KeyNotFoundException($"El pais {getMotorsCountry.Name} no contiene el departamento del prospecto.");
            }
            var prospectDepartment = prospectRegion!.Departments!.Find(x => x.Id == checkProspect.DepartmentId);
            var getHNCountries = await _clientCountryHNAsync.ListAsync(new
                ClientCountryIncludesSpecification());
            var getHNCountry = getHNCountries.Find(x => x.Name == getMotorsCountry!.Name);
            if (getHNCountry == null)
            {
                throw new KeyNotFoundException($"No se encontro el pais {getMotorsCountry.Name} en Platino HN");
            }
            var getDepartment = getHNCountry.Departments.Find(x => x.Name == prospectDepartment!.Name);
            var headings = await _headingRepositoryAsync.ListAsync();
            var socialReasons = await _reasonRepositoryAsync.ListAsync();
            var origins = await _originRepositoryAsync.ListAsync();
            var clientTypes = await _clientTypeRepositoryAsync.ListAsync();
            if (checkProspect.SocialReasonName == "Natural")
            {
                newClient.FullName = checkProspect.FullName;
                var splittedName = checkProspect.FullName.Split(" ");
                if (splittedName.Length % 2 == 0)
                {
                    var firstName = splittedName.Take(splittedName.Length / 2);
                    var lastName = splittedName.Skip(splittedName.Length / 2);
                    newClient.FirstName = string.Join(" ", firstName);
                    newClient.LastName = string.Join(" ", lastName);
                }
                else
                {
                    int ceiling = (int)Math.Round(Math.Ceiling((decimal)splittedName.Length / 2), MidpointRounding.ToEven);
                    var firstName = splittedName.Take(ceiling);
                    var lastName = splittedName.Skip(ceiling);
                    newClient.FirstName = string.Join(" ", firstName);
                    newClient.LastName = string.Join(" ", lastName);
                }
                newClient.PhoneNumber = checkProspect.PhoneNumber;
                newClient.Email = checkProspect.Email;
                newClient.HeadingId = headings.Find(x => x.Name == checkProspect.HeadingName)!.Id;
                newClient.SocialReasonId = socialReasons.Find(x => x.Name == checkProspect.SocialReasonName)!.Id;
            }
            else
            {
                newClient.FullName = checkProspect.FullName;
                newClient.Company = checkProspect.FullName;
                newClient.PhoneNumber = checkProspect.PhoneNumber;
                newClient.ContactPerson = checkProspect.ContactPerson;
                newClient.ContactPersonEmail = checkProspect.ContactPersonEmail;
                newClient.ContactPersonPhone = checkProspect.ContactPersonPhone;
                newClient.Email = checkProspect.Email;
                newClient.HeadingId = headings.Find(x => x.Name == checkProspect.HeadingName)!.Id;
                newClient.SocialReasonId = socialReasons.Find(x => x.Name == checkProspect.SocialReasonName)!.Id;
            }
            newClient.IsActive = checkProspect.AccountHN;
            newClient.CurrencyId = null;
            newClient.CountryId = getHNCountry.Id;
            newClient.DepartmentId = getDepartment!.Id;
            newClient.CustomerTypeId = clientTypes.Find(x => x.Name == "Basico")!.Id;
            newClient.GenderId = null;
            byte[] passwordHash, passwordSalt;
            string tempPassword = RandomString(8);
            _newEncryptionService.CreatePasswordHash(tempPassword, out passwordHash, out passwordSalt);
            newClient.PasswordHash = passwordHash;
            newClient.PasswordSalt = passwordSalt;
            var response = await _clientRepositoryAsync.AddAsync(newClient);
            await _clientRepositoryAsync.SaveChangesAsync();

            bool suceeded = await _registerClientService.RegiterClient(response.Id, checkProspect.UserId);
            if (suceeded == false)
            {
                throw new Exception("Ocurrio un error al tratar de registrar este cliente en Platino Motors");
            }

            var motorCustomer = await _customerRepositoryAsync.FirstOrDefaultAsync(new FilterCustomerByMasterIdSpecification(response.Id));
            var identificationStep = await _stepRepositoryAsync.FirstOrDefaultAsync(new FilterOpportunityStepSpecification("Identificacion", null));
            var lowInterest = await _interestRepositoryAsync.FirstOrDefaultAsync(new FilterInterestLevelSpecification("Bajo", null));
            var newOpportunity = new Opportunity
            {
                Code = await GenerateCode(DateTime.Now),
                CustomerId = motorCustomer!.Id,
                CreationDate = DateTime.Now,
                UserId = checkProspect.UserId,
                OpportunityStepId = identificationStep!.Id,
                Position = 1,
                Budget = 0,
                Total = 0,
                QtyItems = 0,
                ProbabilityPercentage = 0,
                TypeOriginId = checkProspect.TypeOriginId,
                InterestLevelId = lowInterest!.Id
            };

            var identificationContainer = await _opportunityRepositoryAsync.ListAsync(new FilterOpportunityByStepSpecification(identificationStep.Id, null, null, 1, checkProspect.UserId));
            if (identificationContainer.Count > 0)
            {
                foreach (var item in identificationContainer)
                {
                    item.Position += 1;
                }
                await _opportunityRepositoryAsync.UpdateRangeAsync(identificationContainer);
                await _opportunityRepositoryAsync.SaveChangesAsync();
            }

            var opportunityResponse = await _opportunityRepositoryAsync.AddAsync(newOpportunity);
            await _opportunityRepositoryAsync.SaveChangesAsync();

            var listProducts = await _prospectQuoteRepositoryAsync.ListAsync(new FilterProspectQuoteProductByProspectIdSpecification(checkProspect.Id));
            if (listProducts.Count > 0)
            {
                var productList = await _productRepositoryAsync.ListAsync();
                var newQuoteList = new List<QuoteProduct>();
                foreach (var quote in listProducts)
                {
                    var newQuote = new QuoteProduct
                    {
                        ProductId = quote.ProductId,
                        SalePrice = productList.Find(x => x.Id == quote.ProductId)!.RecomendedSalePrice,
                        OpportunityId = opportunityResponse.Id,
                        IsActive = true,
                        Quantity = 1
                    };
                    newQuoteList.Add(newQuote);
                    opportunityResponse.Total += productList.Find(x => x.Id == quote.ProductId)!.RecomendedSalePrice;
                }
                opportunityResponse.QtyItems = listProducts.Count;
                await _quoteRepositoryAsync.AddRangeAsync(newQuoteList);
                await _quoteRepositoryAsync.SaveChangesAsync();

                await _opportunityRepositoryAsync.UpdateAsync(opportunityResponse);
                await _opportunityRepositoryAsync.SaveChangesAsync();
            }
            int opportunityId = opportunityResponse.Id;
            await _repositoryAsync.UpdateAsync(checkProspect);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<ProspectDto>(checkProspect);
            dto.OpportunityId = opportunityId;
            return new Response<ProspectDto>(dto, "Actualizado correctamente");
        }

        static string RandomString(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ12345678901234567890123456789012345678901234";
            StringBuilder res = new StringBuilder();
            while (length-- > 0)
            {
                var rng = RandomNumberGenerator.GetInt32(95);
                res.Append(valid[rng]);
            }
            if (Regex.Match(res.ToString(), @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$").Success)
            {
                return res.ToString();
            }
            else
            {
                return RandomString(8);
            }

        }

        public async Task<string> GenerateCode(DateTime date)
        {
            var getLastRegister = await _opportunityRepositoryAsync.ListAsync(new FilterLastOpportunitySpecification());
            return CodeIdentity.OPM + "-" + date.Year.ToString() + "-" + (getLastRegister.Count > 0 ? getLastRegister[0].Id + 1 : 1);
        }
    }
}
