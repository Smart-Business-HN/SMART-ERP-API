using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ClientSpecification;
using SMART.ERP.Application.Specifications.CountrySpecification;
using SMART.ERP.Application.Specifications.InterestLevelSpecification;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Specifications.OpportunityStepSpecification;
using SMART.ERP.Application.Specifications.ProspectSpecification;
using SMART.ERP.Application.Specifications.TypeOriginSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;
using SMART.ERP.Application.Services.AssignUserToOpportunityService;

namespace SMART.ERP.Application.Features.ProspectFeature.Commands.CreateProspectXpressCommand
{
    public class CreateProspectXpressCommand : IRequest<Response<string>>
    {
        public Guid? CustomerId { get; set; }
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int DepartmentId { get; set; }
        public int ProductId { get; set; }
    }

    public class CreateProspectXpressCommandHandler : IRequestHandler<CreateProspectXpressCommand, Response<string>>
    {
        private readonly IRepositoryAsync<Prospect> _repositoryAsync;
        private readonly IRepositoryAsync<Department> _departmentRepositoryAsync;
        private readonly IRepositoryAsync<ProspectQuoteProduct> _quoteRepositoryAsync;
        private readonly IRepositoryAsync<Product> _productRepositoryAsync;
        private readonly IRepositoryAsync<Opportunity> _opportunityRepositoryAsync;
        private readonly IRepositoryAsync<Customer> _customerRepositoryAsync;
        private readonly IRepositoryAsync<QuoteProduct> _quoteProductRepositoryAsync;
        private readonly IRepositoryAsync<OpportunityStep> _stepRepositoryAsync;
        private readonly IRepositoryAsync<InterestLevel> _interestRepositoryAsync;
        private readonly IAssignUserToOpportunityService _assignUserToOpportunityService;
        private readonly IRepositoryAsync<Customer> _clientRepositoryAsync;
        private readonly IRepositoryAsync<ProspectStep> _prospectStepRepositoryAsync;
        private readonly IRepositoryAsync<Country> _countryRepositoryAsync;
        private readonly IRepositoryAsync<TypeOrigin> _originRepositoryAsync;

        public CreateProspectXpressCommandHandler(IRepositoryAsync<Prospect> repositoryAsync, IRepositoryAsync<Department> departmentRepositoryAsync,
            IRepositoryAsync<ProspectQuoteProduct> quoteRepositoryAsync, IRepositoryAsync<Product> productRepositoryAsync,
            IRepositoryAsync<Opportunity> opportunityRepositoryAsync, IRepositoryAsync<Customer> customerRepositoryAsync,
            IRepositoryAsync<QuoteProduct> quoteProductRepositoryAsync, IRepositoryAsync<OpportunityStep> stepRepositoryAsync,
            IRepositoryAsync<InterestLevel> interestRepositoryAsync, IAssignUserToOpportunityService assignUserToOpportunityService,
            IRepositoryAsync<Customer> clientRepositoryAsync, IRepositoryAsync<ProspectStep> prospectStepRepositoryAsync,
            IRepositoryAsync<Country> countryRepositoryAsync, IRepositoryAsync<TypeOrigin> originRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _departmentRepositoryAsync = departmentRepositoryAsync;
            _quoteRepositoryAsync = quoteRepositoryAsync;
            _productRepositoryAsync = productRepositoryAsync;
            _opportunityRepositoryAsync = opportunityRepositoryAsync;
            _customerRepositoryAsync = customerRepositoryAsync;
            _quoteProductRepositoryAsync = quoteProductRepositoryAsync;
            _stepRepositoryAsync = stepRepositoryAsync;
            _interestRepositoryAsync = interestRepositoryAsync;
            _assignUserToOpportunityService = assignUserToOpportunityService;
            _clientRepositoryAsync = clientRepositoryAsync;
            _prospectStepRepositoryAsync = prospectStepRepositoryAsync;
            _countryRepositoryAsync = countryRepositoryAsync;
            _originRepositoryAsync = originRepositoryAsync;
        }

        public async Task<Response<string>> Handle(CreateProspectXpressCommand request, CancellationToken cancellationToken)
        {
            var checkDepartment = await _departmentRepositoryAsync.GetByIdAsync(request.DepartmentId);
            if (checkDepartment == null)
            {
                throw new KeyNotFoundException($"No se encontro el departamento con id {request.DepartmentId}");
            }
            var checkProduct = await _productRepositoryAsync.GetByIdAsync(request.ProductId);
            if (checkProduct == null)
            {
                throw new KeyNotFoundException($"Ha ocurrido un problema con este producto, contacta a soporte tecnico.");
            }
            if (request.CustomerId != null)
            {
                var checkCustomer = await _customerRepositoryAsync.GetByIdAsync((Guid)request.CustomerId);
                if (checkCustomer == null)
                {
                    throw new KeyNotFoundException("Ha ocurrido un error con tu sesion, contacta a soporte tecnico.");
                }
                var checkActiveOpportunity = await _opportunityRepositoryAsync.ListAsync(new FilterActiveOpportunitiesByCustomerSpecification(request.CustomerId, true));
                if (checkActiveOpportunity.Count > 0)
                {
                    var identificationOpportunity = checkActiveOpportunity.FirstOrDefault(x => x.OpportunityStep!.Name == "Identificacion");
                    if (identificationOpportunity != null)
                    {
                        var addQuote = new QuoteProduct
                        {
                            OpportunityId = identificationOpportunity.Id,
                            IsActive = true,
                            Quantity = 1,
                            SalePrice = checkProduct!.RecomendedSalePrice
                        };

                        await _quoteProductRepositoryAsync.AddAsync(addQuote);
                        await _quoteProductRepositoryAsync.SaveChangesAsync();

                        return new Response<string>("Se ha recibido tu solicitud de cotizacion, dentro de poco uno de nuestros asesores te contactara.");
                    }
                }
                var identificationStep = await _stepRepositoryAsync.FirstOrDefaultAsync(new FilterOpportunityStepSpecification("Identificacion", null));
                var lowInterest = await _interestRepositoryAsync.FirstOrDefaultAsync(new FilterInterestLevelSpecification("Bajo", null));
                var newOpportunity = new Opportunity
                {
                    Code = await GenerateCode(DateTime.Now),
                    CustomerId = checkCustomer!.Id,
                    CreationDate = DateTime.Now,
                    UserId = checkCustomer!.UserId != null ? (Guid)checkCustomer!.UserId : await _assignUserToOpportunityService.FindValidDepartmentUser(request.DepartmentId),
                    OpportunityStepId = identificationStep!.Id,
                    Position = 1,
                    Total = 0,
                    QtyItems = 0,
                    ProbabilityPercentage = 0,
                    InterestLevelId = lowInterest!.Id
                };

                var identificationContainer = await _opportunityRepositoryAsync.ListAsync(new FilterOpportunityByStepSpecification(identificationStep.Id, null, null, 1, newOpportunity.UserId));
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

                var productList = await _productRepositoryAsync.ListAsync();
                var newQuote = new QuoteProduct
                {
                    ProductId = request.ProductId,
                    SalePrice = productList.Find(x => x.Id == request.ProductId)!.RecomendedSalePrice,
                    OpportunityId = opportunityResponse.Id,
                    IsActive = true,
                    Quantity = 1
                };

                opportunityResponse.Total += productList.Find(x => x.Id == request.ProductId)!.RecomendedSalePrice;

                opportunityResponse.QtyItems = 1;
                await _quoteProductRepositoryAsync.AddAsync(newQuote);
                await _quoteProductRepositoryAsync.SaveChangesAsync();

                await _opportunityRepositoryAsync.UpdateAsync(opportunityResponse);
                await _opportunityRepositoryAsync.SaveChangesAsync();

                return new Response<string>("Se ha recibido tu solicitud de cotizacion, dentro de poco uno de nuestros asesores te contactara.");
            }
            else
            {
                var checkClientFields = await _clientRepositoryAsync.FirstOrDefaultAsync(new FilterClientByUniqueValues(request.FullName, request.PhoneNumber, request.Email, null));
                if (checkClientFields != null && checkClientFields.IsActive)
                {
                    throw new ApiException("Ya existe un cliente con estos datos, inicia sesion.");
                }
                var checkFields = await _repositoryAsync.FirstOrDefaultAsync(new FilterProspectByUniqueFieldsSpecification(request.FullName, request.PhoneNumber, request.Email, null));
                if (checkFields != null)
                {
                    if (checkFields.FullName == request.FullName && checkFields.Email == request.Email && checkFields.PhoneNumber == request.PhoneNumber)
                    {
                        var newQuote = new ProspectQuoteProduct
                        {
                            ProspectId = checkFields.Id,
                            ProductId = request.ProductId,
                            IsActive = true
                        };

                        var response = await _quoteRepositoryAsync.AddAsync(newQuote);
                        await _quoteRepositoryAsync.SaveChangesAsync();

                        return new Response<string>("Se ha recibido tu solicitud de cotización, dentro de poco uno de nuestros asesores te contactará.");
                    }
                    else
                    {
                        throw new ApiException("Ya existe un cliente con estos datos, inicia sesión.");
                    }
                }
                else
                {
                    var onlineformOrigin = await _originRepositoryAsync.FirstOrDefaultAsync(new FilterTypeOriginSpecification("Formulario en linea", null));
                    var country = await _countryRepositoryAsync.FirstOrDefaultAsync(new FindCountryByDepartmentSpecification(request.DepartmentId));
                    var pendingId = await _prospectStepRepositoryAsync.ListAsync();
                    var newRecord = new Prospect
                    {
                        FullName = request.FullName,
                        PhoneNumber = request.PhoneNumber,
                        Email = request.Email,
                        DepartmentId = request.DepartmentId,
                        ProspectStepId = pendingId.Find(x => x.Name == "Pendiente")!.Id,
                        UserId = await _assignUserToOpportunityService.FindValidDepartmentUser(request.DepartmentId),
                        HeadingName = "Sin asignar",
                        SocialReasonName = "Natural",
                        TypeOriginId = onlineformOrigin!.Id,
                        CountryId = country!.Id,
                        CreatedBy = "Formulario en linea",
                        CreationDate = DateTime.Now
                    };

                    var response = await _repositoryAsync.AddAsync(newRecord);
                    await _repositoryAsync.SaveChangesAsync();

                    var newQuote = new ProspectQuoteProduct
                    {
                        ProspectId = response.Id,
                        ProductId = request.ProductId,
                        IsActive = true
                    };

                    var quoteResponse = await _quoteRepositoryAsync.AddAsync(newQuote);
                    await _quoteRepositoryAsync.SaveChangesAsync();

                    return new Response<string>("Se ha recibido tu solicitud de cotización, dentro de poco uno de nuestros asesores te contactará.");

                }
            }
        }

        public async Task<string> GenerateCode(DateTime date)
        {
            var getLastRegister = await _opportunityRepositoryAsync.ListAsync(new FilterLastOpportunitySpecification());
            return CodeIdentity.OPM + "-" + date.Year.ToString() + "-" + (getLastRegister.Count > 0 ? getLastRegister[0].Id + 1 : 1);
        }
    }
}
