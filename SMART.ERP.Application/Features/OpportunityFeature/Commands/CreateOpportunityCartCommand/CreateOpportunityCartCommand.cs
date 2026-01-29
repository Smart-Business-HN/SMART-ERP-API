using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using SMART.ERP.Application.DTOs.Mail;
using SMART.ERP.Application.DTOs.Notification;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.AssignUserToOpportunityService;
using SMART.ERP.Application.Services.MailService;
using SMART.ERP.Application.Services.SignalRHub;
using SMART.ERP.Application.Specifications.InterestLevelSpecification;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Specifications.OpportunityStepSpecification;
using SMART.ERP.Application.Specifications.StatusSpecification;
using SMART.ERP.Application.Specifications.TypeOriginSpecification;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.Specifications.WishListProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.OpportunityFeature.Commands.CreateOpportunityCartCommand
{
    public class CreateOpportunityCartCommand : IRequest<Response<string>>
    {
        public int WishListId { get; set; }
    }

    public class CreateOpportunityCartCommandHandler : IRequestHandler<CreateOpportunityCartCommand, Response<string>>
    {
        private readonly IRepositoryAsync<Opportunity> _repositoryAsync;
        private readonly IRepositoryAsync<QuoteProduct> _quoteRepositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IRepositoryAsync<Customer> _customerRepositoryAsync;
        private readonly IRepositoryAsync<Customer> _clientRepositoryAsync;
        private readonly IRepositoryAsync<InterestLevel> _interestRepositoryAsync;
        private readonly IRepositoryAsync<OpportunityStep> _stepRepositoryAsync;
        private readonly IRepositoryAsync<TypeOrigin> _originRepositoryAsync;
        private readonly IRepositoryAsync<Product> _productRepositoryAsync;
        private readonly IAssignUserToOpportunityService _assignUser;
        private readonly IRepositoryAsync<WishList> _wishListRepositoryAsync;
        private readonly IRepositoryAsync<WishListProduct> _wishProductRepositoryAsync;
        private readonly IRepositoryAsync<Status> _statusRepositoryAsync;
        private readonly IMailService _mailService;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IRepositoryAsync<Notification> _notificationRepositoryAsync;
        private readonly IMapper _mapper;

        public CreateOpportunityCartCommandHandler(IRepositoryAsync<Opportunity> repositoryAsync, IRepositoryAsync<QuoteProduct> quoteRepositoryAsync, IRepositoryAsync<User> userRepositoryAsync,
            IRepositoryAsync<Customer> customerRepositoryAsync, IRepositoryAsync<Customer> clientRepositoryAsync, IRepositoryAsync<InterestLevel> interestRepositoryAsync,
            IRepositoryAsync<OpportunityStep> stepRepositoryAsync, IRepositoryAsync<TypeOrigin> originRepositoryAsync, IRepositoryAsync<Product> productRepositoryAsync,
            IAssignUserToOpportunityService assignUser, IRepositoryAsync<WishList> wishListRepositoryAsync, IRepositoryAsync<WishListProduct> wishProductRepositoryAsync,
            IRepositoryAsync<Status> statusRepositoryAsync, IMailService mailService, IHubContext<NotificationHub> hubContext,
            IRepositoryAsync<Notification> notificationRepositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _quoteRepositoryAsync = quoteRepositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _customerRepositoryAsync = customerRepositoryAsync;
            _clientRepositoryAsync = clientRepositoryAsync;
            _interestRepositoryAsync = interestRepositoryAsync;
            _stepRepositoryAsync = stepRepositoryAsync;
            _originRepositoryAsync = originRepositoryAsync;
            _productRepositoryAsync = productRepositoryAsync;
            _assignUser = assignUser;
            _wishListRepositoryAsync = wishListRepositoryAsync;
            _wishProductRepositoryAsync = wishProductRepositoryAsync;
            _statusRepositoryAsync = statusRepositoryAsync;
            _mailService = mailService;
            _hubContext = hubContext;
            _notificationRepositoryAsync = notificationRepositoryAsync;
            _mapper = mapper;
        }
        public async Task<Response<string>> Handle(CreateOpportunityCartCommand request, CancellationToken cancellationToken)
        {
            var origin = await _originRepositoryAsync.FirstOrDefaultAsync(new FilterTypeOriginSpecification("Carrito", null));

            var checkIfWishListExist = await _wishListRepositoryAsync.GetByIdAsync(request.WishListId);
            if (checkIfWishListExist == null || checkIfWishListExist.IsActive != true)
            {
                throw new KeyNotFoundException($"Ocurrio un error con la lista de deseos, ponte en contacto con un administrador");
            }

            var checkIfMotorsCustomerExist = await _customerRepositoryAsync.GetByIdAsync(checkIfWishListExist.CustomerId);
            if (checkIfMotorsCustomerExist == null)
            {
                throw new KeyNotFoundException("No se encontro el cliente de Motors vinculado a esta lista de deseos.");
            }

            var HNClient = await _clientRepositoryAsync.GetByIdAsync(checkIfMotorsCustomerExist.Id);
            if (HNClient == null)
            {
                throw new KeyNotFoundException($"No se encontro el cliente HN con id {checkIfMotorsCustomerExist.Id}");
            }

            var processedStatus = await _statusRepositoryAsync.FirstOrDefaultAsync(new FilterStatusFromNameSpecification("Procesada"));
            checkIfWishListExist.IsActive = false;
            checkIfWishListExist.StatusId = processedStatus!.Id;
            var productsList = await _wishProductRepositoryAsync.ListAsync(new FilterWishListProductByWishListIdSpecification(request.WishListId));

            var interestLow = await _interestRepositoryAsync.FirstOrDefaultAsync(new FilterInterestLevelSpecification("Bajo", null));
            var identificationStep = await _stepRepositoryAsync.FirstOrDefaultAsync(new FilterOpportunityStepSpecification("Identificacion", null));


            var newOpportunity = new Opportunity();
            newOpportunity.CustomerId = checkIfMotorsCustomerExist.Id;
            newOpportunity.CreationDate = DateTime.Now;
            newOpportunity.IsActive = true;
            newOpportunity.InterestLevelId = interestLow!.Id;
            newOpportunity.OpportunityStepId = identificationStep!.Id;
            newOpportunity.Position = 1;
            newOpportunity.ApplyOnCredit = false;
            newOpportunity.QtyItems = productsList.Count;
            newOpportunity.TypeOriginId = origin!.Id;
            newOpportunity.Total = 0;
            newOpportunity.ProbabilityPercentage = 0;

            if (checkIfMotorsCustomerExist.UserId != null)
            {
                newOpportunity.UserId = (Guid)checkIfMotorsCustomerExist.UserId;
            }
            else
            {
                newOpportunity.UserId = await _assignUser!.FindValidUser();
            }

            foreach (var product in productsList)
            {
                var checkIfProductExist = await _productRepositoryAsync.GetByIdAsync(product.ProductId);
                if (checkIfProductExist == null)
                {
                    throw new KeyNotFoundException($"No se encontro el producto con id {product.ProductId}");

                }
                newOpportunity.Total += checkIfProductExist.RecomendedSalePrice * product.Quantity;
            }

            newOpportunity.Code = await GenerateCode(DateTime.Now);

            var opportunityList = await _repositoryAsync.ListAsync(new FilterOpportunityByStepSpecification(identificationStep.Id, null, null, 1, newOpportunity.UserId));
            if (opportunityList.Count > 0)
            {
                foreach (var item in opportunityList)
                {
                    item.Position += 1;
                }
                await _repositoryAsync.UpdateRangeAsync(opportunityList);
                await _repositoryAsync.SaveChangesAsync();
            }

            var opportunity = await _repositoryAsync.AddAsync(newOpportunity);
            await _repositoryAsync.SaveChangesAsync();

            foreach (var product in productsList)
            {
                var newQuote = new QuoteProduct();
                newQuote.ProductId = product.ProductId;
                newQuote.OpportunityId = opportunity.Id;
                newQuote.Quantity = product.Quantity;
                newQuote.SalePrice = product.Product!.RecomendedSalePrice;

                await _quoteRepositoryAsync.AddAsync(newQuote);
                await _quoteRepositoryAsync.SaveChangesAsync();
            }

            await _wishListRepositoryAsync.UpdateAsync(checkIfWishListExist);
            await _wishListRepositoryAsync.SaveChangesAsync();

            var assignedUser = await _userRepositoryAsync.GetByIdAsync(newOpportunity.UserId);
            var manager = await _userRepositoryAsync.FirstOrDefaultAsync(new FilterUserByRoleSpecification("Manager", assignedUser!.BranchOfficeId));

            var mailRequest = new MailRequestDto();
            mailRequest.Subject = "Nueva cotización desde lista de deseo";
            mailRequest.ToEmail = assignedUser!.Email;
            mailRequest.Body = $@"Hola {assignedUser!.FullName}!<br><br>
El cliente {HNClient.FullName} a solicitado una cotizacion desde su lista de deseos. Puedes consultar la informacion de la oportunidad desde el codigo {opportunity.Code} o ingresando 
directamente al enlace: https://adminpm.platino.hn/#/crm/opportunity/{opportunity.Id}. Recuerda que puedes consultar la informacion del cliente desde el panel administrativo si deseas 
ponerte en contacto con el, o puedes comunicarte directamente al correo {HNClient.Email}";

            var notification = new Notification();
            notification.Title = "Nueva oportunidad";
            notification.Icon = "heroicons_outline:information-circle";
            notification.Description = $"El cliente <b>{HNClient.FullName}</b> ha generado la oportunidad <b>{opportunity.Code}</b> desde su lista de deseos. Verifica la información.";
            notification.Time = DateTime.Now;
            notification.UseRouter = true;
            notification.Link = "/crm/opportunity/" + opportunity.Id;
            notification.Read = false;
            notification.UserId = assignedUser!.Id;

            var response = await _notificationRepositoryAsync.AddAsync(notification);
            await _notificationRepositoryAsync.SaveChangesAsync();

            if (manager != null)
            {
                var managerNotification = new Notification();
                managerNotification.Title = "Nueva oportunidad";
                managerNotification.Icon = "heroicons_outline:information-circle";
                managerNotification.Description = $"El cliente <b>{HNClient.FullName}</b> ha generado la oportunidad <b>{opportunity.Code}</b> desde su lista de deseos. Verifica la información.";
                managerNotification.Time = DateTime.Now;
                managerNotification.UseRouter = true;
                managerNotification.Link = "/crm/opportunity/" + opportunity.Id;
                managerNotification.Read = false;
                managerNotification.UserId = manager.Id;
                var managerResponse = await _notificationRepositoryAsync.AddAsync(managerNotification);
                await _notificationRepositoryAsync.SaveChangesAsync();

                var managerNotificationDto = _mapper.Map<NotificationDto>(managerResponse);
                await _hubContext.Clients.User(manager.FullName).SendAsync("NewNotification", managerNotificationDto);
            }

            var notificationDto = _mapper.Map<NotificationDto>(response);
            await _mailService.SendEmailAsync(mailRequest);
            await _hubContext.Clients.User(assignedUser!.FullName).SendAsync("NewNotification", notificationDto);


            return new Response<string>("Se ha recibido tu solicitud de cotizacion, pronto uno de nuestros asesores te contactará.");

        }

        public async Task<string> GenerateCode(DateTime date)
        {
            var getLastRegister = await _repositoryAsync.ListAsync(new FilterLastOpportunitySpecification());
            return CodeIdentity.OPM + "-" + date.Year.ToString() + "-" + (getLastRegister.Count > 0 ? getLastRegister[0].Id + 1 : 1);
        }
    }
}
