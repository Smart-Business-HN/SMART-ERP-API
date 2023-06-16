using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Notification;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.NotificationFeature.Commands.UpdateNotificationCommand
{
    public class UpdateNotificationCommand : IRequest<Response<NotificationDto>>
    {
        public int Id { get; set; }
    }

    public class UpdateNotificationCommandHandler : IRequestHandler<UpdateNotificationCommand, Response<NotificationDto>>
    {
        private readonly IRepositoryAsync<Notification> _repositoryAsync;
        private readonly IMapper _mapper;

        public UpdateNotificationCommandHandler(IRepositoryAsync<Notification> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }
        public async Task<Response<NotificationDto>> Handle(UpdateNotificationCommand request, CancellationToken cancellationToken)
        {
            var checkNotification = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkNotification == null)
            {
                throw new KeyNotFoundException($"No se encontro la notificacion con id {request.Id}");
            }

            checkNotification.Read = true;
            await _repositoryAsync.UpdateAsync(checkNotification);
            await _repositoryAsync.SaveChangesAsync();

            var dto = _mapper.Map<NotificationDto>(checkNotification);

            return new Response<NotificationDto>(dto, "Actualizado correctamente");
        }
    }
}
