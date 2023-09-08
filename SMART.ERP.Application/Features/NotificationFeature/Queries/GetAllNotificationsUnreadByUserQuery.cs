using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Notification;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.NotificationSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.NotificationFeature.Queries
{
    public class GetAllNotificationsUnreadByUserQuery : IRequest<Response<List<NotificationDto>>>
    {
        public Guid UserId { get; set; }
    }

    public class GetAllNotificationsUnreadByUserQueryHandler : IRequestHandler<GetAllNotificationsUnreadByUserQuery, Response<List<NotificationDto>>>
    {
        private readonly IRepositoryAsync<Notification> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetAllNotificationsUnreadByUserQueryHandler(IRepositoryAsync<Notification> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<List<NotificationDto>>> Handle(GetAllNotificationsUnreadByUserQuery request, CancellationToken cancellationToken)
        {
            var notificationList = await _repositoryAsync.ListAsync(new FilterUnreadNotificationByUserSpecification(request.UserId));
            var dto = _mapper.Map<List<NotificationDto>>(notificationList);
            return new Response<List<NotificationDto>>(dto);
        }
    }
}
