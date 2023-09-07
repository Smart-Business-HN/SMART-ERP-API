using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using SMART.ERP.Application.DTOs.Mail;
using SMART.ERP.Application.DTOs.Notification;
using SMART.ERP.Application.DTOs.Prospect;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.AssignUserToProspectService;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Services.MailService;
using SMART.ERP.Application.Services.SignalRHub;
using SMART.ERP.Application.Specifications.ClientSpecification;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Specifications.ProspectSpecification;
using SMART.ERP.Application.Specifications.ProspectStepSpecification;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProspectFeature.Commands.CreateProspectCommand
{
    public class CreateProspectCommand : IRequest<Response<ProspectDto>>
    {
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? Email { get; set; }
        public int HeadingId { get; set; }
        public int TypeOriginId { get; set; }
        public int SocialReasonId { get; set; }
        public int DepartmentId { get; set; }
        public int? CountryId { get; set; }
        public int? CityId { get; set; }
        public int? PostalCode { get; set; }
        public string? Address { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? MetaAdCampaignId { get; set; }
        public int? GenderId { get; set; }
        public string? PreferredContactMethod { get; set; }
        public bool AccountHN { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactPersonPhone { get; set; }
        public string? ContactPersonEmail { get; set; }
        public List<int> Products { get; set; } = null!;
    }

    public class CreateProspectCommandHandler : IRequestHandler<CreateProspectCommand, Response<ProspectDto>>
    {
        private readonly IRepositoryAsync<Prospect> _repositoryAsync;
        private readonly IRepositoryAsync<Heading> _headingRepositoryAsync;
        private readonly IRepositoryAsync<SocialReason> _socialReasonRepositoryAsync;
        private readonly IRepositoryAsync<TypeOrigin> _originRepositoryAsync;
        private readonly IRepositoryAsync<Customer> _clientRepositoryAsync;
        private readonly IRepositoryAsync<Department> _departmentRepositoryAsync;
        private readonly IAssignUserToProspectService _assignUser;
        private readonly IRepositoryAsync<ProspectStep> _stepRepositoryAsync;
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<City> _cityRepositoryAsync;
        private readonly IRepositoryAsync<Country> _countryRepositoryAsync;
        private readonly IRepositoryAsync<Gender> _genderRepositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IMailService _mailService;
        private readonly IHubContext<NotificationHub> _notificationHub;
        private readonly IRepositoryAsync<Notification> _notificationRepositoryAsync;
        private readonly IRepositoryAsync<Product> _productRepositoryAsync;
        private readonly IRepositoryAsync<ProspectQuoteProduct> _quoteProductRepositoryAsync;
        private readonly IRepositoryAsync<MetaAdCampaign> _metaAdRepositoryAsync;

        public CreateProspectCommandHandler(IRepositoryAsync<Prospect> repositoryAsync, IRepositoryAsync<Heading> headingRepositoryAsync,
            IRepositoryAsync<SocialReason> socialReasonRepositoryAsync, IRepositoryAsync<TypeOrigin> originRepositoryAsync,
            IRepositoryAsync<Customer> clientRepositoryAsync, IRepositoryAsync<Department> departmentRepositoryAsync,
            IAssignUserToProspectService assignUser, IRepositoryAsync<ProspectStep> stepRepositoryAsync, IMapper mapper,
            IRepositoryAsync<City> cityRepositoryAsync, IRepositoryAsync<Country> countryRepositoryAsync, IRepositoryAsync<Gender> genderRepositoryAsync,
            IJwtService jwtService, IRepositoryAsync<User> userRepositoryAsync, IMailService mailService,
            IHubContext<NotificationHub> notificationHub, IRepositoryAsync<Notification> notificationRepositoryAsync,
            IRepositoryAsync<Product> productRepositoryAsync, IRepositoryAsync<ProspectQuoteProduct> quoteProductRepositoryAsync,
            IRepositoryAsync<MetaAdCampaign> metaAdRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _headingRepositoryAsync = headingRepositoryAsync;
            _socialReasonRepositoryAsync = socialReasonRepositoryAsync;
            _originRepositoryAsync = originRepositoryAsync;
            _clientRepositoryAsync = clientRepositoryAsync;
            _departmentRepositoryAsync = departmentRepositoryAsync;
            _assignUser = assignUser;
            _stepRepositoryAsync = stepRepositoryAsync;
            _mapper = mapper;
            _cityRepositoryAsync = cityRepositoryAsync;
            _countryRepositoryAsync = countryRepositoryAsync;
            _genderRepositoryAsync = genderRepositoryAsync;
            _jwtService = jwtService;
            _userRepositoryAsync = userRepositoryAsync;
            _mailService = mailService;
            _notificationHub = notificationHub;
            _notificationRepositoryAsync = notificationRepositoryAsync;
            _productRepositoryAsync = productRepositoryAsync;
            _quoteProductRepositoryAsync = quoteProductRepositoryAsync;
            _metaAdRepositoryAsync = metaAdRepositoryAsync;
        }

        public async Task<Response<ProspectDto>> Handle(CreateProspectCommand request, CancellationToken cancellationToken)
        {
            var guid = _jwtService.GetUidToken();
            var checkUser = await _userRepositoryAsync.FirstOrDefaultAsync(new UserIncludesSpecification(guid, null));
            if (checkUser == null)
            {
                throw new ApiException("Usuario invalido");
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
            var checkDepartment = await _departmentRepositoryAsync.GetByIdAsync(request.DepartmentId);
            if (checkDepartment == null)
            {
                throw new KeyNotFoundException($"No se encontro el departamento con id {request.DepartmentId}");
            }
            if (checkTypeOrigin.IsProspectOrigin == false)
            {
                throw new ApiException("Este origen no esta disponible para prospectos");
            }

            var checkFields = await _repositoryAsync.FirstOrDefaultAsync(new FilterProspectByUniqueFieldsSpecification(
                request.FullName, request.PhoneNumber, request.Email, null));
            if (checkFields != null)
            {
                if (checkFields.FullName == request.FullName)
                {
                    throw new ApiException("Ya existe un prospecto con este nombre");
                }

                if (!string.IsNullOrWhiteSpace(request.Email))
                {
                    if (checkFields.Email == request.Email)
                    {
                        throw new ApiException("Ya existe un prospecto con este correo");
                    }
                }

                if (checkFields.PhoneNumber == request.PhoneNumber)
                {
                    throw new ApiException("Ya existe un prospecto con este número telefónico");
                }
            }

            var checkClientFields = await _clientRepositoryAsync.FirstOrDefaultAsync(new FilterClientByUniqueValues(request.FullName, request.PhoneNumber, request.Email, null));
            if (checkClientFields != null)
            {
                if (checkClientFields.FullName == request.FullName)
                {
                    throw new ApiException("Ya existe un cliente con este nombre");
                }
                if (checkClientFields.Email == request.Email)
                {
                    throw new ApiException("Ya existe un cliente con este correo");
                }
                if (checkClientFields.PhoneNumber == request.PhoneNumber)
                {
                    throw new ApiException("Ya existe un cliente con este número telefónico");
                }
            }

            Guid? assignedUser = null;
            var products = await _productRepositoryAsync.ListAsync(new ProductIncludesSpecification(null, slug: null));
            List<ProspectQuoteProduct> productsProspect = new();
            foreach (var productId in request.Products)
            {
                var checkProduct = products.FirstOrDefault(x => x.Id == productId);
                if (checkProduct == null)
                {
                    throw new KeyNotFoundException($"No se encontro el producto con id {productId}");
                }
                if (checkProduct.Brand!.Name == "Menegotti" && assignedUser == null)
                {
                    if (productsProspect.Count > 0)
                    {
                        foreach (var quote in productsProspect)
                        {
                            var checkProductBrand = products.Find(x => x.Id == quote.ProductId);
                            if (checkProductBrand!.Brand!.Name != "Menegotti")
                            {
                                throw new ApiException("No puedes agregar productos de diferentes marcas a este prospecto.");
                            }
                        }
                    }
                    var menegottiUser = await _userRepositoryAsync.FirstOrDefaultAsync(new FilterUserSpecification("Oscar Flores", null));
                    assignedUser = menegottiUser!.Id;
                }
                if (checkProduct.Brand!.Name != "Menegotti" && assignedUser != null)
                {
                    throw new ApiException("No puedes agregar productos de diferentes marcas a este prospecto.");
                }
                ProspectQuoteProduct newQuote = new();
                newQuote.ProductId = productId;
                newQuote.IsActive = true;
                productsProspect.Add(newQuote);
            }

            var firstStep = await _stepRepositoryAsync.FirstOrDefaultAsync(new FilterProspectStepSpecification("Pendiente"));
            if (assignedUser == null)
            {
                if (checkUser.Role!.Name == "Sales Advisor")
                {
                    assignedUser = checkUser!.Id;
                }
                else
                {
                    assignedUser = await _assignUser!.FindValidUserDepartment(checkDepartment.Id);
                }
            }

            var newRecord = new Prospect
            {
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                Email = string.IsNullOrEmpty(request.Email) ? null : request.Email,
                HeadingName = checkHeading.Name,
                SocialReasonName = checkSocialReason.Name,
                TypeOriginId = checkTypeOrigin.Id,
                DepartmentId = request.DepartmentId,
                UserId = (Guid)assignedUser,
                ProspectStepId = firstStep!.Id,
                PreferredContactMethod = request.PreferredContactMethod,
                ContactPerson = request.ContactPerson,
                ContactPersonEmail = request.ContactPersonEmail,
                ContactPersonPhone = request.ContactPersonPhone,
                WebsiteUrl = request.WebsiteUrl,
                Address = request.Address,
                PostalCode = request.PostalCode == 0 || request.PostalCode == null ? null : request.PostalCode,
                AccountHN = request.AccountHN,
                CreationDate = DateTime.Now,
                CreatedBy = _jwtService.GetSubjectToken()
            };
            if (request.GenderId != null)
            {
                var checkGender = await _genderRepositoryAsync.GetByIdAsync((int)request.GenderId);
                if (checkGender == null)
                {
                    throw new KeyNotFoundException($"No se encontro el genero con id {request.GenderId}");
                }
                newRecord.GenderId = checkGender.Id;
            }
            if (!string.IsNullOrEmpty(request.MetaAdCampaignId))
            {
                var checkAd = await _metaAdRepositoryAsync.GetByIdAsync(request.MetaAdCampaignId);
                if (checkAd == null)
                {
                    throw new KeyNotFoundException($"No se encontro la camapaña con id {request.MetaAdCampaignId}");
                }
                newRecord.MetaAdCampaignId = checkAd.Id;
            }
            if (request.CountryId != null)
            {
                var checkCountry = await _countryRepositoryAsync.GetByIdAsync((int)request.CountryId);
                if (checkCountry == null)
                {
                    throw new KeyNotFoundException($"No se encontro el pais con id {request.CountryId}");
                }
                newRecord.CountryId = checkCountry.Id;
            }
            if (request.CityId != null)
            {
                var checkCity = await _cityRepositoryAsync.GetByIdAsync((int)request.CityId);
                if (checkCity == null)
                {
                    throw new KeyNotFoundException($"No se encontro la ciudad con id {request.CityId}");
                }
                newRecord.CityId = checkCity.Id;
            }

            var response = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();

            foreach (var quote in productsProspect)
            {
                quote.ProspectId = response.Id;
            }

            await _quoteProductRepositoryAsync.AddRangeAsync(productsProspect);
            await _quoteProductRepositoryAsync.SaveChangesAsync();

            var getUser = await _userRepositoryAsync.GetByIdAsync((Guid)assignedUser);
            if (assignedUser != checkUser!.Id)
            {
                var notification = new Notification();
                notification.Title = "Ingreso de nuevo Lead";
                notification.Description = "Se ha ingresado un Lead con uno de tus departamentos asignados, verifica la informacion.";
                notification.Icon = "heroicons_outline:information-circle";
                notification.Read = false;
                notification.UseRouter = true;
                notification.Link = $"/crm/prospects/details/{response.Id}";
                notification.Time = DateTime.Now;
                notification.UserId = (Guid)assignedUser;

                var mailRequest = new MailRequestDto();
                mailRequest.Subject = "Ingreso de nuevo Lead";
                mailRequest.ToEmail = getUser!.Email;
                mailRequest.Body = $@"Hola {getUser!.FullName}!<br><br>
Se ha ingresado un nuevo prospecto al sistema. Puedes consultar la informacion del prospecto {newRecord.FullName} o ingresando 
directamente al enlace: https://adminpm.platino.hn/#/crm/prospects/details/{response.Id}";

                var notificationResponse = await _notificationRepositoryAsync.AddAsync(notification);
                await _notificationRepositoryAsync.SaveChangesAsync();

                var notificationDto = _mapper.Map<NotificationDto>(notificationResponse);

                await _notificationHub.Clients.User(getUser!.FullName).SendAsync("NewNotification", notificationDto);
                await _mailService.SendEmailAsync(mailRequest);
            }

            var dto = _mapper.Map<ProspectDto>(response);
            return new Response<ProspectDto>(dto, $"Prospecto {response.FullName} creado exitosamente");
        }
    }
}
