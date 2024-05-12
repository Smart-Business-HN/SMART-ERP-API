using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using SMART.ERP.Application.DTOs.Mail;
using SMART.ERP.Application.DTOs.Notification;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.MailService;
using SMART.ERP.Application.Services.SignalRHub;
using SMART.ERP.Application.Specifications.ClientSpecification;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.MessageFeature.Commands.CreateMessageCommand
{
    public class CreateMessageCommand : IRequest<Response<string>>
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string MessageContent { get; set; } = null!;
        public int CountryId { get; set; }
        public int DepartmentId { get; set; }
        public Guid? CustomerId { get; set; }
    }

    public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommand, Response<string>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Message> _repositoryAsync;
        private readonly IRepositoryAsync<Customer> _customerRepositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IRepositoryAsync<Country> _countryRepositoryAsync;
        private readonly IRepositoryAsync<Department> _departmentRepositoryAsync;
        private readonly IRepositoryAsync<Notification> _notificationRepositoryAsync;
        private readonly IMailService _mailService;
        private readonly IHubContext<NotificationHub> _notificationHub;

        public CreateMessageCommandHandler(IMapper mapper,
            IRepositoryAsync<Message> repositoryAsync,
            IRepositoryAsync<Customer> customerRepositoryAsync,
            IRepositoryAsync<User> userRepositoryAsync,
            IRepositoryAsync<Country> countryRepositoryAsync,
            IRepositoryAsync<Department> departmentRepositoryAsync,
            IRepositoryAsync<Notification> notificationRepositoryAsync,
            IMailService mailService,
            IHubContext<NotificationHub> notificationHub)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _customerRepositoryAsync = customerRepositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _countryRepositoryAsync = countryRepositoryAsync;
            _departmentRepositoryAsync = departmentRepositoryAsync;
            _notificationRepositoryAsync = notificationRepositoryAsync;
            _mailService = mailService;
            _notificationHub = notificationHub;
        }

        public async Task<Response<string>> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
        {
            var checkCountry = await _countryRepositoryAsync.GetByIdAsync(request.CountryId);
            if (checkCountry == null)
            {
                throw new KeyNotFoundException("Ha ocurrido un error con tu solicitud, intenta de nuevo más tarde.");
            }
            var checkDepartment = await _departmentRepositoryAsync.GetByIdAsync(request.DepartmentId);
            if (checkDepartment == null)
            {
                throw new KeyNotFoundException("Ha ocurrido un error con tu solicitud, intenta de nuevo más tarde.");
            }
            var newRecord = _mapper.Map<Message>(request);
            if (request.CustomerId != null)
            {
                var customer = await _customerRepositoryAsync.FirstOrDefaultAsync(
                    new FilterClientByIdSpecification(request.CustomerId));
                if (customer != null)
                {
                    if (newRecord.FirstName != customer.FirstName)
                        throw new ApiException($"El primer nombre {request.FirstName} no coincide con el registrado");
                    else if (newRecord.LastName != customer.LastName)
                        throw new ApiException($"El apellido {request.LastName} no coincide con el registrado");
                    else if (customer.PhoneNumber != newRecord.PhoneNumber)
                        throw new ApiException($"El número de teléfono {request.PhoneNumber} no coincide con el registrado");
                    else if (customer.Email != newRecord.Email)
                        throw new ApiException($"El correo {request.Email} no coincide con el registrado");
                }
                else
                {
                    throw new ApiException($"No se encontro ningun registro con el id: {request.CustomerId}");
                }
            }
            newRecord.FullName = newRecord.FirstName + " " + newRecord.LastName;
            newRecord.Date = DateTime.Now;
            await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();

            var communityManagers = await _userRepositoryAsync.ListAsync(
                new FilterUserByRoleSpecification("SuperAdmin", null));

            if (communityManagers.Count > 0)
            {
                foreach (var manager in communityManagers)
                {
                    var message = new MailRequestDto();
                    message.ToEmail = manager.Email;
                    message.Subject = $"Nuevo mensaje";
                    message.Body = $@"
                    Hola {manager.FullName},<br><br>

                    Se ha recibido un nuevo mensaje en la bandeja de entrada del formulario en linea el {DateTime.Now.ToShortDateString()} a las {DateTime.Now.ToShortTimeString()}. 
Recuerda que puedes ver los detalles del emisor en la bandeja de entrada. Puedes filtrar el mensaje con el correo {request.Email}.";

                    await _mailService.SendEmailAsync(message);

                    var notification = new Notification();
                    notification.Title = "Nuevo mensaje";
                    notification.Icon = "heroicons_outline:inbox-in";
                    notification.Description = $"Se ha recibido un nuevo mensaje en la bandeja de entrada del formulario en linea";
                    notification.Time = DateTime.Now;
                    notification.Read = false;
                    notification.UserId = manager.Id;

                    var response = await _notificationRepositoryAsync.AddAsync(notification);
                    await _notificationRepositoryAsync.SaveChangesAsync();
                    var notificationDto = _mapper.Map<NotificationDto>(response);

                    await _notificationHub.Clients.User(manager.FullName).SendAsync("NewNotification", notificationDto);
                }

            }

            return new Response<string>($"Tu mensaje se envio correctamente, pronto nos pondremos en contacto contigo", "Mensaje enviado correctamente");
        }
    }
}
